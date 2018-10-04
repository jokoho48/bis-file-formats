using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class SelectedTagg : Tagg
    {
        public SelectedTagg(BinaryReaderEx input, int nPoints, int nFaces) : base(input)
        {
            Read(input, nPoints, nFaces);
        }

        public byte[] WeightedPoints { get; set; }
        public byte[] Faces { get; set; }

        public void Read(BinaryReaderEx input, int nPoints, int nFaces)
        {
            WeightedPoints = new byte[nPoints];
            for (int index = 0; index < nPoints; ++index)
                WeightedPoints[index] = input.ReadByte();
            Faces = new byte[nFaces];
            for (int index = 0; index < nFaces; ++index)
                Faces[index] = input.ReadByte();
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            foreach (byte point in WeightedPoints)
                output.Write(point);

            foreach (byte face in Faces)
                output.Write(face);
        }
    }
}