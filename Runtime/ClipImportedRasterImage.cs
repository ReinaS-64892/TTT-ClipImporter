#nullable enable
using System;
using System.Linq;
using net.rs64.TexTransTool.ClipParser;
using net.rs64.TexTransTool.MultiLayerImage;

namespace net.rs64.TexTransTool.ClipImporter
{
    public class ClipImportedRasterImage : TTTImportedImage
    {
        public ClipParser.ExtraData ExtraData;

        protected override void LoadImage(ITTImportedCanvasSource importSource, Span<byte> writeTarget)
        {
            var clipBinary = ((ClipBinaryHolder)importSource).clipBinary;

            var firstData = ExtraData.DataArray.First();
            var blockSize = new Vector2Int((int)firstData.BlockWidth, (int)firstData.BlockHeight);
            var images = ExtraData.DataArray.Select(d => d.LoadColorData(clipBinary));

            // TODO : implement this !!!
        }
    }
}
