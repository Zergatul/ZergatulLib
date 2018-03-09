using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Sasl
{
    public static class PlainMechanism
    {
        public static string Encode(string authorizationIdentity, string authenticationIdentity, string password)
        {
            string message = authorizationIdentity + '\0' + authenticationIdentity + '\0' + password;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
        }
    }
}