#nullable enable
using System;
using System.IO;
using net.rs64.TexTransCore;
using net.rs64.TexTransTool.MultiLayerImage;

namespace net.rs64.TexTransTool.ClipImporter
{
    public class ClipImportedCanvasDescription : TTTImportedCanvasDescription
    {
        public override TexTransCoreTextureFormat ImportedImageFormat => TexTransCoreTextureFormat.Byte;
        public override ITTImportedCanvasSource LoadCanvasSource(string path) { return new ClipBinaryHolder(File.ReadAllBytes(path)); }
    }
}
