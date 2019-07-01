﻿#region

using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class UVSetTagg : Tagg
    {
        public UVSetTagg(uint dataSize, int uvNr, float[][,] uvs) : base(dataSize, "#UVSet#")
        {
            UvSetNr = uvNr;
            FaceUVs = uvs;
        }

        public UVSetTagg(BinaryReaderEx input, Face[] faces) : base(input)
        {
            Read(input, faces);
        }

        public int UvSetNr { get; set; }
        public float[][,] FaceUVs { get; set; }

        public void Read(BinaryReaderEx input, Face[] faces)
        {
            UvSetNr = input.ReadInt32();
            FaceUVs = new float[faces.Length][,];
            for (int i = 0; i < faces.Length; ++i)
            {
                FaceUVs[i] = new float[faces[i].VertexCount, 2];
                for (int j = 0; j < faces[i].VertexCount; ++j)
                {
                    FaceUVs[i][j, 0] = input.ReadSingle();
                    FaceUVs[i][j, 1] = input.ReadSingle();
                }
            }
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            output.Write(UvSetNr);
            foreach (float[,] faceUV in FaceUVs)
            {
                for (int j = 0; j < faceUV.Length / 2; ++j)
                {
                    output.Write(faceUV[j, 0]);
                    output.Write(faceUV[j, 1]);
                }
            }
        }
    }
}