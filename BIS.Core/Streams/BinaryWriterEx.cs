#region

using System.IO;

#endregion

namespace BIS.Core.Streams
{
    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(System.IO.Stream dstStream) : base(dstStream)
        {
        }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public void WriteAscii(string text, uint len)
        {
            Write(text.ToCharArray());
            uint num = (uint)(len - text.Length);
            for (int index = 0; index < num; ++index)
                Write(char.MinValue); //ToDo: check encoding, should always write one byte and never two or more
        }

        public void WriteAsciiz(string text)
        {
            Write(text.ToCharArray());
            Write(char.MinValue);
        }
    }
}