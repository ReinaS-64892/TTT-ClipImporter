#nullable enable
using System;
using net.rs64.TexTransCore;
using net.rs64.TexTransTool.MultiLayerImage;

namespace net.rs64.TexTransTool.ClipImporter
{
    public class ClipBinaryHolder : ITTImportedCanvasSource
    {
        public byte[] clipBinary;

        public ClipBinaryHolder(byte[] clipBinary)
        {
            this.clipBinary = clipBinary;
        }
    }
}
