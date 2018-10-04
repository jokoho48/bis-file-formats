using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class NamedSelectionTagg : Tagg
    {
        public NamedSelectionTagg(string name, byte[] points, byte[] faces) : base(
            (uint) (points.Length + faces.Length), name)
        {
            Points = points;
            Faces = faces;
        }

        public NamedSelectionTagg(BinaryReaderEx input, int nPoints, int nFaces) : base(input)
        {
            Read(input, nPoints, nFaces);
        }

        public byte[] Points { get; set; }
        public byte[] Faces { get; set; }

        public void Read(BinaryReaderEx input, int nPoints, int nFaces)
        {
            Points = new byte[nPoints];
            for (int index = 0; index < nPoints; ++index)
                Points[index] = input.ReadByte();
            Faces = new byte[nFaces];
            for (int index = 0; index < nFaces; ++index)
                Faces[index] = input.ReadByte();
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            foreach (byte point in Points)
                output.Write(point);

            foreach (byte face in Faces)
                output.Write(face);
        }
    }
}