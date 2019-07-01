#region

using System.IO;
using BIS.Core.Math;
using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class Point
    {
        public Vector3P Position;

        public Point(Vector3P pos, PointFlags flags)
        {
            Position = pos;
            PointFlags = flags;
        }

        public Point(BinaryReader input)
        {
            Position = new Vector3P(input);
            PointFlags = (PointFlags) input.ReadInt32();
        }

        public PointFlags PointFlags { get; }

        public void Write(BinaryWriterEx output)
        {
            Position.Write(output);
            output.Write((int) PointFlags);
        }
    }
}