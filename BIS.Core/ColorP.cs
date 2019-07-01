﻿#region

using System.Globalization;
using System.IO;
using BIS.Core.Streams;

#endregion

namespace BIS.Core
{
    public struct ColorP
    {
        public float Red { get; private set; }
        public float Green { get; private set; }
        public float Blue { get; private set; }
        public float Alpha { get; private set; }

        public ColorP(float r, float g, float b, float a)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        public ColorP(BinaryReader input)
        {
            Red = input.ReadSingle();
            Green = input.ReadSingle();
            Blue = input.ReadSingle();
            Alpha = input.ReadSingle();
        }

        public void Read(BinaryReaderEx input)
        {
            Red = input.ReadSingle();
            Green = input.ReadSingle();
            Blue = input.ReadSingle();
            Alpha = input.ReadSingle();
        }

        public void Write(BinaryWriterEx output)
        {
            output.Write(Red);
            output.Write(Green);
            output.Write(Blue);
            output.Write(Alpha);
        }

        public override string ToString()
        {
            CultureInfo cultureInfo = new CultureInfo("en-GB");
            return "{" + Red.ToString(cultureInfo.NumberFormat) + "," + Green.ToString(cultureInfo.NumberFormat) + "," +
                   Blue.ToString(cultureInfo.NumberFormat) + "," + Alpha.ToString(cultureInfo.NumberFormat) + "}";
        }
    }
}