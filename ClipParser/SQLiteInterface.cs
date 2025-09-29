using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using net.rs64.ParserUtility;

namespace net.rs64.TexTransTool.ClipParser
{
    //なぜこんなものがあるのか ... ? unity-sqlite-net が no engine reference でうごかなかったから。

    public interface ISQLiteDBConnector
    {
        ISQLiteDBConnection Deserialize(byte[] bdBytes);
    }
    public interface ISQLiteDBConnection : IDisposable
    {
        List<ICanvasRecord> QueryCanvas();
        List<ILayerRecord> QueryLayer();
        List<IOffscreenRecord> QueryOffscreen();
        List<IMipmapRecord> QueryMipMap();
        List<IMipmapInfoRecord> QueryMipMapInfo();

    }
    public interface ICanvasRecord
    {
        long PwId { get; set; }
        long MainId { get; set; }
        long CanvasUnit { get; set; }
        double CanvasWidth { get; set; }
        double CanvasHeight { get; set; }
        double CanvasResolution { get; set; }
        long CanvasChannelBytes { get; set; }
        long CanvasDefaultChannelOrder { get; set; }
        long CanvasRootFolder { get; set; }
        long CanvasCurrentLayer { get; set; }
        long CanvasDoSimulateColor { get; set; }
        byte[] CanvasSrcProfileName { get; set; }
        byte[] CanvasSrcProfile { get; set; }
        byte[] CanvasDstProfileName { get; set; }
        byte[] CanvasDstProfile { get; set; }
        byte[] CanvasRenderingIntent { get; set; }
        byte[] CanvasUseLibraryType { get; set; }
        byte[] CanvasSimulateSrcProfileName { get; set; }
        byte[] CanvasSimulateSrcProfile { get; set; }
        byte[] CanvasSimulateDstProfileName { get; set; }
        byte[] CanvasSimulateDstProfile { get; set; }
        byte[] CanvasSimulateRenderingIntent { get; set; }
        byte[] CanvasSimulateUseLibraryType { get; set; }
        byte[] CanvasUseColorAdjustment { get; set; }
        byte[] CanvasColorAdjustmentToneCurve { get; set; }
        byte[] CanvasColorAdjustmentLevel { get; set; }
        long CanvasDefaultColorTypeIndex { get; set; }
        long CanvasDefaultColorBlackChecked { get; set; }
        long CanvasDefaultColorWhiteChecked { get; set; }
        double CanvasDefaultToneLine { get; set; }
        long CanvasDoublePage { get; set; }
        long CanvasRenderMipmapForceSaved { get; set; }
        long Canvas3DModelDataLoaderIndex { get; set; }
    }
    public interface ILayerRecord
    {
        long PwId { get; set; }
        long MainId { get; set; }
        long CanvasId { get; set; }
        string LayerName { get; set; }
        long LayerType { get; set; }
        long LayerLock { get; set; }
        long LayerClip { get; set; }
        long LayerMasking { get; set; }
        long LayerOffsetX { get; set; }
        long LayerOffsetY { get; set; }
        long LayerRenderOffscrOffsetX { get; set; }
        long LayerRenderOffscrOffsetY { get; set; }
        long LayerMaskOffsetX { get; set; }
        long LayerMaskOffsetY { get; set; }
        long LayerMaskOffscrOffsetX { get; set; }
        long LayerMaskOffscrOffsetY { get; set; }
        long LayerOpacity { get; set; }
        long LayerComposite { get; set; }
        long LayerUsePaletteColor { get; set; }
        long LayerNoticeablePaletteColor { get; set; }
        long LayerPaletteRed { get; set; }
        long LayerPaletteGreen { get; set; }
        long LayerPaletteBlue { get; set; }
        long LayerFolder { get; set; }
        long LayerVisibility { get; set; }
        long LayerSelect { get; set; }
        long LayerNextIndex { get; set; }
        long LayerFirstChildIndex { get; set; }
        string LayerUuid { get; set; }
        long LayerRenderMipmap { get; set; }
        long LayerLayerMaskMipmap { get; set; }
        long LayerRenderThumbnail { get; set; }
        long LayerLayerMaskThumbnail { get; set; }
        byte[] UsePreviewColorType { get; set; }
        byte[] UsePreviewMaskColorType { get; set; }
        byte[] EffectRangeType { get; set; }
        byte[] DraftLayer { get; set; }
        byte[] FilterLayerV132 { get; set; }
        long DrawColorMainRed { get; set; }
        long DrawColorMainGreen { get; set; }
        long DrawColorMainBlue { get; set; }
        long DrawColorEnable { get; set; }
        long DrawToRenderOffscreenType { get; set; }
        long SpecialRenderType { get; set; }
        long DrawToRenderMipmapType { get; set; }
        long MoveOffsetAndExpandType { get; set; }
        long FixOffsetAndExpandType { get; set; }
        long RenderBoundForLayerMoveType { get; set; }
        long SetRenderThumbnailInfoType { get; set; }
        long DrawRenderThumbnailType { get; set; }
        byte[] MonochromeFillInfo { get; set; }
        long LayerColorTypeIndex { get; set; }
        long LayerColorTypeBlackChecked { get; set; }
        long LayerColorTypeWhiteChecked { get; set; }
    }
    public interface IOffscreenRecord
    {
        long PwId { get; set; }
        long MainId { get; set; }
        long CanvasId { get; set; }
        long LayerId { get; set; }
        byte[] Attribute { get; set; }
        byte[] BlockData { get; set; }
    }

    public interface IMipmapRecord
    {
        long PwId { get; set; }
        long MainId { get; set; }
        long CanvasId { get; set; }
        long LayerId { get; set; }
        long MipmapCount { get; set; }
        long BaseMipmapInfo { get; set; }
    }
    public interface IMipmapInfoRecord
    {
        long PwId { get; set; }
        long MainId { get; set; }
        long CanvasId { get; set; }
        long LayerId { get; set; }
        double ThisScale { get; set; }
        long Offscreen { get; set; }
        long NextIndex { get; set; }
    }

}
