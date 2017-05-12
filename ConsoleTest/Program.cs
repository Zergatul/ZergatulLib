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
            /*var tcp = new TcpClient("localhost", 2828);
            TlsStream stream = new TlsStream(tcp.GetStream());
            stream.AuthenticateAsClient("localhost");*/
            /*BigInteger bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Console.WriteLine(bi.ToString());
            Console.ReadLine();*/

            /*string num1 = "08396905339595156620";
            string num2 = "13568775824401428918";
            var res2 = new BigInteger(num1) * new BigInteger(num2);
            Console.WriteLine(res2.ToString());
            return;*/

            tt();
            return;

            var rnd = new Random();
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
                string num1 = rndNumber(20);
                string num2 = rndNumber(10);
                var bi1 = new BigInteger(num1);
                var bi2 = new BigInteger(num2);
                var si1 = System.Numerics.BigInteger.Parse(num1);
                var si2 = System.Numerics.BigInteger.Parse(num2);
                System.Numerics.BigInteger rem;
                var sres = System.Numerics.BigInteger.DivRem(si1, si2, out rem);
                var res = bi1.Division(bi2);
                if (res.Item1.ToString() != sres.ToString())
                {
                    Console.WriteLine("Div failed:");
                    Console.WriteLine(num1);
                    Console.WriteLine(num2);
                    break;
                }
                if (res.Item2.ToString() != rem.ToString())
                {
                    Console.WriteLine("Mod failed:");
                    Console.WriteLine(num1);
                    Console.WriteLine(num2);
                    break;
                }

                count++;
                Console.WriteLine("OK");
            }

            Console.ReadLine();
        }

        private static void tt()
        {
            string num1 = "32999900863646250090";
            string num2 = "9341814507";

            var bi1 = new BigInteger(num1);
            var bi2 = new BigInteger(num2);

            var div = System.Numerics.BigInteger.Parse(num1) / System.Numerics.BigInteger.Parse(num2);
            var mod = System.Numerics.BigInteger.Parse(num1) % System.Numerics.BigInteger.Parse(num2);

            var res = bi1.Division(bi2);
        }
    }
}