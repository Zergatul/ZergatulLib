using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class UTCTime : ASN1TimeElement
    {
        public UTCTime()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.UTCTime
            })
        {

        }

        protected override byte[] BodyToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ReadBody(Stream stream)
        {
            byte[] buffer = ReadBuffer(stream, checked((int)Length));
            string str = Encoding.ASCII.GetString(buffer);

            if (str.Length == 13 && str.ToLowerInvariant().EndsWith("z"))
                Date = DateTime.ParseExact(str.Substring(0, 12), "yyMMddHHmmss", DateTimeFormatInfo.InvariantInfo).ToLocalTime();
            else
                throw new NotImplementedException();
        }
    }
}