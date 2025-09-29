using SQLite;

namespace net.rs64.TexTransTool.ClipParser
{
    [Table("Canvas")]
    internal class CanvasRecord : ICanvasRecord
    {
        [Column("_PW_ID")] public long PwId { get; set; }
        [Column("MainId")] public long MainId { get; set; }
        [Column("CanvasUnit")] public long CanvasUnit { get; set; }
        [Column("CanvasWidth")] public double CanvasWidth { get; set; }
        [Column("CanvasHeight")] public double CanvasHeight { get; set; }
        [Column("CanvasResolution")] public double CanvasResolution { get; set; }
        [Column("CanvasChannelBytes")] public long CanvasChannelBytes { get; set; }
        [Column("CanvasDefaultChannelOrder")] public long CanvasDefaultChannelOrder { get; set; }
        [Column("CanvasRootFolder")] public long CanvasRootFolder { get; set; }
        [Column("CanvasCurrentLayer")] public long CanvasCurrentLayer { get; set; }
        [Column("CanvasDoSimulateColor")] public long CanvasDoSimulateColor { get; set; }
        [Column("CanvasSrcProfileName")] public byte[] CanvasSrcProfileName { get; set; }
        [Column("CanvasSrcProfile")] public byte[] CanvasSrcProfile { get; set; }
        [Column("CanvasDstProfileName")] public byte[] CanvasDstProfileName { get; set; }
        [Column("CanvasDstProfile")] public byte[] CanvasDstProfile { get; set; }
        [Column("CanvasRenderingIntent")] public byte[] CanvasRenderingIntent { get; set; }
        [Column("CanvasUseLibraryType")] public byte[] CanvasUseLibraryType { get; set; }
        [Column("CanvasSimulateSrcProfileName")] public byte[] CanvasSimulateSrcProfileName { get; set; }
        [Column("CanvasSimulateSrcProfile")] public byte[] CanvasSimulateSrcProfile { get; set; }
        [Column("CanvasSimulateDstProfileName")] public byte[] CanvasSimulateDstProfileName { get; set; }
        [Column("CanvasSimulateDstProfile")] public byte[] CanvasSimulateDstProfile { get; set; }
        [Column("CanvasSimulateRenderingIntent")] public byte[] CanvasSimulateRenderingIntent { get; set; }
        [Column("CanvasSimulateUseLibraryType")] public byte[] CanvasSimulateUseLibraryType { get; set; }
        [Column("CanvasUseColorAdjustment")] public byte[] CanvasUseColorAdjustment { get; set; }
        [Column("CanvasColorAdjustmentToneCurve")] public byte[] CanvasColorAdjustmentToneCurve { get; set; }
        [Column("CanvasColorAdjustmentLevel")] public byte[] CanvasColorAdjustmentLevel { get; set; }
        [Column("CanvasDefaultColorTypeIndex")] public long CanvasDefaultColorTypeIndex { get; set; }
        [Column("CanvasDefaultColorBlackChecked")] public long CanvasDefaultColorBlackChecked { get; set; }
        [Column("CanvasDefaultColorWhiteChecked")] public long CanvasDefaultColorWhiteChecked { get; set; }
        [Column("CanvasDefaultToneLine")] public double CanvasDefaultToneLine { get; set; }
        [Column("CanvasDoublePage")] public long CanvasDoublePage { get; set; }
        [Column("CanvasRenderMipmapForceSaved")] public long CanvasRenderMipmapForceSaved { get; set; }
        [Column("Canvas3DModelDataLoaderIndex")] public long Canvas3DModelDataLoaderIndex { get; set; }
    }


