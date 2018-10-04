using BIS.Core.Math;
using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class AnimationTagg : Tagg
    {
        public AnimationTagg(float frameTime, Vector3P[] framePoints) : base((uint) (framePoints.Length * 4 + 4),
            "#Animation#")
        {
            FrameTime = frameTime;
            FramePoints = framePoints;
        }

        public AnimationTagg(BinaryReaderEx input) : base(input)
        {
            Read(input);
        }

        public float FrameTime { get; set; }
        public Vector3P[] FramePoints { get; set; }

        public void Read(BinaryReaderEx input)
        {
            uint num = (DataSize - 4) / 12;
            FrameTime = input.ReadSingle();
            FramePoints = new Vector3P[num];
            for (int i = 0; i < num; ++i)
                FramePoints[i] = new Vector3P(input);
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            output.Write(FrameTime);
            for (int index = 0; index < FramePoints.Length; ++index)
                FramePoints[index].Write(output);
        }
    }
}