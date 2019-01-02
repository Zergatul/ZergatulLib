namespace Zergatul.Security
{
    public class ECDSASignatureParameters : SignatureParameters
    {
        public string Curve;
        public SecureRandom Random;
        public bool LowS;
    }
}