    [Table("Layer")]
    internal class LayerRecord : ILayerRecord
    {
        [Column("_PW_ID")] public long PwId { get; set; }
        [Column("MainId")] public long MainId { get; set; }
        [Column("CanvasId")] public long CanvasId { get; set; }
        [Column("LayerName")] public string LayerName { get; set; }
        [Column("LayerType")] public long LayerType { get; set; }
        [Column("LayerLock")] public long LayerLock { get; set; }
        [Column("LayerClip")] public long LayerClip { get; set; }
        [Column("LayerMasking")] public long LayerMasking { get; set; }
        [Column("LayerOffsetX")] public long LayerOffsetX { get; set; }
        [Column("LayerOffsetY")] public long LayerOffsetY { get; set; }
        [Column("LayerRenderOffscrOffsetX")] public long LayerRenderOffscrOffsetX { get; set; }
        [Column("LayerRenderOffscrOffsetY")] public long LayerRenderOffscrOffsetY { get; set; }
        [Column("LayerMaskOffsetX")] public long LayerMaskOffsetX { get; set; }
        [Column("LayerMaskOffsetY")] public long LayerMaskOffsetY { get; set; }
        [Column("LayerMaskOffscrOffsetX")] public long LayerMaskOffscrOffsetX { get; set; }
        [Column("LayerMaskOffscrOffsetY")] public long LayerMaskOffscrOffsetY { get; set; }
        [Column("LayerOpacity")] public long LayerOpacity { get; set; }
        [Column("LayerComposite")] public long LayerComposite { get; set; }
        [Column("LayerUsePaletteColor")] public long LayerUsePaletteColor { get; set; }
        [Column("LayerNoticeablePaletteColor")] public long LayerNoticeablePaletteColor { get; set; }
        [Column("LayerPaletteRed")] public long LayerPaletteRed { get; set; }
        [Column("LayerPaletteGreen")] public long LayerPaletteGreen { get; set; }
        [Column("LayerPaletteBlue")] public long LayerPaletteBlue { get; set; }
        [Column("LayerFolder")] public long LayerFolder { get; set; }
        [Column("LayerVisibility")] public long LayerVisibility { get; set; }
        [Column("LayerSelect")] public long LayerSelect { get; set; }
        [Column("LayerNextIndex")] public long LayerNextIndex { get; set; }
        [Column("LayerFirstChildIndex")] public long LayerFirstChildIndex { get; set; }
        [Column("LayerUuid")] public string LayerUuid { get; set; }
        [Column("LayerRenderMipmap")] public long LayerRenderMipmap { get; set; }
        [Column("LayerLayerMaskMipmap")] public long LayerLayerMaskMipmap { get; set; }
        [Column("LayerRenderThumbnail")] public long LayerRenderThumbnail { get; set; }
        [Column("LayerLayerMaskThumbnail")] public long LayerLayerMaskThumbnail { get; set; }
        [Column("UsePreviewColorType")] public byte[] UsePreviewColorType { get; set; }
        [Column("UsePreviewMaskColorType")] public byte[] UsePreviewMaskColorType { get; set; }
        [Column("EffectRangeType")] public byte[] EffectRangeType { get; set; }
        [Column("DraftLayer")] public byte[] DraftLayer { get; set; }
        [Column("FilterLayerV132")] public byte[] FilterLayerV132 { get; set; }
        [Column("DrawColorMainRed")] public long DrawColorMainRed { get; set; }
        [Column("DrawColorMainGreen")] public long DrawColorMainGreen { get; set; }
        [Column("DrawColorMainBlue")] public long DrawColorMainBlue { get; set; }
        [Column("DrawColorEnable")] public long DrawColorEnable { get; set; }
        [Column("DrawToRenderOffscreenType")] public long DrawToRenderOffscreenType { get; set; }
        [Column("SpecialRenderType")] public long SpecialRenderType { get; set; }
        [Column("DrawToRenderMipmapType")] public long DrawToRenderMipmapType { get; set; }
        [Column("MoveOffsetAndExpandType")] public long MoveOffsetAndExpandType { get; set; }
        [Column("FixOffsetAndExpandType")] public long FixOffsetAndExpandType { get; set; }
        [Column("RenderBoundForLayerMoveType")] public long RenderBoundForLayerMoveType { get; set; }
        [Column("SetRenderThumbnailInfoType")] public long SetRenderThumbnailInfoType { get; set; }
        [Column("DrawRenderThumbnailType")] public long DrawRenderThumbnailType { get; set; }
        [Column("MonochromeFillInfo")] public byte[] MonochromeFillInfo { get; set; }
        [Column("LayerColorTypeIndex")] public long LayerColorTypeIndex { get; set; }
        [Column("LayerColorTypeBlackChecked")] public long LayerColorTypeBlackChecked { get; set; }
        [Column("LayerColorTypeWhiteChecked")] public long LayerColorTypeWhiteChecked { get; set; }
    }

    [Table("Offscreen")]
    internal class OffscreenRecord : IOffscreenRecord
    {
        [Column("_PW_ID")] public long PwId { get; set; }
        [Column("MainId")] public long MainId { get; set; }
        [Column("CanvasId")] public long CanvasId { get; set; }
        [Column("LayerId")] public long LayerId { get; set; }
        [Column("Attribute")] public byte[] Attribute { get; set; }
        [Column("BlockData")] public byte[] BlockData { get; set; }
    }

    [Table("Mipmap")]
    internal class MipmapRecord : IMipmapRecord
    {
        [Column("_PW_ID")] public long PwId { get; set; }
        [Column("MainId")] public long MainId { get; set; }
        [Column("CanvasId")] public long CanvasId { get; set; }
        [Column("LayerId")] public long LayerId { get; set; }
        [Column("MipmapCount")] public long MipmapCount { get; set; }
        [Column("BaseMipmapInfo")] public long BaseMipmapInfo { get; set; }

    }


    [Table("MipmapInfo")]
    internal class MipmapInfoRecord : IMipmapInfoRecord
    {
        [Column("_PW_ID")] public long PwId { get; set; }
        [Column("MainId")] public long MainId { get; set; }
        [Column("CanvasId")] public long CanvasId { get; set; }
        [Column("LayerId")] public long LayerId { get; set; }
        [Column("ThisScale")] public double ThisScale { get; set; }
        [Column("Offscreen")] public long Offscreen { get; set; }
        [Column("NextIndex")] public long NextIndex { get; set; }

    }
}
