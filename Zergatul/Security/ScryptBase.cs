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
        }
    }
}