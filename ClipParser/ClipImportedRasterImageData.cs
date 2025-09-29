#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using net.rs64.ParserUtility;
using net.rs64.TexTransTool.MultiLayerImage.LayerData;

namespace net.rs64.TexTransTool.ClipParser
{
    public abstract class AbstractClipImportedRasterImageData : ImportRasterImageData
    {
        public ExtraData ExtraData;

        protected AbstractClipImportedRasterImageData(ExtraData extraData)
        {
            ExtraData = extraData;
        }
    }
    public class ClipImportedRasterImageData : AbstractClipImportedRasterImageData
    {
        public ClipImportedRasterImageData(ExtraData extraData) : base(extraData)
        {
        }
    }
    public class ClipImportedMaskImageData : AbstractClipImportedRasterImageData
    {
        public int DefaultValue;

        public ClipImportedMaskImageData(ExtraData extraData) : base(extraData)
        {
        }
    }
}
