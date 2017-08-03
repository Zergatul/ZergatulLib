using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// Represents store for trusted root certificates. Used during creating and validating certificate chains.
    /// </summary>
    public interface IRootCertificateStore
    {
        X509Certificate FindBySubjectKeyId(byte[] subject);
    }
}