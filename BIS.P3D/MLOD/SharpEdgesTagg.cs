#region

using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class SharpEdgesTagg : Tagg
    {
        public SharpEdgesTagg(BinaryReaderEx input) : base(input)
        {
            Read(input);
        }

        public int[,] PointIndices { get; private set; }

        public void Read(BinaryReaderEx input)
        {
            uint num = DataSize / 8;
            PointIndices = new int[num, 2];
            for (int index = 0; index < num; ++index)
            {
                PointIndices[index, 0] = input.ReadInt32();
                PointIndices[index, 1] = input.ReadInt32();
            }
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            uint num = DataSize / 8;
            for (int index = 0; index < num; ++index)
            {
                output.Write(PointIndices[index, 0]);
                output.Write(PointIndices[index, 1]);
            }
        }
    }
}