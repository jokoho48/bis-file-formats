#region

using System;
using System.Runtime.InteropServices;
using BIS.Core.Compression;

#endregion

namespace BIS.Core.Streams
{
    public class BufferReader
    {
        private readonly byte[] buffer;
        private int pos;

        public BufferReader(byte[] buffer)
        {
            this.buffer = buffer;
            pos = 0;
        }

        public BufferReader(ReadOnlySpan<byte> bufferSpan)
        {
            buffer = bufferSpan.ToArray();
            pos = 0;
        }

        public bool UseLZOCompression { get; set; }
        public bool UseCompressionFlag { get; set; }
        public int Version { get; set; }

        public ReadOnlySpan<byte> ReadSpan(int len)
        {
            Span<byte> span = buffer.AsSpan().Slice(pos, len);
            pos += len;
            return span;
        }

        public T Read<T>() where T : struct
        {
            int elemSize = Marshal.SizeOf(typeof(T));
            T result = MemoryMarshal.Read<T>(buffer.AsSpan(pos, elemSize));
            pos += elemSize;
            return result;
        }

        public T[] ReadCompressedArray<T>() where T : struct
        {
            int nElements = BitConverter.ToInt32(buffer, pos);
            pos += 4;
            int elemSize = Marshal.SizeOf(typeof(T));
            int expectedDataSize = nElements * elemSize;
            ReadOnlySpan<byte> decompressedSpan = ReadCompressed(expectedDataSize);

            return MemoryMarshal.Cast<byte, T>(decompressedSpan).ToArray();
        }

        public CondensedArray<T> ReadCondensedArray<T>() where T : struct
        {
            int nElements = BitConverter.ToInt32(buffer, pos);
            pos += 4;
            bool defaultFill = buffer[pos++] != 0;
            int elemSize = Marshal.SizeOf(typeof(T));
            if (defaultFill)
            {
                T defaultValue = MemoryMarshal.Read<T>(buffer.AsSpan(pos, elemSize));
                pos += elemSize;

                return new CondensedArray<T>(nElements, defaultValue);
            }

            int expectedDataSize = nElements * elemSize;
            ReadOnlySpan<byte> decompressedSpan = ReadCompressed(expectedDataSize);
            T[] data = MemoryMarshal.Cast<byte, T>(decompressedSpan).ToArray();
            return new CondensedArray<T>(data);
        }

        #region Compression

        public ReadOnlySpan<byte> ReadCompressed(int expectedSize)
        {
            if (expectedSize == 0)
            {
                return new byte[0];
            }

            if (UseLZOCompression) return ReadLZO(expectedSize);

            return ReadLZSS(expectedSize);
        }

        public ReadOnlySpan<byte> ReadLZO(int expectedSize)
        {
            bool isCompressed = expectedSize >= 1024;
            if (UseCompressionFlag)
            {
                isCompressed = buffer[pos++] != 0;
            }

            if (!isCompressed)
            {
                return ReadSpan(expectedSize);
            }

            byte[] output = LZO.Decompress(buffer.AsSpan(pos), expectedSize, out int bytesRead);
            pos += bytesRead;
            return output;
        }

        public ReadOnlySpan<byte> ReadLZSS(int expectedSize, bool inPAA = false)
        {
            if (expectedSize < 1024 && !inPAA) //data is always compressed in PAAs
            {
                return ReadSpan(expectedSize);
            }

            byte[] dst = LZSS.ReadLZSS(buffer.AsSpan(pos), expectedSize, inPAA, out int bytesRead);
            pos += bytesRead;
            return dst;
        }

        #endregion
    }
}