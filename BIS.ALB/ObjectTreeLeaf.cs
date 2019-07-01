#region

using System.Text;
using BIS.Core.Streams;

#endregion

namespace BIS.ALB
{
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
}