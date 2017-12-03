namespace Zergatul.Cryptocurrency.Bitcoin.ScriptOpcodes
{
    public enum Opcode : byte
    {
        OP_0 = 0x00,

        OP_PUSHDATA1 = 0x4C,
        OP_PUSHDATA2 = 0x4D,
        OP_PUSHDATA4 = 0x4E,
        OP_1NEGATE = 0x4F,

        OP_1 = 0x51,
        OP_2 = 0x52,
        OP_3 = 0x53,
        OP_4 = 0x54,
        OP_5 = 0x55,
        OP_6 = 0x56,
        OP_7 = 0x57,
        OP_8 = 0x58,
        OP_9 = 0x59,
        OP_10 = 0x5A,
        OP_11 = 0x5B,
        OP_12 = 0x5C,
        OP_13 = 0x5D,
        OP_14 = 0x5E,
        OP_15 = 0x5F,
        OP_16 = 0x60,

        OP_NOP = 0x61,
        OP_IF = 0x63,
        OP_NOTIF = 0x64,
        OP_ELSE = 0x67,
        OP_ENDIF = 0x68,
        OP_VERIFY = 0x69,
        OP_RETURN = 0x6A,

        OP_TOALTSTACK = 0x6B,
        OP_FROMALTSTACK = 0x6C,
        OP_IFDUP = 0x73,
        OP_DEPTH = 0x74,
        OP_DROP = 0x75,
        OP_DUP = 0x76,
        OP_NIP = 0x77,
        OP_OVER = 0x78,
        OP_PICK = 0x79,
        OP_ROLL = 0x7A,
        OP_ROT = 0x7B,
        OP_SWAP = 0x7C,
        OP_TUCK = 0x7D,
        OP_2DROP = 0x6D,
        OP_2DUP = 0x6E,
        OP_3DUP = 0x6F,
        OP_2OVER = 0x70,
        OP_2ROT = 0x71,
        OP_2SWAP = 0x72,

        OP_SIZE = 0x82,

        OP_EQUAL = 0x87,
        OP_EQUALVERIFY = 0x88,

        OP_RIPEMD160 = 0xA6,
        OP_SHA1 = 0xA7,
        OP_SHA256 = 0xA8,
        OP_HASH160 = 0xA9,
        OP_HASH256 = 0xAA,
        OP_CODESEPARATOR = 0xAB,
        OP_CHECKSIG = 0xAC,
        OP_CHECKSIGVERIFY = 0xAD,
        OP_CHECKMULTISIG = 0xAE,
        OP_CHECKMULTISIGVERIFY = 0xAF,

        OP_CHECKLOCKTIMEVERIFY = 0xB1,
        OP_CHECKSEQUENCEVERIFY = 0xB2
    }
}