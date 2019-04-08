namespace Zergatul.Security.OpenSsl.KeyDerivationFunction
{
    class PBKDF2 : PBKDF2Base
    {
        public override byte[] GetKeyBytes()
        {
            Validate();

            byte[] bytes = new byte[_parameters.KeyLength];

            int result = Native.PKCS5_PBKDF2_HMAC(
                _parameters.Password,
                _parameters.Password.Length,
                _parameters.Salt,
                _parameters.Salt?.Length ?? 0,
                _parameters.Iterations,
                MessageDigest.Helper.GetMdByName(_parameters.MessageDigest),
                _parameters.KeyLength,
                bytes);
            if (result != 1)
                throw new OpenSslException();

            _parameters = null;

            return bytes;
        }
    }
}