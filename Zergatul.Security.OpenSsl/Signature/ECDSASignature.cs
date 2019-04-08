using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Zergatul.Security.OpenSsl.Signature
{
    class ECDSASignature : Security.Signature
    {
        private State _state;
        private Security.MessageDigest _md;
        private List<byte> _buffer;
        private SecureRandom _random;
        private IntPtr _eckey;
        private bool _lowS;

        public override void InitForSign(PrivateKey key, SignatureParameters parameters)
        {
            Init(parameters);

            switch (key)
            {
                case RawPrivateKey raw:
                    var bignum = Native.BN_bin2bn(raw.Data, raw.Data.Length, IntPtr.Zero);
                    if (bignum == IntPtr.Zero)
                        throw new OpenSslException();
                    try
                    {
                        if (Native.EC_KEY_set_private_key(_eckey, bignum) != 1)
                            throw new OpenSslException();
                    }
                    finally
                    {
                        Native.BN_free(bignum);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            _state = State.Sign;
        }

        public override void InitForVerify(PublicKey key, SignatureParameters parameters)
        {
            Init(parameters);

            switch (key)
            {
                case RawPublicKey raw:
                    var ecgroup = Native.EC_KEY_get0_group(_eckey);
                    if (ecgroup == IntPtr.Zero)
                        throw new InvalidOperationException();
                    var ecpoint = Native.EC_POINT_new(ecgroup);
                    if (ecpoint == IntPtr.Zero)
                        throw new OpenSslException();
                    if (Native.EC_POINT_oct2point(ecgroup, ecpoint, raw.Data, raw.Data.Length, IntPtr.Zero) != 1)
                        throw new OpenSslException();
                    if (Native.EC_KEY_set_public_key(_eckey, ecpoint) != 1)
                        throw new OpenSslException();
                    break;

                default:
                    throw new NotImplementedException();
            }

            _state = State.Verify;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (_state != State.Sign && _state != State.Verify)
                throw new InvalidOperationException();

            if (_md != null)
                _md.Update(data, offset, length);
            else
                for (int i = 0; i < length; i++)
                    _buffer.Add(data[offset + i]);
        }

        public override byte[] Sign()
        {
            if (_state != State.Sign)
                throw new InvalidOperationException();

            _state = State.Finished;

            byte[] hash = _md != null ? _md.Digest() : _buffer.ToArray();

            IntPtr ecgroup = IntPtr.Zero;
            IntPtr q = IntPtr.Zero;

            IntPtr ecdsaSig;
            if (_random == null)
            {
                ecdsaSig = Native.ECDSA_do_sign(hash, hash.Length, _eckey);
                if (ecdsaSig == IntPtr.Zero)
                    throw new OpenSslException();
            }
            else
            {
                ecgroup = Native.EC_KEY_get0_group(_eckey);
                if (ecgroup == IntPtr.Zero)
                    throw new InvalidOperationException();

                q = Native.EC_GROUP_get0_order(ecgroup);
                if (q == IntPtr.Zero)
                    throw new OpenSslException();
            CalculateK:
                IntPtr k = BigInteger.Random(new BigInteger(q), _random).Pointer;
                if (Native.BN_is_zero(k) == 1)
                    goto CalculateK;

                IntPtr ctx = Native.BN_CTX_new();
                if (ctx == IntPtr.Zero)
                    throw new OpenSslException();
                IntPtr kInv = Native.BN_mod_inverse(IntPtr.Zero, k, q, ctx);
                if (kInv == IntPtr.Zero)
                    throw new OpenSslException();

                IntPtr ecpoint = Native.EC_POINT_new(ecgroup);
                if (ecpoint == IntPtr.Zero)
                    throw new OpenSslException();

                if (Native.EC_POINT_mul(ecgroup, ecpoint, k, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 1)
                    throw new OpenSslException();

                IntPtr x = Native.BN_new();
                if (x == IntPtr.Zero)
                    throw new OpenSslException();
                IntPtr y = Native.BN_new();
                if (y == IntPtr.Zero)
                    throw new OpenSslException();

                if (Native.EC_POINT_get_affine_coordinates_GFp(ecgroup, ecpoint, x, y, IntPtr.Zero) != 1)
                    throw new OpenSslException();

                IntPtr r = Native.BN_new();
                if (r == IntPtr.Zero)
                    throw new OpenSslException();
                if (Native.BN_div(IntPtr.Zero, r, x, q, IntPtr.Zero) != 1)
                    throw new OpenSslException();
                if (Native.BN_is_zero(r) == 1)
                    goto CalculateK;

                Native.BN_CTX_free(ctx);

                ecdsaSig = Native.ECDSA_do_sign_ex(hash, hash.Length, kInv, r, _eckey);
                if (ecdsaSig == IntPtr.Zero)
                    throw new OpenSslException();

                // TODO: free memory
            }

            try
            {
                if (_lowS)
                {
                    IntPtr r = IntPtr.Zero;
                    IntPtr s = IntPtr.Zero;
                    Native.ECDSA_SIG_get0(ecdsaSig, ref r, ref s);

                    if (ecgroup == IntPtr.Zero)
                    {
                        ecgroup = Native.EC_KEY_get0_group(_eckey);
                        if (ecgroup == IntPtr.Zero)
                            throw new InvalidOperationException();
                    }

                    if (q == IntPtr.Zero)
                    {
                        q = Native.EC_GROUP_get0_order(ecgroup);
                        if (q == IntPtr.Zero)
                            throw new OpenSslException();
                    }

                    IntPtr half = Native.BN_new();
                    if (half == IntPtr.Zero)
                        throw new OpenSslException();
                    if (Native.BN_rshift1(half, q) != 1)
                        throw new OpenSslException();
                    if (Native.BN_cmp(s, half) == 1) // s > half
                    {
                        if (Native.BN_sub(s, q, s) != 1)
                            throw new OpenSslException();
                        Native.ECDSA_SIG_set0(ecdsaSig, IntPtr.Zero, s);
                    }
                }

                IntPtr alloc = IntPtr.Zero;
                int size = Native.i2d_ECDSA_SIG(ecdsaSig, ref alloc);
                if (size == 0)
                    throw new OpenSslException();
                alloc = Marshal.AllocHGlobal(size);
                IntPtr copy = alloc;
                try
                {
                    size = Native.i2d_ECDSA_SIG(ecdsaSig, ref copy);
                    if (size == 0)
                        throw new OpenSslException();
                    byte[] result = new byte[size];
                    Marshal.Copy(alloc, result, 0, size);
                    return result;
                }
                finally
                {
                    Marshal.FreeHGlobal(alloc);
                }
            }
            finally
            {
                Native.ECDSA_SIG_free(ecdsaSig);
            }
        }

        public override bool Verify(byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));
            if (_state != State.Verify)
                throw new InvalidOperationException();

            _state = State.Finished;

            byte[] hash = _md != null ? _md.Digest() : _buffer.ToArray();

            IntPtr alloc = Marshal.AllocHGlobal(signature.Length);
            Marshal.Copy(signature, 0, alloc, signature.Length);
            IntPtr copy = alloc;
            IntPtr ecdsaSig = IntPtr.Zero;
            try
            {
                ecdsaSig = Native.d2i_ECDSA_SIG(IntPtr.Zero, ref copy, signature.Length);
                if (ecdsaSig == IntPtr.Zero)
                    throw new OpenSslException();
                int result = Native.ECDSA_do_verify(hash, hash.Length, ecdsaSig, _eckey);
                if (result == 0)
                    return false;
                else if (result == 1)
                    return true;
                else
                    throw new OpenSslException();
            }
            finally
            {
                Marshal.FreeHGlobal(alloc);
                if (ecdsaSig != IntPtr.Zero)
                    Native.ECDSA_SIG_free(ecdsaSig);
            }
        }

        private void Init(SignatureParameters sp)
        {
            if (_state != State.Uninitialized)
                throw new InvalidOperationException();

            var parameters = sp as ECDSASignatureParameters;
            if (parameters == null)
                throw new InvalidOperationException();

            _md = parameters.MessageDigest;
            if (_md == null)
                _buffer = new List<byte>();

            _random = parameters.Random;
            _lowS = parameters.LowS;

            int nid = ECGroupConverter.GroupNameToNID(parameters.Curve);
            _eckey = Native.EC_KEY_new_by_curve_name(nid);
            if (_eckey == IntPtr.Zero)
                throw new OpenSslException();
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

        private enum State
        {
            Uninitialized,
            Sign,
            Verify,
            Finished
        }
    }
}