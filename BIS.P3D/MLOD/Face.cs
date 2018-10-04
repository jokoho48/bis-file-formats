﻿#region

using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class Face
    {
        public Face(int nVerts, Vertex[] verts, FaceFlags flags, string texture, string material)
        {
            VertexCount = nVerts;
            Vertices = verts;
            Flags = flags;
            Texture = texture;
            Material = material;
        }

        public Face(BinaryReaderEx input)
        {
            Read(input);
        }

        public int VertexCount { get; private set; }
        public Vertex[] Vertices { get; private set; }
        public FaceFlags Flags { get; private set; }
        public string Texture { get; private set; }
        public string Material { get; private set; }

        public void Read(BinaryReaderEx input)
        {
            VertexCount = input.ReadInt32();
            Vertices = new Vertex[4];
            for (int i = 0; i < 4; ++i)
            {
                Vertices[i] = new Vertex(input);
            }

            Flags = (FaceFlags) input.ReadInt32();
            Texture = input.ReadAsciiz();
            Material = input.ReadAsciiz();
        }

        public void Write(BinaryWriterEx output)
        {
            output.Write(VertexCount);
            for (int i = 0; i < 4; ++i)
                if (i < Vertices.Length && Vertices[i] != null)
                    Vertices[i].Write(output);
                else
                {
                    output.Write(0);
                    output.Write(0);
                    output.Write(0);
                    output.Write(0);
                }

            output.Write((int) Flags);
            output.WriteAsciiz(Texture);
            output.WriteAsciiz(Material);
        }
    }
}