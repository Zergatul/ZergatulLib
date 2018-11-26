namespace Zergatul.Cryptocurrency.P2P
{
    public class InvVect
    {
        public VectorType Type;
        public byte[] Hash;

        public static InvVect[] Parse(byte[] buffer, ref int index)
        {
            int count = checked((int)VarLengthInt.Parse(buffer, ref index));
            var result = new InvVect[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new InvVect();

                result[i].Type = (VectorType)BitHelper.ToUInt32(buffer, index, ByteOrder.LittleEndian);
                index += 4;

                result[i].Hash = ByteArray.SubArray(buffer, index, 32);
                index += 32;
            }

            return result;
        }
    }

    public enum VectorType : uint
    {
        ERROR = 0,
        MSG_TX = 1,
        MSG_BLOCK = 2,
        MSG_FILTERED_BLOCK = 3,
        MSG_CMPCT_BLOCK = 4
    }
}