using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zergatul.Network.Mime
{
    public class Message
    {
        private static readonly Regex _headerRegex = new Regex(@"^[a-zA-Z0-9\-]+$");

        public string ContentType { get; set; }
        public string ContentTransferEncoding { get; set; } = Mime.ContentTransferEncoding.Base64;

        public List<Header> Headers { get; private set; } = new List<Header>();
        public List<Message> Attachments { get; private set; } = new List<Message>();

        public Encoding ContentEncoding { get; set; } = Encoding.UTF8;
        public Encoding HeadersEncoding { get; set; } = Encoding.UTF8;

        public string FileName { get; set; }
        public byte[] Content { get; set; }

        private List<byte> _bytes;
        private string _boundary;

        public byte[] ToBytes()
        {
            _bytes = new List<byte>();
            AppendHeader("MIME-Version", "1.0");

            ToBytesSimple();

            return _bytes.ToArray();
        }

        private void ToBytesSimple()
        {
            if (Attachments.Count > 0)
            {
                ProcessMixedContentHeaders();
                ProcessCustomHeaders();
                EndHeaders();

                AppendString("--" + _boundary + Constants.TelnetEndOfLine);
                ProcessContentTypeHeaders();
                AppendHeader("Content-Disposition", "inline");
                EndHeaders();
                ProcessContentData();
                AppendString(Constants.TelnetEndOfLine);

                foreach (var attachment in Attachments)
                {
                    AppendString("--" + _boundary + Constants.TelnetEndOfLine);
                    attachment._bytes = new List<byte>();
                    attachment.ProcessAttachmentHeader();
                    attachment.ToBytesSimple();
                    _bytes.AddRange(attachment._bytes);
                    AppendString(Constants.TelnetEndOfLine);
                }

                AppendString("--" + _boundary + "--" + Constants.TelnetEndOfLine);
            }
            else
            {
                ProcessContentTypeHeaders();
                ProcessCustomHeaders();
                EndHeaders();
                ProcessContentData();
            }
        }

        private void ProcessMixedContentHeaders()
        {
            _boundary = BitHelper.BytesToHex(Guid.NewGuid().ToByteArray().Take(16).ToArray());
            AppendHeader("Content-Type", Mime.ContentType.Multipart.Mixed, new HeaderAttribute("boundary", _boundary));
        }

        private void ProcessContentTypeHeaders()
        {
            if (string.IsNullOrEmpty(ContentType))
                throw new InvalidOperationException("ContentType is empty");

            if (ContentType == Mime.ContentType.Text.Plain || ContentType == Mime.ContentType.Text.Html)
            {
                if (ContentEncoding == null)
                    throw new InvalidOperationException("ContentEncoding cannot be null for text content type");
                AppendHeader("Content-Type", ContentType, new HeaderAttribute("charset", ContentEncoding.BodyName));
            }
            else
                AppendHeader("Content-Type", ContentType);

            if (string.IsNullOrEmpty(ContentTransferEncoding))
                throw new InvalidOperationException("ContentTransferEncoding is empty");
            AppendHeader("Content-Transfer-Encoding", ContentTransferEncoding);
        }

        private void ProcessCustomHeaders()
        {
            foreach (var header in Headers)
                AppendHeader(header.Key, header.Values);
        }

        private void EndHeaders()
        {
            AppendString(Constants.TelnetEndOfLine);
        }

        private void ProcessContentData()
        {
            switch (ContentTransferEncoding)
            {
                case Mime.ContentTransferEncoding.Base64:
                    if (Content?.Length > 0)
                        AppendString(Convert.ToBase64String(Content));
                    break;
                case Mime.ContentTransferEncoding.Binary:
                    if (Content?.Length > 0)
                        _bytes.AddRange(Content);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ProcessAttachmentHeader()
        {
            if (!string.IsNullOrEmpty(FileName))
                AppendHeader("Content-Disposition", "attachment", new HeaderAttribute("filename", FileName));
            else
                AppendHeader("Content-Disposition", "attachment");
        }

        private void AppendHeader(string header, string[] values, params HeaderAttribute[] attributes)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException(nameof(header));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Any(v => string.IsNullOrEmpty(v)))
                throw new ArgumentException(nameof(values));

            if (!_headerRegex.IsMatch(header))
                throw new ArgumentException("Invalid character in header", nameof(header));

            if (attributes != null)
                foreach (var attr in attributes)
                    if (!_headerRegex.IsMatch(attr.Key))
                        throw new ArgumentException("Invalid character in attribute key", nameof(attributes));

            _bytes.AddRange(Encoding.ASCII.GetBytes(header + ":"));
            foreach (var value in values)
            {
                if (EncodingHelper.IsValidHeaderValueString(value, true))
                    _bytes.AddRange(Encoding.ASCII.GetBytes(" " + value));
                else
                    _bytes.AddRange(Encoding.ASCII.GetBytes(" " + EncodingHelper.EncodedWordsEncode(value, HeadersEncoding)));
            }

            if (attributes != null)
                foreach (var attr in attributes)
                {
                    if (EncodingHelper.IsValidHeaderValueString(attr.Value, false))
                        _bytes.AddRange(Encoding.ASCII.GetBytes("; " + attr.Key + "=" + attr.Value));
                    else
                        _bytes.AddRange(Encoding.ASCII.GetBytes("; " + attr.Key + "*=" + EncodeAttributeValue(attr.Value, HeadersEncoding)));
                }

            _bytes.AddRange(Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine));
        }

        private void AppendHeader(string header, string value, params HeaderAttribute[] attributes)
        {
            AppendHeader(header, new[] { value }, attributes);
        }

        private string EncodeAttributeValue(string value, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(value);
            return encoding.BodyName + "''" + string.Join("", bytes.Select(b => "%" + b.ToString("x2").PadLeft(2, '0')));
        }

        private void AppendString(string data)
        {
            _bytes.AddRange(Encoding.ASCII.GetBytes(data));
        }
    }
}