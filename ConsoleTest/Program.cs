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

            var rnd = new Random();
            Func<int, string> rndNumber = (int len) =>
            {
                string result = "";
                for (int i = 0; i < len; i++)
                    result += (char)(rnd.Next(10) + (int)'0');
                return result;
            };

            var bi1 = new BigInteger[10000];
            var bi2 = new BigInteger[10000];
            for (int i = 0; i < bi1.Length; i++)
            {
                string num1 = rndNumber(1000 + rnd.Next(1000));
                string num2 = rndNumber(1000 + rnd.Next(1000));
                bi1[i] = new BigInteger(num1);
                bi2[i] = new BigInteger(num2);
            }

            for (BigInteger.KaratsubaBitLen = 5; BigInteger.KaratsubaBitLen < 50; BigInteger.KaratsubaBitLen++)
            {
                var sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < 10000; i++)
                    bi1[i].Multiply(bi2[i]);
                sw.Stop();

                Console.WriteLine($"{BigInteger.KaratsubaBitLen}: {sw.ElapsedMilliseconds}ms");
            }

            Console.ReadLine();
        }
    }
}