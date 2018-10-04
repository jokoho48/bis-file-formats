#region

using System;
using System.Globalization;
using System.IO;
using BIS.Core.Streams;

#endregion

namespace BIS.Core.Math
{
    public struct Vector3P
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector3P(float val) : this(val, val, val)
        {
        }

        public Vector3P(BinaryReader input) : this(input.ReadSingle(), input.ReadSingle(), input.ReadSingle())
        {
        }

        public Vector3P(int compressed) : this()
        {
            const double scaleFactor = -1.0 / 511;
            int x = compressed & 0x3FF;
            int y = (compressed >> 10) & 0x3FF;
            int z = (compressed >> 20) & 0x3FF;
            if (x > 511) x -= 1024;
            if (y > 511) y -= 1024;
            if (z > 511) z -= 1024;
            X = (float) (x * scaleFactor);
            Y = (float) (y * scaleFactor);
            Z = (float) (z * scaleFactor);
        }

        public Vector3P(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z);

        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(i), i, "Index to Vector3P has to be 0, 1 or 2");
                }
            }

            set
            {
                switch (i)
                {
                    case 0:
                        X = value;
                        return;
                    case 1:
                        Y = value;
                        return;
                    case 2:
                        Z = value;
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(i), i, "Index to Vector3P has to be 0, 1 or 2");
                }
            }
        }

        public static Vector3P operator -(Vector3P a)
        {
            return new Vector3P(-a.X, -a.Y, -a.Z);
        }

        public void Write(BinaryWriterEx output)
        {
            output.Write(X);
            output.Write(Y);
            output.Write(Z);
        }

        //Scalarmultiplication
        public static Vector3P operator *(Vector3P a, float b)
        {
            return new Vector3P(a.X * b, a.Y * b, a.Z * b);
        }

        //Scalarproduct
        public static float operator *(Vector3P a, Vector3P b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3P operator +(Vector3P a, Vector3P b)
        {
            return new Vector3P(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3P operator -(Vector3P a, Vector3P b)
        {
            return new Vector3P(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3P v)
            {
                return base.Equals(obj) && Equals(v);
            }

            return false;
        }

        //ToDo:
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Vector3P other)
        {
            bool nearlyEqual(float f1, float f2)
            {
                return System.Math.Abs(f1 - f2) < 0.05;
            }

            return nearlyEqual(X, other.X) && nearlyEqual(Y, other.Y) && nearlyEqual(Z, other.Z);
        }

        public override string ToString()
        {
            CultureInfo cultureInfo = new CultureInfo("en-GB");
            return "{" + X.ToString(cultureInfo.NumberFormat) + "," + Y.ToString(cultureInfo.NumberFormat) + "," +
                   Z.ToString(cultureInfo.NumberFormat) + "}";
        }

        public float Distance(Vector3P v)
        {
            Vector3P d = this - v;
            return (float) System.Math.Sqrt(d.X * d.X + d.Y * d.Y + d.Z * d.Z);
        }

        public void Normalize()
        {
            float l = (float) Length;
            X /= l;
            Y /= l;
            Z /= l;
        }

        public static Vector3P CrossProduct(Vector3P a, Vector3P b)
        {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = a.Z * b.X - a.X * b.Z;
            float z = a.X * b.Y - a.Y * b.X;

            return new Vector3P(x, y, z);
        }
    }
}