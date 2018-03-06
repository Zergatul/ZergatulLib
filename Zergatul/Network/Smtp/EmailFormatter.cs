using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Smtp
{
    public static class EmailFormatter
    {
        public static string GetText(string from, string subject, string to, string body)
        {
            return
                $"From: {from}{Environment.NewLine}" +
                $"Subject: {subject}{Environment.NewLine}" +
                $"To: {to}{Environment.NewLine}" +
                Environment.NewLine +
                body;
        }

        public static string GetHtml(string from, string subject, string to, string body)
        {
            return
                $"From: {from}{Environment.NewLine}" +
                $"Subject: {subject}{Environment.NewLine}" +
                $"To: {to}{Environment.NewLine}" +
                $"Mime-Version: 1.0;{Environment.NewLine}" +
                $"Content-Type: text/html; charset=\"utf-8\";{Environment.NewLine}" +
                Environment.NewLine +
                body;
        }
    }
}