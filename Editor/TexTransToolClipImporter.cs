#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.rs64.TexTransTool.ClipParser;
using net.rs64.TexTransTool.MultiLayerImage;
using net.rs64.TexTransTool.MultiLayerImage.Importer;
using net.rs64.TexTransTool.MultiLayerImage.LayerData;
using SQLite;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace net.rs64.TexTransTool.ClipImporter
{
    /*
    これらの情報が変わると、インポーター再設定だから変わらないように気を付けないといけない。
    AssemblyName: net.rs64.ttt-clip-importer.editor
    FullName: net.rs64.TexTransTool.ClipImporter.TexTransToolClipImporter
    */
    [ScriptedImporter(0, new string[] { "clip" }, new string[] { "clip" }, AllowCaching = true)]
    public class TexTransToolClipImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var clipBytes = File.ReadAllBytes(ctx.assetPath);

            var lowLevelData = ClipLowLevelParser.Parse(clipBytes);
            var highLevelData = ClipHighLevelParser.Parse(clipBytes, lowLevelData, new UnitySQLiteWrapper());

            if (highLevelData.Canvases.Count is 1)
            {
                var canvas = highLevelData.Canvases.First();

                var prefabName = Path.GetFileName(ctx.assetPath) + "-Canvas";
                var rootCanvas = new GameObject(prefabName);
                var multiLayerImageCanvas = rootCanvas.AddComponent<MultiLayerImageCanvas>();

                ctx.AddObjectToAsset("RootCanvas", rootCanvas);
                ctx.SetMainObject(rootCanvas);

                var canvasDescription = ScriptableObject.CreateInstance<ClipImportedCanvasDescription>();
                canvasDescription.Width = canvas.Width;
                canvasDescription.Height = canvas.Height;
                canvasDescription.name = "CanvasDescription";
                ctx.AddObjectToAsset(canvasDescription.name, canvasDescription);
                multiLayerImageCanvas.tttImportedCanvasDescription = canvasDescription;

                var mliImporter = new MultiLayerImageImporter(multiLayerImageCanvas, canvasDescription, ctx, CreateClipImportedImage);
                mliImporter.AddLayers(canvas.RootLayers);
                mliImporter.SaveSubAsset();
            }
        }

        private TTTImportedImage? CreateClipImportedImage(ImportRasterImageData importRasterImage)
        {
            switch (importRasterImage)
            {
                default: return null;
                case ClipImportedRasterImageData clipImportedRasterImageData:
                    {
                        var importedImage = ScriptableObject.CreateInstance<ClipImportedRasterImage>();
                        importedImage.ExtraData = clipImportedRasterImageData.ExtraData;
                        return importedImage;
                    }
            }
        }

        public class UnitySQLiteWrapper : ISQLiteDBConnector
        {
            public ISQLiteDBConnection Deserialize(byte[] bdBytes)
            {
                var sqlDB = new SQLiteConnection("").Deserialize(bdBytes, null, SQLite3.DeserializeFlags.ReadOnly);

                return new UnitySQLiteConnectionWrapper(sqlDB);
            }

            private class UnitySQLiteConnectionWrapper : ISQLiteDBConnection
            {
                private SQLiteConnection sqlDB;

                public UnitySQLiteConnectionWrapper(SQLiteConnection sqlDB) { this.sqlDB = sqlDB; }

                public List<ICanvasRecord> QueryCanvas() { return sqlDB.Query<CanvasRecord>("SELECT * from Canvas;").OfType<ICanvasRecord>().ToList(); }
                public List<ILayerRecord> QueryLayer() { return sqlDB.Query<LayerRecord>("SELECT * from Layer;").OfType<ILayerRecord>().ToList(); }
                public List<IOffscreenRecord> QueryOffscreen() { return sqlDB.Query<OffscreenRecord>("SELECT * from Offscreen;").OfType<IOffscreenRecord>().ToList(); }
                public List<IMipmapRecord> QueryMipMap() { return sqlDB.Query<MipmapRecord>("SELECT * from Mipmap;").OfType<IMipmapRecord>().ToList(); }
                public List<IMipmapInfoRecord> QueryMipMapInfo() { return sqlDB.Query<MipmapInfoRecord>("SELECT * from MipmapInfo;").OfType<IMipmapInfoRecord>().ToList(); }

                public void Dispose() { sqlDB.Dispose(); }

            }
        }
    }
}
