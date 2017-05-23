using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Ftp;
using Zergatul.Math;
using Zergatul.Net.Proxy;
using Zergatul.Net.Tls;

namespace Test
{
    class Program
    {
        // TODO: test with slow connection
        // TODO: check ipv6
        // http proxy parse response
        // implicit/exp ftps
        // exception texts to resources
        // https://www.chilkatsoft.com/ftp-2-dotnet.asp
        // http://xceed.com/FTP_NET_Features.html
        // https://www.rebex.net/ftp-ssl.net/
        // http://www.dart.com/ftp-control-features.aspx

        static void Main(string[] args)
        {
            var tcp = new TcpClient("localhost", 32028);

            /*TlsStream stream = new TlsStream(tcp.GetStream());
            stream.AuthenticateAsClient("localhost");*/

            SslStream stream = new SslStream(tcp.GetStream(), false, (a1, a2, a3, a4) => true);
            stream.AuthenticateAsClient("localhost", null, System.Security.Authentication.SslProtocols.Tls12, false);
            Console.WriteLine("**************************************************");
            Console.WriteLine(stream.CipherAlgorithm);
            Console.WriteLine(stream.CipherStrength);
            Console.WriteLine(stream.HashAlgorithm);
            Console.WriteLine(stream.HashStrength);
            Console.WriteLine(stream.KeyExchangeAlgorithm);
            Console.WriteLine(stream.KeyExchangeStrength);
            Console.WriteLine("**************************************************");
            Console.WriteLine(new StreamReader(stream).ReadToEnd());
            stream.Close();
            Console.ReadLine();

            /*BigInteger bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Console.WriteLine(bi.ToString());
            Console.ReadLine();

            /*string num1 = "08396905339595156620";
            string num2 = "13568775824401428918";
            var res2 = new BigInteger(num1) * new BigInteger(num2);
            Console.WriteLine(res2.ToString());
            return;*/

            /*tt();
            return;*/

            /*var rnd = new Random();
            Func<int, string> rndNumber = (int len) =>
            {
                string result = "";
                for (int i = 0; i < len; i++)
                    result += (char)(rnd.Next(10) + (int)'0');
                return result;
            };

            int count = 0;
            while (true)
            {
                string num1 = rndNumber(10 + rnd.Next(2));
                string num2 = rndNumber(10 + rnd.Next(2));
                string num3 = rndNumber(12 + rnd.Next(2));
                var bi1 = new BigInteger(num1);
                var bi2 = new BigInteger(num2);
                var bi3 = new BigInteger(num3);
                var si1 = System.Numerics.BigInteger.Parse(num1);
                var si2 = System.Numerics.BigInteger.Parse(num2);
                var si3 = System.Numerics.BigInteger.Parse(num3);

                var res1 = BigInteger.ModularExponentiation(bi1, bi2, bi3);
                var res2 = System.Numerics.BigInteger.ModPow(si1, si2, si3);

                if (res1.ToString() != res2.ToString())
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(num1);
                    Console.WriteLine(num2);
                    Console.WriteLine(num3);
                    break;
                }

                count++;
                if (count % 1 == 0)
                    Console.WriteLine($"{count} OK");
            }

            Console.ReadLine();*/
        }

        private static void tt()
        {
            string num1 = "163338342975806799406";
            string num2 = "02827803280063344426";
            string num3 = "76224379996895040153";

            var bi1 = new BigInteger(num1);
            var bi2 = new BigInteger(num2);
            var bi3 = new BigInteger(num3);
            Func<string, System.Numerics.BigInteger> parse = s => System.Numerics.BigInteger.Parse(s);

            var res1 = System.Numerics.BigInteger.ModPow(parse(num1), parse(num2), parse(num3));
            var res2 = BigInteger.ModularExponentiation(bi1, bi2, bi3);
        }
    }
}