#region

using BIS.Core.Streams;

#endregion

namespace BIS.P3D.MLOD
{
    public class MassTagg : Tagg
    {
        public MassTagg(float[] mass) : base((uint) (mass.Length * 4), "#Mass#")
        {
            Mass = mass;
        }

        public MassTagg(BinaryReaderEx input) : base(input)
        {
            Read(input);
        }

        public float[] Mass { get; set; }

        public void Read(BinaryReaderEx input)
        {
            uint num = DataSize / 4;
            Mass = new float[num];
            for (int index = 0; index < num; ++index)
                Mass[index] = input.ReadSingle();
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            uint num = DataSize / 4;
            for (int index = 0; index < num; ++index)
                output.Write(Mass[index]);
        }
    }
}