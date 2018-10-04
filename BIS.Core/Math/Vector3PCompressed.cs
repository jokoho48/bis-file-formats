using BIS.Core.Streams;

namespace BIS.Core.Math
{
    public struct Vector3PCompressed
    {
        private const float ScaleFactor = -1.0f / 511.0f;

        private readonly int xyz;


        //public float X
        //{
        //    get
        //    {
        //        int x = value & 0x3FF;
        //        if (x > 511) x -= 1024;
        //        return x * ScaleFactor;
        //    }
        //}

        //public float Y
        //{
        //    get
        //    {
        //        int y = (value >> 10) & 0x3FF;
        //        if (y > 511) y -= 1024;
        //        return y * ScaleFactor;
        //    }
        //}

        //public float Z
        //{
        //    get
        //    {
        //        int z = (value >> 20) & 0x3FF;
        //        if (z > 511) z -= 1024;
        //        return z * ScaleFactor;
        //    }
        //}

        public static implicit operator Vector3P(Vector3PCompressed src)
        {
            int x = src.xyz & 0x3FF;
            int y = (src.xyz >> 10) & 0x3FF;
            int z = (src.xyz >> 20) & 0x3FF;
            if (x > 511) x -= 1024;
            if (y > 511) y -= 1024;
            if (z > 511) z -= 1024;

            return new Vector3P(x * ScaleFactor, y * ScaleFactor, z * ScaleFactor);
        }

        public static implicit operator int(Vector3PCompressed src)
        {
            return src.xyz;
        }

        public static implicit operator Vector3PCompressed(int src)
        {
            return new Vector3PCompressed(src);
        }

        public Vector3PCompressed(int value)
        {
            xyz = value;
        }

        public Vector3PCompressed(BinaryReaderEx input)
        {
            xyz = input.ReadInt32();
        }
    }
}