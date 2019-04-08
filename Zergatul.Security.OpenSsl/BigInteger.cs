using System;

namespace Zergatul.Security.OpenSsl
{
    class BigInteger
    {
        public IntPtr Pointer { get; }

        public BigInteger(IntPtr bn)
        {
            if (bn == IntPtr.Zero)
                throw new ArgumentNullException(nameof(bn));

            Pointer = bn;
        }

        public static BigInteger Random(BigInteger value, SecureRandom random)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (Native.BN_is_zero(value.Pointer) == 1)
                throw new InvalidOperationException();

            byte[] bytes = new byte[Native.BN_num_bytes(value.Pointer)];
            if (Native.BN_bn2bin(value.Pointer, bytes) <= 0)
                throw new OpenSslException();

            var result = new byte[bytes.Length];
            byte[] buffer = new byte[1];
            bool finished = false;
            while (!finished)
            {
                bool highBytesEqual = true;
                for (int i = 0; i < result.Length; i++)
                {
                    random.GetNextBytes(buffer);
                    result[i] = buffer[0];
                    if (highBytesEqual && result[i] > bytes[i])
                        break;
                    if (highBytesEqual && result[i] < bytes[i])
                        highBytesEqual = false;
                    if (i == result.Length - 1)
                        finished = true;
                }
            }

            IntPtr bn = Native.BN_bin2bn(result, result.Length, IntPtr.Zero);
            if (bn == IntPtr.Zero)
                throw new OpenSslException();
            return new BigInteger(bn);
        }
    }
}