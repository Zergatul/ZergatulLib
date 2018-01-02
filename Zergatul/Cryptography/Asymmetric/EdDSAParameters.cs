using System;
using System.Text;
using Zergatul.Cryptography.Hash;
using Zergatul.Math.EdwardsCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class EdDSAParameters : AbstractParameters
    {
        public EdCurve Curve { get; set; }

        public virtual byte[] Dom() => null;

        public virtual byte[] PH(byte[] data) => data;

        public EdDSAParameters(EdCurve curve)
        {
            this.Curve = curve;
        }

        public static EdDSAParameters Ed25519 => new EdDSAParameters(EdCurve.Ed25519);
        public static EdDSAParametersCtx Ed25519ctx => new EdDSAParametersCtx(EdCurve.Ed25519, 0, "SigEd25519 no Ed25519 collisions");
        public static EdDSAParametersCtx Ed25519ph => new EdDSAParametersPh(EdCurve.Ed25519, 1, "SigEd25519 no Ed25519 collisions");
    }

    public class EdDSAParametersCtx : EdDSAParameters
    {
        private byte[] _text;

        public byte[] Context { get; set; }

        public byte PHFlag { get; private set; }

        public EdDSAParametersCtx(EdCurve curve, byte phFlag, string prefix)
            : base(curve)
        {
            this.PHFlag = phFlag;
            this._text = Encoding.ASCII.GetBytes(prefix);
        }

        public override byte[] Dom()
        {
            if (Context == null)
                throw new InvalidOperationException();

            if (Context.Length > 255)
                throw new InvalidOperationException();

            byte[] result = new byte[_text.Length + 2 + Context.Length];
            Array.Copy(_text, 0, result, 0, _text.Length);
            result[_text.Length] = PHFlag;
            result[_text.Length + 1] = checked((byte)Context.Length);
            Array.Copy(Context, 0, result, _text.Length + 2, Context.Length);

            return result;
        }
    }

    public class EdDSAParametersPh : EdDSAParametersCtx
    {
        private SHA512 _sha512 = new SHA512();

        public EdDSAParametersPh(EdCurve curve, byte phFlag, string prefix)
            : base(curve, phFlag, prefix)
        {
            this.Context = new byte[0];
        }

        public override byte[] PH(byte[] data)
        {
            _sha512.Reset();
            _sha512.Update(data);
            return _sha512.ComputeHash();
        }
    }
}