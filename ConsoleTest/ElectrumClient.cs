using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls;

namespace Test
{
    class ElectrumClient
    {
        private string _host;
        private int _port;
        private bool _tls;
        private Stream _stream;
        private int _id;

        public ElectrumClient(string host, int port, bool tls = true)
        {
            this._host = host;
            this._port = port;
            this._tls = tls;
        }

        public void Connect()
        {
            Console.Write("Connecting... ");
            var client = new TcpClient(_host, _port);
            Console.WriteLine("OK");

            if (_tls)
            {
                Console.Write("Begin TLS handshake... ");
                var tlsStream = new TlsStream(client.GetStream());
                tlsStream.Settings.ServerCertificateValidationOverride = cert => true;
                tlsStream.AuthenticateAsClient(_host);
                Console.WriteLine("OK");

                _stream = tlsStream;
            }
            else
            {
                _stream = client.GetStream();
            }

            _id = 1;
        }

        public void Close()
        {
            _stream.Close();
        }

        public string GetRawTransaction(string id)
        {
            return ExecuteMethod("blockchain.transaction.get", id, false);
        }

        public string BroadcastTransaction(string raw)
        {
            return ExecuteMethod("blockchain.transaction.broadcast", raw);
        }

        private string ExecuteMethod(string method, params object[] parameters)
        {
            var json = new Newtonsoft.Json.Linq.JObject();
            json["jsonrpc"] = "2.0";
            json["method"] = method;
            json["params"] = new Newtonsoft.Json.Linq.JArray(parameters);
            json["id"] = _id.ToString();

            string jsonStr = json.ToString(Newtonsoft.Json.Formatting.None) + "\n";
            byte[] jsonBytes = Encoding.ASCII.GetBytes(jsonStr);

            Console.Write("Write command... ");
            _stream.Write(jsonBytes, 0, jsonBytes.Length);
            Console.WriteLine("OK");

            List<byte> resp = new List<byte>();
            byte[] buffer = new byte[1024];
            string respStr = null;
            Console.WriteLine("Waiting for response...");
            while (true)
            {
                int read = _stream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                    break;
                Console.WriteLine("Read: " + read);
                resp.AddRange(buffer.Take(read));

                respStr = Encoding.ASCII.GetString(resp.ToArray());
                if (respStr.EndsWith("\n"))
                    break;
            }

            return respStr;
        }
    }
}