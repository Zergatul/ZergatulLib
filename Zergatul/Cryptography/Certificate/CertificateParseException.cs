﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Certificate
{
    public class CertificateParseException : Exception
    {
        public CertificateParseException()
            : base()
        {

        }

        public CertificateParseException(string message)
            : base(message)
        {

        }

        public static void ThrowIfTrue(bool value)
        {
            if (value)
                throw new CertificateParseException();
        }

        public static void ThrowIfFalse(bool value)
        {
            if (!value)
                throw new CertificateParseException();
        }

        public static void ThrowIfNull(object value)
        {
            if (value == null)
                throw new CertificateParseException();
        }
    }
}