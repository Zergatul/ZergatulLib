using System;

namespace Zergatul.Security
{
    public abstract class PBKDF2Base : KeyDerivationFunction
    {
        protected PBKDF2Parameters _parameters;

        public override void Init(KDFParameters parameters)
        {
            _parameters = parameters as PBKDF2Parameters;
            if (_parameters == null)
                throw new ArgumentException("parameters must be instance of PBKDF2Parameters");
        }

        protected void Validate()
        {
            if (_parameters == null)
                throw new InvalidOperationException("PBKDF2 is not initialized");
            if (_parameters.Password == null)
                throw new InvalidOperationException("Parameters.Password is null");
            if (string.IsNullOrEmpty(_parameters.MessageDigest))
                throw new InvalidOperationException("Parameters.MessageDigest is null");
            if (_parameters.Iterations <= 0)
                throw new InvalidOperationException("Parameters.Iterations must be greater than 0");
        }
    }
}