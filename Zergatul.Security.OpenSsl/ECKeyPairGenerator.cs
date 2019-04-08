using System;

namespace Zergatul.Security.OpenSsl
{
    class ECKeyPairGenerator : KeyPairGenerator
    {
        private IntPtr _eckey;

        public override void Init(KeyPairGeneratorParameters parameters)
        {
            var @params = parameters as ECKeyPairGeneratorParameters;
            if (@params == null)
                throw new InvalidOperationException();

            int nid = ECGroupConverter.GroupNameToNID(@params.Curve);
            _eckey = Native.EC_KEY_new_by_curve_name(nid);
            if (_eckey == IntPtr.Zero)
                throw new OpenSslException();
        }

        public override KeyPair GenerateKeyPair()
        {
            throw new NotImplementedException();
        }

        public override PublicKey GetPublicKey(PrivateKey privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));
            if (_eckey == IntPtr.Zero)
                throw new InvalidOperationException();

            switch (privateKey)
            {
                case RawPrivateKey raw:
                    var ecgroup = Native.EC_KEY_get0_group(_eckey);
                    if (ecgroup == IntPtr.Zero)
                        throw new InvalidOperationException();
                    var bignum = Native.BN_bin2bn(raw.Data, raw.Data.Length, IntPtr.Zero);
                    if (bignum == IntPtr.Zero)
                        throw new OpenSslException();
                    try
                    {
                        IntPtr ecpoint = Native.EC_POINT_new(ecgroup);
                        if (Native.EC_POINT_mul(ecgroup, ecpoint, bignum, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 1)
                            throw new OpenSslException();
                        return new ECPublicKey(ecpoint);
                    }
                    finally
                    {
                        Native.BN_free(bignum);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public override byte[] Format(PublicKey publicKey, KeyFormat format)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));
            if (format != KeyFormat.Uncompressed && format != KeyFormat.Compressed)
                throw new InvalidOperationException();

            switch (publicKey)
            {
                case ECPublicKey ec:
                    var ecgroup = Native.EC_KEY_get0_group(_eckey);
                    if (ecgroup == IntPtr.Zero)
                        throw new InvalidOperationException();
                    var pcf = format == KeyFormat.Uncompressed ? Native.PointConversionForm.Uncompressed : Native.PointConversionForm.Compressed;
                    int size = Native.EC_POINT_point2oct(ecgroup, ec.ECPoint, pcf, null, 0, IntPtr.Zero);
                    if (size == 0)
                        throw new OpenSslException();
                    byte[] result = new byte[size];
                    int size2 = Native.EC_POINT_point2oct(ecgroup, ec.ECPoint, pcf, result, size, IntPtr.Zero);
                    if (size2 == 0)
                        throw new OpenSslException();
                    if (size2 != size)
                        throw new InvalidOperationException();
                    return result;

                default:
                    throw new NotImplementedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (_eckey != IntPtr.Zero)
            {
                Native.EC_KEY_free(_eckey);
                _eckey = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }
    }
}