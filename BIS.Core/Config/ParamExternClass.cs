using BIS.Core.Streams;

namespace BIS.Core.Config
{
    public class ParamExternClass : ParamEntry
    {
        public ParamExternClass(BinaryReaderEx input) : this(input.ReadAsciiz())
        {
        }

        public ParamExternClass(string name)
        {
            Name = name;
        }

        public override string ToString(int indentionLevel = 0)
        {
            return $"{new string(' ', indentionLevel * 4)}class {Name};";
        }
    }
}