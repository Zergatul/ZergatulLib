using System;

namespace Zergatul.Security.OpenSsl
{
    class ECPublicKey : PublicKey
    {
        public IntPtr ECPoint { get; }

        public ECPublicKey(IntPtr point)
        {
            if (point == IntPtr.Zero)
                throw new ArgumentNullException(nameof(point));

            ECPoint = point;
        }

        ~ECPublicKey()
        {
            Native.EC_POINT_free(ECPoint);
        }
    }
}