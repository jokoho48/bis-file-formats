#region

using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
    public class ParamValue : ParamEntry
    {
        public ParamValue(string name, bool value)
        {
            Name = name;
            Value = new RawValue(value ? 1 : 0);
        }

        public ParamValue(string name, int value)
        {
            Name = name;
            Value = new RawValue(value);
        }

        public ParamValue(string name, float value)
        {
            Name = name;
            Value = new RawValue(value);
        }

        public ParamValue(string name, string value)
        {
            Name = name;
            Value = new RawValue(value);
        }

        public ParamValue(BinaryReaderEx input)
        {
            ValueType subtype = (ValueType) input.ReadByte();
            Name = input.ReadAsciiz();
            Value = new RawValue(input, subtype);
        }

        public RawValue Value { get; }

        public override string ToString(int indentionLevel = 0)
        {
            return $"{new string(' ', indentionLevel * 4)}{Name}={Value};";
        }
    }
}