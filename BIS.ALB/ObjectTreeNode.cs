#region

using System.Linq;
using BIS.Core.Streams;

#endregion

namespace BIS.ALB
{
    public class ObjectTreeNode
    {
        public ObjectTreeNode[] Childs;
        public byte flags;

        public ObjectTreeLeaf[] Objects;

        public ObjectTreeNode(BinaryReaderEx input, int layerVersion)
        {
            NodeType = input.ReadSByte();

            Area = new MapArea(input, layerVersion >= 4);

            Level = input.ReadInt32();
            Color = Enumerable.Range(0, 4).Select(_ => input.ReadByte()).ToArray();
            flags = input.ReadByte();

            if (NodeType == 16)
            {
                Objects = new ObjectTreeLeaf[4];
                byte isChild = flags;
                for (int i = 0; i < 4; i++)
                {
                    if ((isChild & 1) == 1) Objects[i] = new ObjectTreeLeaf(input, layerVersion);
                    isChild >>= 1;
                }
            }
            else
            {
                Childs = new ObjectTreeNode[4];
                byte isChild = flags;
                for (int i = 0; i < 4; i++)
                {
                    if ((isChild & 1) == 1) Childs[i] = new ObjectTreeNode(input, layerVersion);
                    isChild >>= 1;
                }
            }
        }

        public sbyte NodeType { get; }
        public MapArea Area { get; }
        public int Level { get; }
        public byte[] Color { get; }
    }
}