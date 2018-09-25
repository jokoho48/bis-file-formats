#region

using BIS.Core.Math;
using BIS.Core.Streams;

#endregion

namespace BIS.WRP
{
    public class StaticEntityInfo
    {
        public StaticEntityInfo(BinaryReaderEx input)
        {
            ClassName = input.ReadAsciiz();
            ShapeName = input.ReadAsciiz();
            Position = new Vector3P(input);
            ObjectId = input.ReadInt32();
        }

        public string ClassName { get; }
        public string ShapeName { get; }
        public Vector3P Position { get; }
        public ObjectId ObjectId { get; }
    }
}