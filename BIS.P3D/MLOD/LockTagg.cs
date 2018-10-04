using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class LockTagg : Tagg
    {
        public LockTagg(BinaryReaderEx input, int nPoints, int nFaces) : base(input)
        {
            Read(input, nPoints, nFaces);
        }

        public bool[] LockedPoints { get; private set; }
        public bool[] LockedFaces { get; private set; }

        public void Read(BinaryReaderEx input, int nPoints, int nFaces)
        {
            LockedPoints = new bool[nPoints];
            for (int index = 0; index < nPoints; ++index)
                LockedPoints[index] = input.ReadBoolean();
            LockedFaces = new bool[nFaces];
            for (int index = 0; index < nFaces; ++index)
                LockedFaces[index] = input.ReadBoolean();
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            for (int index = 0; index < LockedPoints.Length; ++index)
                output.Write(LockedPoints[index]);
            for (int index = 0; index < LockedFaces.Length; ++index)
                output.Write(LockedFaces[index]);
        }
    }
}