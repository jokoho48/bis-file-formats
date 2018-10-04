#region

using System;
using System.IO;
using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class MLOD
    {
        public MLOD(string fileName) : this(File.OpenRead(fileName))
        {
        }

        public MLOD(Stream stream)
        {
            Read(new BinaryReaderEx(stream));
        }

        public MLOD(P3DM_LOD[] lods)
        {
            Version = 257;
            Lods = lods;
        }

        public int Version { get; private set; }
        public P3DM_LOD[] Lods { get; private set; }

        private void Read(BinaryReaderEx input)
        {
            if (input.ReadAscii(4) != "MLOD")
                throw new FormatException("MLOD signature expected");

            Version = input.ReadInt32();
            if (Version != 257)
                throw new ArgumentException("Unknown MLOD version");

            Lods = input.ReadArray(inp => new P3DM_LOD(inp));
        }

        private void Write(BinaryWriterEx output)
        {
            output.WriteAscii("MLOD", 4);
            output.Write(Version);
            output.Write(Lods.Length);
            foreach (P3DM_LOD mLod in Lods)
                mLod.Write(output);
        }

        public void WriteToFile(string file, bool allowOverwriting = false)
        {
            FileMode mode = allowOverwriting ? FileMode.Create : FileMode.CreateNew;

            FileStream fs = new FileStream(file, mode);
            using (BinaryWriterEx output = new BinaryWriterEx(fs))
            {
                Write(output);
            }
        }

        public MemoryStream WriteToMemory()
        {
            MemoryStream memStream = new MemoryStream(100000);
            BinaryWriterEx outStream = new BinaryWriterEx(memStream);
            Write(outStream);
            outStream.Position = 0;
            return memStream;
        }

        public void WriteToStream(Stream stream)
        {
            BinaryWriterEx output = new BinaryWriterEx(stream);
            Write(output);
        }
    }
}