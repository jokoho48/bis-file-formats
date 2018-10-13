#region

using BIS.Core.Streams;
using System;

#endregion

namespace BIS.Core.Config
{
    #region ParamEntries

    public abstract class ParamEntry
    {
        public string Name { get; protected set; }

        public static ParamEntry ReadParamEntry(BinaryReaderEx input)
        {
            EntryType entryType = (EntryType)input.ReadByte();

            switch (entryType)
            {
                case EntryType.Class:
                    return new ParamClass(input);

                case EntryType.Array:
                    return new ParamArray(input);

                case EntryType.Value:
                    return new ParamValue(input);

                case EntryType.ClassDecl:
                    return new ParamExternClass(input);

                case EntryType.ClassDelete:
                    return new ParamDeleteClass(input);

                default: throw new ArgumentException("Unknown ParamEntry Type", nameof(entryType));
            }
        }

        public virtual string ToString(int indentionLevel = 0)
        {
            return base.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }
    }

    #endregion
}