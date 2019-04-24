namespace Zergatul.Security.Zergatul.MessageDigest
{
    static class LuffaConstants
    {
        public static readonly ulong[] RCW010 = new ulong[]
        {
            0xB6DE10ED303994A6, 0x70F47AAEC0E65299,
            0x0707A3D46CC33A12, 0x1C1E8F51DC56983E,
            0x707A3D451E00108F, 0xAEB285627800423D,
            0xBACA15898F5B7882, 0x40A46F3E96E1DB12
        };

        public static readonly ulong[] RCW014 = new ulong[]
        {
            0x01685F3DE0337818, 0x05A17CF4441BA90D,
            0xBD09CACA7F34D442, 0xF4272B289389217F,
            0x144AE5CCE5A8BCE6, 0xFAA7AE2B5274BAF4,
            0x2E48F1C126889BA7, 0xB923C7049A226E9D
        };

        public static readonly ulong[] RCW230 = new ulong[]
        {
            0xB213AFA5FC20D9D2, 0xC84EBE9534552E25,
            0x4E608A227AD8818F, 0x56D858FE8438764A,
            0x343B138FBB6DE032, 0xD0EC4E3DEDB780C8,
            0x2CEB4882D9847356, 0xB3AD2208A2C78434
        };

        public static readonly ulong[] RCW234 = new ulong[]
        {
            0xE028C9BFE25E72C1, 0x44756F91E623BB72,
            0x7E8FCE325C58A4A4, 0x956548BE1E38E2E7,
            0xFE191BE278E38B9D, 0x3CB226E527586719,
            0x5944A28E36EDA57F, 0xA1C4C355703AACE7
        };

        public static readonly uint[] RC20 = new uint[]
        {
            0xFC20D9D2, 0x34552E25,
            0x7AD8818F, 0x8438764A,
            0xBB6DE032, 0xEDB780C8,
            0xD9847356, 0xA2C78434
        };

        public static readonly uint[] RC24 = new uint[]
        {
            0xE25E72C1, 0xE623BB72,
            0x5C58A4A4, 0x1E38E2E7,
            0x78E38B9D, 0x27586719,
            0x36EDA57F, 0x703AACE7
        };

        public static readonly uint[] RC40 = new uint[]
        {
            0xF0D2E9E3, 0xAC11D7FA,
            0x1BCB66F2, 0x6F2D9BC9,
            0x78602649, 0x8EDAE952,
            0x3B6BA548, 0xEDAE9520
        };

        public static readonly uint[] RC44 = new uint[]
        {
            0x5090D577, 0x2D1925AB,
            0xB46496AC, 0xD1925AB0,
            0x29131AB6, 0x0FC053C3,
            0x3F014F0C, 0xFC053C31
        };
    }
}