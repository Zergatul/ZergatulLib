using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class UTCTime : Asn1TimeElement
    {
        public UTCTime()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.UTCTime
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
            _raw.AddRange(buffer);
            string str = Encoding.ASCII.GetString(buffer);

            if (str.Length == 13 && str.ToLowerInvariant().EndsWith("z"))
            {
                Date = DateTime.ParseExact(str.Substring(0, 12), "yyMMddHHmmss", DateTimeFormatInfo.InvariantInfo).ToLocalTime();
                if (Date.Year < 1950)
                    Date = Date.AddYears(100);
            }
            else
                throw new NotImplementedException();
        }
    }
}