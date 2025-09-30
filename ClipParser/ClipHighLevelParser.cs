#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.rs64.ParserUtility;
using net.rs64.TexTransTool.MultiLayerImage.LayerData;

namespace net.rs64.TexTransTool.ClipParser
{
    public static class ClipHighLevelParser
    {

        public static ClipHighLevelData Parse(byte[] bytes, ClipLowLevelData lowLevelData, ISQLiteDBConnector dbConnector)
        {
            var sqlite3DBRawBytes = new BinarySectionStream(bytes, lowLevelData.SQLiteDataAdders).ReadToArray();

            using var sqlite = dbConnector.Deserialize(sqlite3DBRawBytes);
            var canvas = sqlite.QueryCanvas();
            var layers = sqlite.QueryLayer();
            var offscreen = sqlite.QueryOffscreen();
            var mipmap = sqlite.QueryMipMap();
            var mipmapInfo = sqlite.QueryMipMapInfo();

            var canvasData = new List<ClipCanvasData>();
            var hlCtx = new highLevelParsingContext(layers, offscreen, mipmap, mipmapInfo, lowLevelData.CHNKExtaList);

            foreach (var c in canvas)
            {
                var layersData = new List<AbstractLayerData>();
                hlCtx.GenerateLayerData(layersData, hlCtx.GetLayer(c.CanvasRootFolder));
                canvasData.Add(new(layersData) { Width = (int)c.CanvasWidth, Height = (int)c.CanvasHeight });
            }

            return new ClipHighLevelData(canvasData);


        }
        class highLevelParsingContext
        {
            List<ILayerRecord> layers;
            List<IOffscreenRecord> offscreen;
            List<IMipmapRecord> mipmap;
            List<IMipmapInfoRecord> mipmapInfo;
            List<ExtraData> extraData;

            public highLevelParsingContext(List<ILayerRecord> layers, List<IOffscreenRecord> offscreen, List<IMipmapRecord> mipmap, List<IMipmapInfoRecord> mipmapInfo, List<ExtraData> extraData)
            {
                this.layers = layers;
                this.offscreen = offscreen;
                this.mipmap = mipmap;
                this.mipmapInfo = mipmapInfo;
                this.extraData = extraData;
            }
            public ILayerRecord GetLayer(long id)
            {
                return layers.First(l => l.MainId == id);
            }
            public void GenerateLayerData(List<AbstractLayerData> layersData, ILayerRecord layer)
            {
                var nextLayerIndex = layer.LayerFirstChildIndex;
                while (nextLayerIndex is not 0)
                {
                    var cLayer = GetLayer(nextLayerIndex);
                    if (cLayer.LayerFolder is not 0)
                    {
                        var lf = new LayerFolderData() { Layers = new() };
                        layersData.Add(lf);

                        WriteData(lf, cLayer);

                        var maskMipMapID = cLayer.LayerLayerMaskMipmap;
                        var maskOffscreenID = GetOffscreenID(maskMipMapID);
                        if (maskOffscreenID is not null)
                        {
                            var data = GetExtraDataFromOffscreenID(maskOffscreenID.Value);
                            if (data is not null)
                                lf.LayerMask = new()
                                {
                                    LayerMaskDisabled = false,
                                    MaskTexture = new ClipImportedRasterImageData(data)
                                };
                        }

                        GenerateLayerData(lf.Layers, cLayer);
                    }
                    else
                    {
                        var rasterLayerData = new RasterLayerData();
                        layersData.Add(rasterLayerData);
                        WriteData(rasterLayerData, cLayer);

                        var mipMapID = cLayer.LayerRenderMipmap;
                        var offscreenID = GetOffscreenID(mipMapID);
                        var maskMipMapID = cLayer.LayerLayerMaskMipmap;
                        var maskOffscreenID = GetOffscreenID(maskMipMapID);

                        if (offscreenID is not null)
                        {
                            var data = GetExtraDataFromOffscreenID(offscreenID.Value);
                            if (data is not null)
                                rasterLayerData.RasterTexture = new ClipImportedRasterImageData(data);
                        }
                        if (maskOffscreenID is not null)
                        {
                            var data = GetExtraDataFromOffscreenID(maskOffscreenID.Value);
                            if (data is not null)
                                rasterLayerData.LayerMask = new()
                                {
                                    LayerMaskDisabled = false,
                                    MaskTexture = new ClipImportedRasterImageData(data)
                                };
                        }
                    }
                    nextLayerIndex = cLayer.LayerNextIndex;
                }

                long? GetOffscreenID(long mipMapID)
                {
                    var mipMap = mipmap.FirstOrDefault(m => m.MainId == mipMapID);
                    if (mipMap is null) { return null; }
                    var mipMapInfo = mipmapInfo.FirstOrDefault(m => m.MainId == mipMap.BaseMipmapInfo);
                    if (mipMapInfo is null) { return null; }
                    var offscreenID = mipMapInfo.Offscreen;
                    return offscreenID;
                }

                ExtraData GetExtraDataFromOffscreenID(long v)
                {
                    var offscreenRecord = offscreen.First(o => o.MainId == v);
                    var externalId = Encoding.UTF8.GetString(offscreenRecord.BlockData);
                    var data = extraData.FirstOrDefault(d => d.ExternalID == externalId);
                    return data;
                }
            }
        }

