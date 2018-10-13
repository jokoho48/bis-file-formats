using BIS.Core.Streams;
using System.Collections.Generic;

namespace BIS.Core.Config
{
    public class ParamArray : ParamEntry
    {
        public ParamArray(BinaryReaderEx input)
        {
            Name = input.ReadAsciiz();
            Array = new RawArray(input);
        }

        public ParamArray(string name, IEnumerable<RawValue> values)
        {
            Name = name;
            Array = new RawArray(values);
        }

        public ParamArray(string name, params RawValue[] values) : this(name, (IEnumerable<RawValue>)values)
        {
        }

        public RawArray Array { get; }

        public override string ToString(int indentionLevel = 0)
        {
            return $"{new string(' ', indentionLevel * 4)}{Name}[]={Array};";
        }
    }
}