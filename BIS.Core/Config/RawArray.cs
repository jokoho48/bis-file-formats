#region

using System.Collections.Generic;
using System.Linq;
using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
    public class RawArray
    {
        public RawArray(IEnumerable<RawValue> values)
        {
            Entries = values.ToList();
        }

        public RawArray(BinaryReaderEx input)
        {
            int nEntries = input.ReadCompactInteger();
            Entries = Enumerable.Range(0, nEntries).Select(_ => new RawValue(input)).ToList();
        }

        public List<RawValue> Entries { get; }

        public override string ToString()
        {
            string valStr = string.Join(", ", Entries.Select(x => x.ToString()));
            return $"{{{valStr}}}";
        }
    }
}