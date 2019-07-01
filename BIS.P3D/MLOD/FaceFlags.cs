#region

using System;

#endregion

namespace BIS.P3D.MLOD
{
    [Flags]
    public enum FaceFlags
    {
        DEFAULT = 0,
        NOLIGHT = 0x1,
        AMBIENT = 0x2,
        FULLLIGHT = 0x4,
        BOTHSIDESLIGHT = 0x20,
        SKYLIGHT = 0x80,
        REVERSELIGHT = 0x100000,
        FLATLIGHT = 0x200000,
        LIGHT_MASK = 0x3000a7
    }
}