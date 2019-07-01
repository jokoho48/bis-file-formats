#region

using BIS.Core.Streams;

#endregion

namespace BIS.ALB
{
    public class ObjectInfo
    {
        public ObjectInfo(BinaryReaderEx input)
        {
            X = input.ReadDouble();
            Y = input.ReadDouble();
            Yaw = input.ReadSingle();
            Pitch = input.ReadSingle();
            Roll = input.ReadSingle();
            Scale = input.ReadSingle();
            RelativeElevation = input.ReadSingle();
            ID = input.ReadInt32();
        }

        public double X { get; }
        public double Y { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
        public float Scale { get; }
        public float RelativeElevation { get; }
        public int ID { get; }

        public override string ToString()
        {
            return
                $"{X:0.###};{Y:0.###};{Yaw:0.###};{Pitch:0.###};{Roll:0.###};{Scale:0.###};{RelativeElevation:0.###};{ID}";
        }
    }
}