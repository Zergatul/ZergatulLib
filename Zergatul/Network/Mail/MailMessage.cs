using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Network.Mime;

namespace Zergatul.Network.Mail
{
    public class MailMessage
    {
        public MailAddress From { get; set; }
        public List<MailAddress> To { get; private set; } = new List<MailAddress>();
        public List<MailAddress> Cc { get; private set; } = new List<MailAddress>();
        public List<MailAddress> Bcc { get; private set; } = new List<MailAddress>();

        public List<MailAttachment> Attachments { get; private set; } = new List<MailAttachment>();

        public Encoding HeadersEncoding { get; set; } = Encoding.UTF8;
        public string Subject { get; set; }
        public string Body { get; set; }
        public Encoding BodyEncoding { get; set; } = Encoding.UTF8;
        public bool IsBodyHtml { get; set; }

        public void Send(Smtp.SmtpConnection smtp)
        {
            if (From == null)
                throw new InvalidOperationException("From is null");

            smtp.Mail(From.Address);
            foreach (var addr in To.Concat(Cc).Concat(Bcc))
                smtp.Recipient(addr.Address);
            smtp.Data(ToBytes());
        }

        public byte[] ToBytes()
        {
            if (BodyEncoding == null)
                throw new InvalidOperationException("BodyEncoding is null");
            if (string.IsNullOrEmpty(BodyEncoding.BodyName))
                throw new InvalidOperationException("Cannot use selected BodyEncoding");

            var message = new Message
            {
                ContentType = IsBodyHtml ? ContentType.Text.Html : ContentType.Text.Plain,
                ContentEncoding = BodyEncoding,
                Content = BodyEncoding.GetBytes(Body)
            };

            if (From != null)
                message.Headers.Add(new Header("From", FormatAddressCollection(new List<MailAddress> { From })));
            if (To.Count > 0)
                message.Headers.Add(new Header("To", FormatAddressCollection(To)));
            if (Cc.Count > 0)
                message.Headers.Add(new Header("Cc", FormatAddressCollection(Cc)));
            if (!string.IsNullOrEmpty(Subject))
                message.Headers.Add(new Header("Subject", Subject));

            foreach (var attachment in Attachments)
            {
                message.Attachments.Add(new Message
                {
                    ContentType = attachment.ContentType,
                    Content = attachment.Content,
                    ContentEncoding = attachment.TextEncoding,
                    FileName = attachment.Filename
                });
            }

            return message.ToBytes();
        }

        private string[] FormatAddressCollection(List<MailAddress> addresses)
        {
            List<string> result = new List<string>();
            foreach (var addr in addresses)
            {
                if (!string.IsNullOrEmpty(addr.DisplayName))
                    result.Add(addr.DisplayName);
                result.Add("<" + addr.Address + ">,");
            }
            result[result.Count - 1] = result[result.Count - 1].Substring(0, result[result.Count - 1].Length - 1);
            return result.ToArray();
        }
    }
}