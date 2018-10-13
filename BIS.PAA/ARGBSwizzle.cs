using System;

namespace BIS.PAA
{
    internal struct ARGBSwizzle
    {
        internal TexSwizzle SwizB;
        internal TexSwizzle SwizG;
        internal TexSwizzle SwizR;
        internal TexSwizzle SwizA;

        internal TexSwizzle this[int ch]
        {
            get
            {
                switch (ch)
                {
                    case 0: return SwizA;
                    case 1: return SwizR;
                    case 2: return SwizG;
                    case 3: return SwizB;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                switch (ch)
                {
                    case 0:
                        SwizA = value;
                        break;

                    case 1:
                        SwizR = value;
                        break;

                    case 2:
                        SwizG = value;
                        break;

                    case 3:
                        SwizB = value;
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        static ARGBSwizzle()
        {
            Default.SwizA = TexSwizzle.TSAlpha;
            Default.SwizR = TexSwizzle.TSRed;
            Default.SwizG = TexSwizzle.TSGreen;
            Default.SwizB = TexSwizzle.TSBlue;
        }

        internal static ARGBSwizzle Default;
    }
}