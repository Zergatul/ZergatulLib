using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Mail
{
    public class MailAddress
    {
        public string Address { get; private set; }
        public string DisplayName { get; private set; }

        public MailAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Invalid address");
            this.Address = address;
        }

        public MailAddress(string address, string name)
            : this(address)
        {
            this.DisplayName = name;
        }
    }
}