#region

using System.Linq;
using System.Text;
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

    public class ObjectTreeLeaf
    {
        public ObjectTreeLeaf(BinaryReaderEx input, int layerVersion)
        {
            Area = new MapArea(input, layerVersion >= 4);
            Color = input.ReadBytes(4);
            HashValue = input.ReadInt32();
            ObjectTypeCount = input.ReadInt32();

            ObjectTypeHashes = new int[ObjectTypeCount];
            ObjectInfos = new ObjectInfo[ObjectTypeCount][];
            for (int curObjType = 0; curObjType < ObjectTypeCount; curObjType++)
            {
                int nObjects = input.ReadInt32();
                ObjectTypeHashes[curObjType] = input.ReadInt32();
                ObjectInfos[curObjType] = new ObjectInfo[nObjects];
                for (int obj = 0; obj < nObjects; obj++)
                {
                    ObjectInfos[curObjType][obj] = new ObjectInfo(input);
                }
            }
        }

        public MapArea Area { get; }
        public byte[] Color { get; }

        //it's currently not clear what object hash is stored here; maybe the one covering the most area
        public int HashValue { get; }
        public int ObjectTypeCount { get; }
        public int[] ObjectTypeHashes { get; }
        public ObjectInfo[][] ObjectInfos { get; }

        public override string ToString()
        {
            string node = $"{Area};{HashValue}:";
            StringBuilder sb = new StringBuilder(node);
            sb.AppendLine();
            for (int i = 0; i < ObjectTypeCount; i++)
            {
                int objType = ObjectTypeHashes[i];
                foreach (ObjectInfo objinfo in ObjectInfos[i])
                {
                    sb.AppendLine($"    {objType};{objinfo}");
                }
            }

            return sb.ToString();
        }
    }

    public class ObjectInfo
    {
        public ObjectInfo(BinaryReaderEx input)
        {
            X = input.ReadDouble();
            Y = input.ReadDouble();
            Yaw = input.ReadSingle();
            Pitch = input.ReadSingle();
            Roll = input.ReadSingle();
            Scale = input.ReadSingle();
            RelativeElevation = input.ReadSingle();
            ID = input.ReadInt32();
        }

        public double X { get; }
        public double Y { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
        public float Scale { get; }
        public float RelativeElevation { get; }
        public int ID { get; }

        public override string ToString()
        {
            return
                $"{X:0.###};{Y:0.###};{Yaw:0.###};{Pitch:0.###};{Roll:0.###};{Scale:0.###};{RelativeElevation:0.###};{ID}";
        }
    }
}