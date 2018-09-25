#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
    public class ParamFile
    {
        public ParamFile()
        {
            EnumValues = new List<KeyValuePair<string, int>>(10);
        }

        public ParamFile(Stream stream)
        {
            BinaryReaderEx input = new BinaryReaderEx(stream);

            char[] sig = {'\0', 'r', 'a', 'P'};
            if (!input.ReadBytes(4).SequenceEqual(sig.Select(c => (byte) c)))
                throw new ArgumentException();

            int ofpVersion = input.ReadInt32();
            int version = input.ReadInt32();
            int offsetToEnums = input.ReadInt32();

            Root = new ParamClass(input, "rootClass");

            input.Position = offsetToEnums;
            int nEnumValues = input.ReadInt32();
            EnumValues = Enumerable.Range(0, nEnumValues)
                .Select(_ => new KeyValuePair<string, int>(input.ReadAsciiz(), input.ReadInt32())).ToList();
        }

        public ParamClass Root { get; }
        public List<KeyValuePair<string, int>> EnumValues { get; }

        public override string ToString()
        {
            return Root.ToString(0, true);
        }
    }
}