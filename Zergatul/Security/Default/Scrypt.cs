namespace Zergatul.Security.Default
{
    class Scrypt : ScryptBase
    {
        public override byte[] GetKeyBytes()
        {
            Validate();

            var scrypt = new Cryptography.KDF.Scrypt();
            byte[] bytes = scrypt.DeriveKeyBytes(
                _parameters.Password,
                _parameters.Salt,
                _parameters.r,
                _parameters.N,
                _parameters.p,
                _parameters.KeyLength);

            _parameters = null;

            return bytes;
        }
    }
}