#nullable enable
using System;
using System.Linq;
using System.Runtime.InteropServices;
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
            var distentionSize = new Vector2Int(CanvasDescription.Width, CanvasDescription.Height);
            var images = ExtraData.DataArray.Select(d => d.NotEmpty is not 0 ? d.LoadColorData(clipBinary) : new Color32[d.BlockHeight * d.BlockWidth]).ToArray();

            var distention = MemoryMarshal.Cast<byte, Color32>(writeTarget);

            var blockWidthCount = distentionSize.x / blockSize.x;
            var blockHeightCount = distentionSize.y / blockSize.y;

            for (var i = 0; images.Length > i; i += 1)
            {
                var rawBrockImage = images[i].AsSpan();

                var blockPosition = new Vector2Int(i % blockWidthCount, i / blockHeightCount);
                var distentionPosition = new Vector2Int(blockPosition.x * blockSize.x, blockPosition.y * blockSize.y);

                for (var sy = 0; blockSize.y > sy; sy += 1)
                {
                    var sourceOrigin = sy * blockSize.x;
                    var sourceRawLine = rawBrockImage.Slice(sourceOrigin, blockSize.x);

                    var distentionWriteOrigin = distentionPosition.x + ((distentionPosition.y + sy) * distentionSize.x);
                    var distentionLine = distention.Slice(distentionWriteOrigin, blockSize.x);

                    sourceRawLine.CopyTo(distentionLine);
                }
            }

            HeightInvert(distention, distentionSize.x, distentionSize.y);
        }

        public static void HeightInvert<T>(Span<T> target, int width, int height)
        {
            var halfHeight = height / 2;
            for (var y = 0; halfHeight > y; y += 1)
            {
                var flipYByteStart = (height - 1 - y) * width;
                var yByteStart = y * width;

                var u = target.Slice(flipYByteStart, width);
                var d = target.Slice(yByteStart, width);

                SwapSpan(u, d);

                static void SwapSpan(Span<T> l, Span<T> r)
                {
                    for (var i = 0; l.Length > i; i += 1)
                    {
                        (l[i], r[i]) = (r[i], l[i]);
                    }
                }
            }
        }
    }
}
