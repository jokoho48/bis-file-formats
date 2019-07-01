#region

using System;
using System.Diagnostics;
using System.IO;
using BIS.Core.Compression;

#endregion

namespace BIS.Core.Streams
{
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream stream) : base(stream)
        {
            UseCompressionFlag = false;
        }

        public bool UseCompressionFlag { get; set; }
        public bool UseLZOCompression { get; set; }

        //used to store file format versions (e.g. ODOL v60)
        public int Version { get; set; }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public uint ReadUInt24()
        {
            return (uint) (ReadByte() + (ReadByte() << 8) + (ReadByte() << 16));
        }

        public string ReadAscii(int count)
        {
            string str = "";
            for (int index = 0; index < count; ++index)
                str = str + (char) ReadByte();
            return str;
        }

        public string ReadAscii()
        {
            ushort n = ReadUInt16();
            return ReadAscii(n);
        }

        public string ReadAsciiz()
        {
            string str = "";
            char ch;
            while ((ch = (char) ReadByte()) != 0)
                str = str + ch;
            return str;
        }

        public int ReadCompactInteger()
        {
            int val = ReadByte();
            if ((val & 0x80) == 0) return val;
            int extra = ReadByte();
            val += (extra - 1) * 0x80;

            return val;
        }

        public byte[] ReadCompressed(uint expectedSize)
        {
            if (expectedSize == 0)
            {
                return new byte[0];
            }

            return UseLZOCompression ? ReadLZO(expectedSize) : ReadLZSS(expectedSize);
        }

        public byte[] ReadLZO(uint expectedSize)
        {
            bool isCompressed = expectedSize >= 1024;
            if (UseCompressionFlag)
            {
                isCompressed = ReadBoolean();
            }

            if (!isCompressed)
            {
                return ReadBytes((int) expectedSize);
            }

            return LZO.ReadLZO(BaseStream, expectedSize);
        }

        public byte[] ReadLZSS(uint expectedSize, bool inPAA = false)
        {
            if (expectedSize < 1024 && !inPAA) //data is always compressed in PAAs
            {
                return ReadBytes((int) expectedSize);
            }

            LZSS.ReadLZSS(BaseStream, out byte[] dst, expectedSize,
                inPAA); //PAAs calculate checksums with signed byte values
            return dst;
        }

        public byte[] ReadCompressedIndices(int bytesToRead, uint expectedSize)
        {
            byte[] result = new byte[expectedSize];
            int outputI = 0;
            for (int i = 0; i < bytesToRead; i++)
            {
                byte b = ReadByte();
                if ((b & 128) != 0)
                {
                    byte n = (byte) (b - 127);
                    byte value = ReadByte();
                    for (int j = 0; j < n; j++)
                        result[outputI++] = value;
                }
                else
                {
                    for (int j = 0; j < b + 1; j++)
                        result[outputI++] = ReadByte();
                }
            }

            Debug.Assert(outputI == expectedSize);

            return result;
        }

        #region SimpleArray

        public T[] ReadArrayBase<T>(Func<BinaryReaderEx, T> readElement, int size)
        {
            T[] array = new T[size];
            for (int i = 0; i < size; i++)
                array[i] = readElement(this);

            return array;
        }

        public T[] ReadArray<T>(Func<BinaryReaderEx, T> readElement)
        {
            return ReadArrayBase(readElement, ReadInt32());
        }

        public float[] ReadFloatArray()
        {
            return ReadArray(i => i.ReadSingle());
        }

        public int[] ReadIntArray()
        {
            return ReadArray(i => i.ReadInt32());
        }

        public string[] ReadStringArray()
        {
            return ReadArray(i => i.ReadAsciiz());
        }

        #endregion

        #region CompressedArray

        public T[] ReadCompressedArray<T>(Func<BinaryReaderEx, T> readElement, int elemSize)
        {
            int nElements = ReadInt32();
            uint expectedDataSize = (uint) (nElements * elemSize);
            BinaryReaderEx stream = new BinaryReaderEx(new MemoryStream(ReadCompressed(expectedDataSize)));

            return stream.ReadArrayBase(readElement, nElements);
        }

        public short[] ReadCompressedShortArray()
        {
            return ReadCompressedArray(i => i.ReadInt16(), 2);
        }

        public int[] ReadCompressedIntArray()
        {
            return ReadCompressedArray(i => i.ReadInt32(), 4);
        }

        public float[] ReadCompressedFloatArray()
        {
            return ReadCompressedArray(i => i.ReadSingle(), 4);
        }

        #endregion

        #region CondensedArray

        public T[] ReadCondensedArray<T>(Func<BinaryReaderEx, T> readElement, int sizeOfT)
        {
            int size = ReadInt32();
            T[] result = new T[size];
            bool defaultFill = ReadBoolean();
            if (defaultFill)
            {
                T defaultValue = readElement(this);
                for (int i = 0; i < size; i++)
                    result[i] = defaultValue;

                return result;
            }

            uint expectedDataSize = (uint) (size * sizeOfT);
            using (BinaryReaderEx stream = new BinaryReaderEx(new MemoryStream(ReadCompressed(expectedDataSize))))
            {
                result = stream.ReadArrayBase(readElement, size);
            }

            return result;
        }

        public int[] ReadCondensedIntArray()
        {
            return ReadCondensedArray(i => i.ReadInt32(), 4);
        }

        #endregion
    }
}