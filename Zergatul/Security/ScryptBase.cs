using System;

namespace Zergatul.Security
{
    internal abstract class ScryptBase : KeyDerivationFunction
    {
        protected ScryptParameters _parameters;

        public override void Init(KDFParameters parameters)
        {
            _parameters = parameters as ScryptParameters;
            if (_parameters == null)
                throw new ArgumentException("parameters must be instance of ScryptParameters");
        }

        protected void Validate()
        {
            if (_parameters == null)
                throw new InvalidOperationException("Scrypt is not initialized");
            if (_parameters.Password == null)
                throw new InvalidOperationException("Parameters.Password is null");
            if (_parameters.N == 0)
                throw new InvalidOperationException("Parameters.N cannot be 0");
            if (_parameters.r == 0)
                throw new InvalidOperationException("Parameters.r must be greater than 0");
            if (_parameters.p == 0)
                throw new InvalidOperationException("Parameters.p must be greater than 0");
        }
    }
}