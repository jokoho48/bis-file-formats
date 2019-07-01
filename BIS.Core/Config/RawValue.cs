#region

using System;
using System.Globalization;
using BIS.Core.Streams;

#endregion

namespace BIS.Core.Config
{
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
            switch (Type)
            {
                case ValueType.Expression:
                case ValueType.Generic:
                    return $"\"{Value}\"";

                case ValueType.Float:
                    return ((float) Value).ToString(CultureInfo.InvariantCulture);

                default:
                    return Value.ToString();
            }
        }
    }
}