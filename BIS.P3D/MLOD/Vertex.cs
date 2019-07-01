﻿#region

using System.IO;
using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class Vertex
    {
        public Vertex(BinaryReaderEx input)
        {
            Read(input);
        }

        public Vertex(int point, int normal, float u, float v)
        {
            PointIndex = point;
            NormalIndex = normal;
            U = u;
            V = v;
        }

        public int PointIndex { get; private set; }
        public int NormalIndex { get; private set; }
        public float U { get; private set; }
        public float V { get; private set; }

        public void Read(BinaryReaderEx input)
        {
            PointIndex = input.ReadInt32();
            NormalIndex = input.ReadInt32();
            U = input.ReadSingle();
            V = input.ReadSingle();
        }

        public void Write(BinaryWriter output)
        {
            output.Write(PointIndex);
            output.Write(NormalIndex);
            output.Write(U);
            output.Write(V);
        }
    }
}