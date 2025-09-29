#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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

            var canvasData = new List<ClipCanvasData>();

            foreach (var c in canvas)
            {
                var layersData = new List<AbstractLayerData>();
                GenerateLayerData(layersData, layers, GetLayer(layers, c.CanvasRootFolder));
                canvasData.Add(new(layersData) { Width = (int)c.CanvasWidth, Height = (int)c.CanvasHeight });
            }

            return new ClipHighLevelData(canvasData);

            static ILayerRecord GetLayer(List<ILayerRecord> layers, long id)
            {
                return layers.First(l => l.MainId == id);
            }
            static void GenerateLayerData(List<AbstractLayerData> layersData, List<ILayerRecord> layers, ILayerRecord layer)
            {
                var nextLayerIndex = layer.LayerFirstChildIndex;
                while (nextLayerIndex is not 0)
                {
                    var cLayer = GetLayer(layers, nextLayerIndex);
                    if (cLayer.LayerFolder is not 0)
                    {
                        var lf = new LayerFolderData() { Layers = new() };
                        layersData.Add(lf);

                        WriteData(lf, cLayer);
                        GenerateLayerData(lf.Layers, layers, cLayer);
                    }
                    else
                    {
                        var l = new EmptyOrUnsupported();
                        layersData.Add(l);
                        WriteData(l, cLayer);
                    }
                    nextLayerIndex = cLayer.LayerNextIndex;
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
