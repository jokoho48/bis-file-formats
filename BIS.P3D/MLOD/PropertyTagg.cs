using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class PropertyTagg : Tagg
    {
        public PropertyTagg(string prop, string val) : base(128, "#Property#")
        {
            PropertyName = prop;
            Value = val;
        }

        public PropertyTagg(BinaryReaderEx input) : base(input)
        {
            Read(input);
        }

        public string PropertyName { get; set; }
        public string Value { get; set; }

        public void Read(BinaryReaderEx input)
        {
            PropertyName = input.ReadAscii(64);
            Value = input.ReadAscii(64);
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
            output.WriteAscii(PropertyName, 64);
            output.WriteAscii(Value, 64);
        }
    }
}