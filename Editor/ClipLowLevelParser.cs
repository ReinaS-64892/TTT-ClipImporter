#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;
using UnityEngine;

namespace net.rs64.MultiLayerImageParser.Clip
{
    public static class ClipLowLevelParser
    {
        public static ClipLowLevelData Parse(string path)
        {
            var stream = new SubSpanStream(File.ReadAllBytes(path).AsSpan());

            if (!ParserUtility.Signature(ref stream, new byte[] { 0x43, 0x53, 0x46, 0x43, 0x48, 0x55, 0x4E, 0x4B })) { throw new Exception(); }

            var clipData = new ClipLowLevelData();

            var fileSize = stream.ReadUInt64();
            var nazoTwoFor = stream.ReadUInt64();

            clipData.FileSize = fileSize;

            if (!ParserUtility.Signature(ref stream, new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x48, 0x65, 0x61, 0x64 })) { throw new Exception(); }

            var headerSize = stream.ReadUInt64();
            var header = stream.ReadSubStream((int)headerSize);
            var extraDataList = new List<ExtraData>();
            while (true)
            {
                if (!ParserUtility.Signature(ref stream, new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x45, 0x78, 0x74, 0x61 })) { break; }
                var extraData = new ExtraData();

                var extraDataSize = stream.ReadUInt64();
                extraData.size = extraDataSize;
                var externalIDLength = stream.ReadUInt64();
                var externalID = Encoding.ASCII.GetString(stream.ReadSubStream((int)externalIDLength).Span.ToArray());
                extraData.ExternalID = externalID;
                var extraDataSpan = stream.ReadSubStream((int)(extraDataSize - externalIDLength - 8));

                extraData.DataArray = ParseExtraDataBlock(extraDataSpan);
                extraDataList.Add(extraData);
            }

            clipData.CHNKExtaList = extraDataList;
            stream.Position -= 8;

            if (!ParserUtility.Signature(ref stream, new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x53, 0x51, 0x4C, 0x69 })) { throw new Exception(); }

            var sQLiteSize = stream.ReadUInt64();
            var sQlLiteData = stream.ReadSubStream((int)sQLiteSize);
            clipData.SQLiteData = sQlLiteData.Span.ToArray();

            if (!ParserUtility.Signature(ref stream, new byte[] { 0x43, 0x48, 0x4E, 0x4B, 0x46, 0x6F, 0x6F, 0x74 })) { throw new Exception(); }

            return clipData;
        }

        private static ExtraDataBlock[] ParseExtraDataBlock(SubSpanStream extaDataSpan)
        {
            var dataBlockList = new List<ExtraDataBlock>();
            var spanStream = extaDataSpan;
            var nazo1 = spanStream.ReadUInt64();
            while (spanStream.Position < spanStream.Length)
            {
                var nazo2 = spanStream.ReadUInt32();

                var blockNameSize = spanStream.ReadUInt32();
                if ((spanStream.Length - spanStream.Position) < blockNameSize) { break; }

                var strbyte = spanStream.ReadSubStream((int)(blockNameSize * 2));
                var str = Encoding.BigEndianUnicode.GetString(strbyte.Span.ToArray());
                switch (str)
                {
                    case "BlockDataBeginChunk":
                        {
                            var dataBlock = new ExtraDataBlock();
                            dataBlock.BlockDataIndex = spanStream.ReadUInt32();
                            dataBlock.UnCompressedSize = spanStream.ReadUInt32();
                            dataBlock.BlockWidth = spanStream.ReadUInt32();
                            dataBlock.BlockHeight = spanStream.ReadUInt32();
                            dataBlock.NotEmpty = spanStream.ReadUInt32();

                            if (dataBlock.NotEmpty == 0)
                            {
                                dataBlock.UnCompressedData = new byte[dataBlock.UnCompressedSize];
                                dataBlockList.Add(dataBlock);


                                spanStream.ReadSubStream(38);
                                break;
                            }
                            var blockLen = spanStream.ReadUInt32();
                            var blockDataLen = spanStream.ReadUInt32(false);
                            if ((spanStream.Length - spanStream.Position) < blockDataLen)
                            {
                                dataBlockList.Add(dataBlock);


                                spanStream.ReadSubStream(38);
                                break;
                            }

                            dataBlock.UnCompressedData = ZlibStream.UncompressBuffer(spanStream.ReadSubStream((int)blockDataLen).Span.ToArray());
                            dataBlock.ConvertColorData();

                            spanStream.ReadSubStream(38);
                            dataBlockList.Add(dataBlock);
                            break;
                        }
                    case "BlockStatus":
                    case "BlockCheckSum":
                        {
                            spanStream.ReadSubStream(28);
                            break;
                        }
                }
            }
            return dataBlockList.ToArray();
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

            var blockSpanStream = new SubSpanStream(UnCompressedData.AsSpan());

            var alpStream = blockSpanStream.ReadSubStream(blockLen);
            var bgrStream = blockSpanStream.ReadSubStream(blockLen * pixelSize);

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