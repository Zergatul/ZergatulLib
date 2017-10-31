using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public abstract class Address
    {
        protected string _value;

        public string Value => _value;

        public override string ToString()
        {
            return _value;
        }
    }
}