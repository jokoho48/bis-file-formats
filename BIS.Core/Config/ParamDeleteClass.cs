#region

using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
    public class ParamDeleteClass : ParamEntry
    {
        public ParamDeleteClass(BinaryReaderEx input) : this(input.ReadAsciiz())
        {
        }

        public ParamDeleteClass(string name)
        {
            Name = name;
        }

        public override string ToString(int indentionLevel = 0)
        {
            return $"{new string(' ', indentionLevel * 4)}delete {Name};";
        }
    }
}