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

        #region System.Object overrides

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var oid = obj as OID;
            if (oid == null)
                return false;
            return this.DotNotation == oid.DotNotation;
        }

        public override int GetHashCode()
        {
            return DotNotation.GetHashCode();
        }

        public override string ToString()
        {
            return $"{DotNotation} [{Identifier}]";
        }

        #endregion

        public static OID Find(string dotnotation)
        {
            return All.FirstOrDefault(oid => oid.DotNotation == dotnotation);
        }
    }
}