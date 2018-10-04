using BIS.Core.Streams;

namespace BIS.P3D.MLOD
{
    public class EOFTagg : Tagg
    {
        public EOFTagg() : base(0, "#EndOfFile#")
        {
        }

        public EOFTagg(BinaryReaderEx input) : base(input)
        {
        }

        public override void Write(BinaryWriterEx output)
        {
            WriteHeader(output);
        }
    }
}