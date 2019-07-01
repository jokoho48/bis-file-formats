#region

using System;

#endregion

namespace BIS.P3D.MLOD
{
    [Flags]
    public enum PointFlags
    {
        NONE = 0,

        ONLAND = 0x1,
        UNDERLAND = 0x2,
        ABOVELAND = 0x4,
        KEEPLAND = 0x8,
        LAND_MASK = 0xf,

        DECAL = 0x100,
        VDECAL = 0x200,
        DECAL_MASK = 0x300,

        NOLIGHT = 0x10,
        AMBIENT = 0x20,
        FULLLIGHT = 0x40,
        HALFLIGHT = 0x80,
        LIGHT_MASK = 0xf0,

        NOFOG = 0x1000,
        SKYFOG = 0x2000,
        FOG_MASK = 0x3000,

        USER_MASK = 0xff0000,
        USER_STEP = 0x010000,

        SPECIAL_MASK = 0xf000000,
        SPECIAL_HIDDEN = 0x1000000,

        ALL_FLAGS = LAND_MASK | DECAL_MASK | LIGHT_MASK | FOG_MASK | USER_MASK | SPECIAL_MASK
    }
}