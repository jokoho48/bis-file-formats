using System.Diagnostics;

namespace BIS.Core
{
    public struct PackedColor
    {
        private readonly uint value;

        public byte A8 => (byte) ((value >> 24) & 0xff);
        public byte R8 => (byte) ((value >> 16) & 0xff);
        public byte G8 => (byte) ((value >> 8) & 0xff);
        public byte B8 => (byte) (value & 0xff);

        public PackedColor(uint value)
        {
            this.value = value;
        }

        public PackedColor(byte r, byte g, byte b, byte a = 255)
        {
            value = PackColor(r, g, b, a);
        }

        public PackedColor(float r, float g, float b, float a)
        {
            Debug.Assert(r <= 1.0f && r >= 0 && !float.IsNaN(r));
            Debug.Assert(g <= 1.0f && g >= 0 && !float.IsNaN(g));
            Debug.Assert(b <= 1.0f && b >= 0 && !float.IsNaN(b));
            Debug.Assert(a <= 1.0f && a >= 0 && !float.IsNaN(a));

            byte r8 = (byte) (r * 255);
            byte g8 = (byte) (g * 255);
            byte b8 = (byte) (b * 255);
            byte a8 = (byte) (a * 255);

            value = PackColor(r8, g8, b8, a8);
        }

        internal static uint PackColor(byte r, byte g, byte b, byte a)
        {
            return (uint) ((a << 24) | (r << 16) | (g << 8)) | b;
        }
    }
}