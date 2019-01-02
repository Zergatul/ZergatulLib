namespace Zergatul.Security.Default
{
    class PBKDF2 : PBKDF2Base
    {
        public override byte[] GetKeyBytes()
        {
            Validate();

            var pbkdf2 = new Cryptography.KDF.PBKDF2(_parameters.MessageDigest);
            byte[] bytes = pbkdf2.DeriveKeyBytes(_parameters.Password, _parameters.Salt, _parameters.Iterations, (ulong)_parameters.KeyLength);

            _parameters = null;

            return bytes;
        }
    }
}