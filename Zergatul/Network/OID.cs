using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public partial class OID
    {
        public string DotNotation { get; private set; }
        public string Identifier { get; private set; }
        public string ShortName { get; private set; }

        public OID(string dotnotation)
        {
            this.DotNotation = dotnotation;
        }

        public OID(string dotnotation, string identifier)
            : this(dotnotation)
        {
            this.Identifier = identifier;
        }

        public OID(string dotnotation, string identifier, string shortname)
            : this(dotnotation, identifier)
        {
            this.ShortName = shortname;
        }

        public override string ToString()
        {
            return $"{DotNotation} [{Identifier}]";
        }

        public static OID Find(string dotnotation)
        {
            return All.FirstOrDefault(oid => oid.DotNotation == dotnotation);
        }
    }
}