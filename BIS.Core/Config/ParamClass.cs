using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIS.Core.Streams;

namespace BIS.Core.Config
{
    public class ParamClass : ParamEntry
    {
        public ParamClass()
        {
            BaseClassName = "";
            Name = "";
            Entries = new List<ParamEntry>(20);
        }

        public ParamClass(string name, string baseclass, IEnumerable<ParamEntry> entries)
        {
            BaseClassName = baseclass;
            Name = name;
            Entries = entries.ToList();
        }

        public ParamClass(string name, IEnumerable<ParamEntry> entries) : this(name, "", entries)
        {
        }

        public ParamClass(string name, params ParamEntry[] entries) : this(name, (IEnumerable<ParamEntry>) entries)
        {
        }

        public ParamClass(BinaryReaderEx input)
        {
            Name = input.ReadAsciiz();
            uint offset = input.ReadUInt32();
            long oldPos = input.Position;
            input.Position = offset;
            ReadCore(input);
            input.Position = oldPos;
        }

        public ParamClass(BinaryReaderEx input, string fileName)
        {
            Name = fileName;
            ReadCore(input);
        }

        public string BaseClassName { get; private set; }
        public List<ParamEntry> Entries { get; private set; }

        private void ReadCore(BinaryReaderEx input)
        {
            BaseClassName = input.ReadAsciiz();

            int nEntries = input.ReadCompactInteger();
            Entries = Enumerable.Range(0, nEntries).Select(_ => ReadParamEntry(input)).ToList();
        }

        public string ToString(int indentionLevel, bool onlyClassBody)
        {
            string ind = new string(' ', indentionLevel * 4);
            StringBuilder classBody = new StringBuilder();

            int indLvl = onlyClassBody ? indentionLevel : indentionLevel + 1;
            foreach (ParamEntry entry in Entries)
                classBody.AppendLine(entry.ToString(indLvl));

            string classHead = string.IsNullOrEmpty(BaseClassName)
                ? $"{ind}class {Name}"
                : $"{ind}class {Name} : {BaseClassName}";

            return onlyClassBody ? classBody.ToString() : $@"{classHead}
{ind}{{
{classBody}{ind}}};";
        }

        public override string ToString(int indentionLevel = 0)
        {
            return ToString(indentionLevel, false);
        }
    }
}