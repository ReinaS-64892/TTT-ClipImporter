#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using net.rs64.ParserUtility;
using UnityEngine;

namespace net.rs64.MultiLayerImageParser.Clip
{
    public static class ClipLowLevelParser
    {
        // CSFCHUNK
        static byte[] FileSignature = new byte[] { 0x43, 0x53, 0x46, 0x43, 0x48, 0x55, 0x4E, 0x4B };
        // CHNKHead
        static byte[] HeaderSignature = new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x48, 0x65, 0x61, 0x64 };

        // CHNKExta
        static byte[] ExtraDataSignature = new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x45, 0x78, 0x74, 0x61 };
        public static ClipLowLevelData Parse(string path)
        {
            var stream = new BinarySectionStream(File.ReadAllBytes(path));
            // stream.BigEndian = false;

            if (stream.Signature(FileSignature) is false) { throw new Exception(); }

            var clipData = new ClipLowLevelData();

            var fileSize = stream.ReadUInt64();
            _ = stream.ReadUInt64();// 24 が入っている 不明が固定 Offset という説もある

            clipData.FileSize = fileSize;

            if (stream.Signature(HeaderSignature) is false) { throw new Exception(); }

            var headerSize = stream.ReadUInt64();// ヘッダのサイズと同値 常に 40 らしい
            _ = stream.ReadSubSection((int)headerSize);


            var extraDataList = new List<ExtraData>();
            clipData.CHNKExtaList = extraDataList;
            while (true)
            {
                if (stream.Signature(ExtraDataSignature) is false)
                {
                    stream.Position -= 8;// Signature 分を読み戻す
                    break;
                }

                var extraData = new ExtraData();
                extraDataList.Add(extraData);

                var extraDataSize = extraData.size = stream.ReadUInt64();// その extra data すべてのサイズとなる
                var extraDataStream = stream.ReadSubSection((long)extraDataSize);

                var externalIDLength = extraDataStream.ReadUInt64();// ID それ自体は 40 しか見られていない
                // externalid**** のような感じの ID が入っているようだ
                extraData.ExternalID = extraDataStream.ReadToASCII((long)externalIDLength);
                extraData.DataArray = ParseExtraDataBlock(extraDataStream);
            }


            if (stream.Signature(new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x53, 0x51, 0x4C, 0x69 }) is false) { throw new Exception(); }

            var sQLiteSize = stream.ReadUInt64();
            var sQlLiteData = stream.ReadSubSection((int)sQLiteSize);
            clipData.SQLiteData = sQlLiteData.ReadToArray();

            if (stream.Signature(new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x46, 0x6F, 0x6F, 0x74 }) is false) { throw new Exception(); }

            return clipData;
        }

        private static ExtraDataBlock[] ParseExtraDataBlock(BinarySectionStream extaDataSpan)
        {
            var dataBlockList = new List<ExtraDataBlock>();
            var spanStream = extaDataSpan;
            _ = spanStream.ReadUInt64();//この ExtraData 終わりまでの Length ともなる、 ExtraDataBlock の長さ。取得したところであまり意味はない。
            while (spanStream.Position < spanStream.Length)
            {
                var dataBlockLength = spanStream.ReadUInt32();//一つの data block の長さっぽい
                dataBlockLength -= 4;//この length の 4 byte も長さに含まれてるっぽい

                var dataBlockStream = spanStream.ReadSubSection(dataBlockLength);

                var beginBlockIDCharCount = dataBlockStream.ReadUInt32();
                if ((dataBlockStream.Length - dataBlockStream.Position) < beginBlockIDCharCount) { break; }

                // BigEndian Unicode - UTF16 BE なので 文字数 * 2 となる
                var beginBlockIDString = dataBlockStream.ReadToUTF16BE((int)(beginBlockIDCharCount * 2));
                switch (beginBlockIDString)
                {
                    case "BlockDataBeginChunk":
                        {
                            var dataBlock = new ExtraDataBlock();
                            dataBlock.BlockDataIndex = dataBlockStream.ReadUInt32();
                            dataBlock.UnCompressedSize = dataBlockStream.ReadUInt32();
                            dataBlock.BlockWidth = dataBlockStream.ReadUInt32();
                            dataBlock.BlockHeight = dataBlockStream.ReadUInt32();
                            dataBlock.NotEmpty = dataBlockStream.ReadUInt32();

                            if (dataBlock.NotEmpty == 0)
                            {
                                dataBlock.UnCompressedData = new byte[dataBlock.UnCompressedSize];
                                dataBlockList.Add(dataBlock);

                                var endBlockIDCharCount2 = dataBlockStream.ReadUInt32();
                                dataBlockStream.ReadToUTF16BE(endBlockIDCharCount2 * 2);// BlockDataEndChunk
                                break;
                            }

                            var compressedDataBlockLength = dataBlockStream.ReadUInt32();

                            using (dataBlockStream.ChangeEndianScope(false))
                            {
                                // ここなぜか 4byte LE の zlib stream のサイズがエンコードさている。
                                // そのすぐ直前に 4byte BE で その 4byte LE 含めたサイズがエンコードされているのでそれれを使う
                                _ = dataBlockStream.ReadUInt32();
                            }
                            compressedDataBlockLength -= 4;
                            var compressedDataBlock = dataBlockStream.ReadSubSection((int)compressedDataBlockLength);

                            dataBlock.UnCompressedData = DecompressZlib(compressedDataBlock.ReadToArray());
                            try { dataBlock.ConvertColorData(); }
                            catch
                            {
                                // 例外は一旦 握りつぶします！！！
                            }

                            var endBlockIDCharCount = dataBlockStream.ReadUInt32();
                            dataBlockStream.ReadToUTF16BE(endBlockIDCharCount * 2);// BlockDataEndChunk

                            dataBlockList.Add(dataBlock);
                            break;
                        }
                    case "BlockStatus":
                    case "BlockCheckSum":
                        {
                            // 謎 ... 誰か調べといて
                            dataBlockStream.ReadSubSection(28);
                            break;
                        }
                }
            }
            return dataBlockList.ToArray();
        }

        public static byte[] DecompressZlib(byte[] imageSourceData)
        {

            using (var memStream = new MemoryStream(imageSourceData, 2, imageSourceData.Length - 2))
            using (var outMemStream = new MemoryStream(imageSourceData.Length))
            using (var gzipStream = new DeflateStream(memStream, System.IO.Compression.CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outMemStream);
                return outMemStream.ToArray();
            }

        }
    }
    [Serializable]
    public class ClipLowLevelData
    {
        public ulong FileSize;
        public List<ExtraData> CHNKExtaList;

        [NonSerialized]
        public byte[] SQLiteData;
    }

    [Serializable]
    public class ExtraData
    {
        public ulong size;
        public string ExternalID;
        public ExtraDataBlock[] DataArray;
    }
    [Serializable]
    public class ExtraDataBlock
    {
        public uint BlockDataIndex;
        public uint UnCompressedSize;
        public uint BlockWidth;
        public uint BlockHeight;
        public uint NotEmpty;

        [NonSerialized]
        public byte[] UnCompressedData;
        [NonSerialized]
        public Color32[] ColorData;

        public void ConvertColorData()
        {
            var blockSize = new Vector2Int((int)BlockWidth, (int)BlockHeight);
            var blockLen = blockSize.x * blockSize.y;
            var pixelSize = 4;

            var colors = new Color32[blockLen];

            var blockSpanStream = new BinarySectionStream(UnCompressedData);

            var alpStream = blockSpanStream.ReadSubSection(blockLen);
            var bgrStream = blockSpanStream.ReadSubSection(blockLen * pixelSize);

            for (int i = 0; colors.Length > i; i += 1)
            {
                var b = bgrStream.ReadByte();
                var g = bgrStream.ReadByte();
                var r = bgrStream.ReadByte();
                bgrStream.ReadByte();//謎のパディング

                colors[i] = new Color32(r, g, b, alpStream.ReadByte());

                // if (bgrStream.Position == bgrStream.Length - 1) { Debug.Log("bgrStreamEnd!" + i); break; }
            }
            ColorData = colors;
        }
    }

}
#endif
