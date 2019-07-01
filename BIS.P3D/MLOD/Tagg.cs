#region

using System;
using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public abstract class Tagg
    {
        protected Tagg(uint dataSize, string taggName)
        {
            Name = taggName;
            DataSize = dataSize;
        }

        protected Tagg(BinaryReaderEx input)
        {
            if (!input.ReadBoolean())
                throw new FormatException("Deactivated Tagg?");

            Name = input.ReadAsciiz();
            DataSize = input.ReadUInt32();
        }

        public string Name { get; set; }
        public uint DataSize { get; set; }

        protected void WriteHeader(BinaryWriterEx output)
        {
            output.Write(true);
            output.WriteAsciiz(Name);
            output.Write(DataSize);
        }

        public abstract void Write(BinaryWriterEx output);

        public static Tagg ReadTagg(BinaryReaderEx input, int nPoints, Face[] faces)
        {
            if (!input.ReadBoolean())
                throw new Exception("Deactivated Tagg?");
            string taggName = input.ReadAsciiz();
            input.Position -= taggName.Length + 2;

            switch (taggName)
            {
                case "#SharpEdges#":
                    return new SharpEdgesTagg(input);

                case "#Property#":
                    return new PropertyTagg(input);

                case "#Mass#":
                    return new MassTagg(input);

                case "#UVSet#":
                    return new UVSetTagg(input, faces);

                case "#Lock#":
                    return new LockTagg(input, nPoints, faces.Length);

                case "#Selected#":
                    return new SelectedTagg(input, nPoints, faces.Length);

                case "#Animation#":
                    return new AnimationTagg(input);

                case "#EndOfFile#":
                    return new EOFTagg(input);

                default:
                    return new NamedSelectionTagg(input, nPoints, faces.Length);
            }
        }
    }
}