        private static void WriteData(AbstractLayerData ld, ILayerRecord l)
        {
            ld.LayerName = l.LayerName;
            // ld.TransparencyProtected = l.LayerLock;
            ld.Visible = l.LayerVisibility is not 0;
            ld.Opacity = l.LayerOpacity / 256f;
            ld.Clipping = l.LayerClip is not 0;
            ld.BlendTypeKey = ToBlendTypeKey((ClipBlending)l.LayerComposite);
        }

        public static string ToBlendTypeKey(ClipBlending clipBlending)
        {
            switch (clipBlending)
            {
                default:
                case ClipBlending.通常: return "Clip/Normal";

                case ClipBlending.比較_暗: return "Clip/DarkenOnly";
                case ClipBlending.乗算: return "Clip/Mul";
                case ClipBlending.焼きこみカラー: return "Clip/ColorBurn";
                case ClipBlending.焼きこみ_リニア: return "Clip/LinearBurn";
                case ClipBlending.減算: return "Clip/Subtract";
                case ClipBlending.比較_明: return "Clip/LightenOnly";
                case ClipBlending.スクリーン: return "Clip/Screen";
                case ClipBlending.覆い焼きカラー: return "Clip/ColorDodge";
                case ClipBlending.覆い焼き_発光: return "Clip/ColorDodgeGlow";
                case ClipBlending.加算: return "Clip/Addition";
                case ClipBlending.加算_発光: return "Clip/AdditionGlow";
                case ClipBlending.オーバーレイ: return "Clip/Overlay";
                case ClipBlending.ソフトライト: return "Clip/SoftLight";
                case ClipBlending.ハードライト: return "Clip/HardLight";
                case ClipBlending.差の絶対値: return "Clip/Difference";
                case ClipBlending.ビビッドライト: return "Clip/VividLight";
                case ClipBlending.リニアライト: return "Clip/LinearLight";
                case ClipBlending.ピンライト: return "Clip/PinLight";
                case ClipBlending.ハードミックス: return "Clip/HardMix";
                case ClipBlending.除外: return "Clip/Exclusion";
                case ClipBlending.カラー比較_暗: return "Clip/DarkenColorOnly";
                case ClipBlending.カラー比較_明: return "Clip/LightenColorOnly";
                case ClipBlending.除算: return "Clip/Divide";
                case ClipBlending.色相: return "Clip/Hue";
                case ClipBlending.彩度: return "Clip/Saturation";
                case ClipBlending.カラー: return "Clip/Color";
                case ClipBlending.輝度: return "Clip/Luminosity";
            }
        }
    }

    public class ClipHighLevelData
    {
        public List<ClipCanvasData> Canvases;

        public ClipHighLevelData(List<ClipCanvasData> canvases)
        {
            Canvases = canvases;
        }
    }
    public class ClipCanvasData
    {
        public int Width;
        public int Height;
        public List<AbstractLayerData> RootLayers;

        public ClipCanvasData(List<AbstractLayerData> rootLayers)
        {
            RootLayers = rootLayers;
        }
    }

    public enum ClipBlending
    {
        // めんどくさかったのでここでは日本語を使用します。
        通常 = 0,
        比較_暗 = 1,
        乗算 = 2,
        焼きこみカラー = 3,
        焼きこみ_リニア = 4,
        減算 = 5,
        比較_明 = 7,
        スクリーン = 8,
        覆い焼きカラー = 9,
        覆い焼き_発光 = 10,
        加算 = 11,
        加算_発光 = 12,
        オーバーレイ = 14,
        ソフトライト = 15,
        ハードライト = 16,
        差の絶対値 = 21,
        ビビッドライト = 17,
        リニアライト = 18,
        ピンライト = 19,
        ハードミックス = 20,
        除外 = 22,
        カラー比較_暗 = 6,
        カラー比較_明 = 13,
        除算 = 36,
        色相 = 23,
        彩度 = 24,
        カラー = 25,
        輝度 = 26,
        通過 = 30,
    }

}
