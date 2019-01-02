namespace Zergatul.Security.OpenSsl
{
    class Scrypt : ScryptBase
    {
        public override byte[] GetKeyBytes()
        {
            Validate();

            byte[] bytes = new byte[_parameters.KeyLength];

            int result = OpenSsl.EVP_PBE_scrypt(
                _parameters.Password,
                _parameters.Password.Length,
                _parameters.Salt,
                _parameters.Salt?.Length ?? 0,
                _parameters.N,
                (ulong)_parameters.r,
                (ulong)_parameters.p,
                ulong.MaxValue,
                bytes,
                bytes.Length);
            if (result != 1)
                throw new OpenSslException();

            _parameters = null;

            return bytes;
        }
    }
}