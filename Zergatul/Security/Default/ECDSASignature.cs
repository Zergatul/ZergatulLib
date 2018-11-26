using System;
using System.Collections.Generic;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Security.Default
{
    class ECDSASignature : Signature
    {
        private State _state = State.Uninitialized;
        private MessageDigest _md;
        private List<byte> _buffer;
        private ECPDSA _ecdsa;

        public override void InitForSign(PrivateKey key, SignatureParameters parameters)
        {
            Init(parameters);

            var random = ((ECDSASignatureParameters)parameters).Random;
            if (random == null)
                random = SecureRandom.GetInstance(SecureRandoms.Default);
            _ecdsa.Random = new SecureRandomAdapter(random);

            switch (key)
            {
                case RawPrivateKey raw:
                    _ecdsa.PrivateKey = new ECPPrivateKey(raw.Data);
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
                    var point = Math.EllipticCurves.PrimeField.ECPoint.FromBytes(raw.Data, _ecdsa.Parameters.Curve);
                    _ecdsa.PublicKey = new ECPPublicKey(point);
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
            return _ecdsa.SignHash(hash);
        }

        public override bool Verify(byte[] signature)
        {
            if (_state != State.Verify)
                throw new InvalidOperationException();

            _state = State.Finished;

            byte[] hash = _md != null ? _md.Digest() : _buffer.ToArray();
            return _ecdsa.VerifyHash(hash, signature);
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

            Math.EllipticCurves.PrimeField.EllipticCurve curve;
            switch (parameters.Curve)
            {
                case Curves.secp256k1: curve = Math.EllipticCurves.PrimeField.EllipticCurve.secp256k1; break;
                default:
                    throw new NotImplementedException();
            }

            _ecdsa = new ECPDSA();
            _ecdsa.Parameters = new ECPDSAParameters(curve);
            _ecdsa.Parameters.LowS = parameters.LowS;
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