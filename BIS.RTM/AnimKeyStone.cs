#region

using BIS.Core.Streams;

#endregion

namespace BIS.RTM
{
    public enum AnimKeystoneTypeID
    {
        AKSStepSound,
        NAnimKeystoneTypeID,
        AKSUninitialized = -1
    }

    public enum AnimMetaDataID
    {
        AMDWalkCycles,
        AMDAnimLength,
        NAnimMetaDataID,
        AMDUninitialized = -1
    }

    public class AnimKeyStone
    {
        public AnimKeyStone(BinaryReaderEx input)
        {
            ID = (AnimKeystoneTypeID) input.ReadInt32();
            StringID = input.ReadAsciiz();
            Time = input.ReadSingle();
            Value = input.ReadAsciiz();
        }

        public AnimKeystoneTypeID ID { get; }
        public string StringID { get; }
        public float Time { get; }
        public string Value { get; }
    }
}