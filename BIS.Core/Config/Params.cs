#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
    #region Enums

    public enum EntryType : byte
    {
        Class,
        Value,
        Array,
        ClassDecl,
        ClassDelete,
        ArraySpec
    }

    public enum ValueType : byte
    {
        Generic, // generic = string
        Float,
        Int,
        Array, //not used?
        Expression,
        NSpecValueType,
        Int64
    }

    #endregion

    #region ParamEntries

    public abstract class ParamEntry
    {
        public string Name { get; protected set; }

        public static ParamEntry ReadParamEntry(BinaryReaderEx input)
        {
            EntryType entryType = (EntryType) input.ReadByte();

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

            if (onlyClassBody)
                return classBody.ToString();

            return
                $@"{classHead}
{ind}{{
{classBody}{ind}}};";
        }

        public override string ToString(int indentionLevel = 0)
        {
            return ToString(indentionLevel, false);
        }
    }

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

        public ParamArray(string name, params RawValue[] values) : this(name, (IEnumerable<RawValue>) values)
        {
        }

        public RawArray Array { get; }

        public override string ToString(int indentionLevel = 0)
        {
            return $"{new string(' ', indentionLevel * 4)}{Name}[]={Array};";
        }
    }

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

    #endregion

    #region ParamValues

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

    public class RawValue
    {
        public RawValue(string v)
        {
            Type = ValueType.Generic;
            Value = v;
        }

        public RawValue(int v)
        {
            Type = ValueType.Int;
            Value = v;
        }

        public RawValue(long v)
        {
            Type = ValueType.Int64;
            Value = v;
        }

        public RawValue(float v)
        {
            Type = ValueType.Float;
            Value = v;
        }

        public RawValue(BinaryReaderEx input) : this(input, (ValueType) input.ReadByte())
        {
        }

        public RawValue(BinaryReaderEx input, ValueType type)
        {
            Type = type;
            switch (Type)
            {
                case ValueType.Expression: goto case ValueType.Generic;
                case ValueType.Generic:
                    Value = input.ReadAsciiz();
                    break;
                case ValueType.Float:
                    Value = input.ReadSingle();
                    break;
                case ValueType.Int:
                    Value = input.ReadInt32();
                    break;
                case ValueType.Int64:
                    Value = input.ReadInt64();
                    break;
                case ValueType.Array:
                    Value = new RawArray(input);
                    break;

                default: throw new ArgumentException();
            }
        }

        public ValueType Type { get; protected set; }
        public object Value { get; }

        public override string ToString()
        {
            if (Type == ValueType.Expression || Type == ValueType.Generic)
                return $"\"{Value}\"";

            if (Type == ValueType.Float)
                return ((float) Value).ToString(CultureInfo.InvariantCulture);

            return Value.ToString();
        }
    }

    #endregion
}