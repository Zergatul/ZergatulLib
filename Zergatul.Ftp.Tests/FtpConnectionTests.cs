using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Net;
using Zergatul.Network;
using Zergatul.Network.Ftp;

namespace Zergatul.Ftp.Tests
{
    [TestClass]
    public class FtpConnectionTests
    {
        [TestMethod]
        public void Connect_ByHostIPv4()
        {
            try
            {
                var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
                server.Start();
                var con = new FtpConnection();
                con.Connect("localhost", 60028);
                con.Dispose();
                server.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [TestMethod]
        public void Connect_ByHostIPv6()
        {
            try
            {
                var server = new SimpleServer(AddressFamily.InterNetworkV6, 60028);
                server.Start();
                var con = new FtpConnection();
                con.PreferIPv4 = false;
                con.Connect("localhost", 60028);
                con.Dispose();
                server.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [TestMethod]
        public void Connect_ByAddressIPv4()
        {
            try
            {
                var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
                server.Start();
                var con = new FtpConnection();
                con.Connect(IPAddress.Parse("127.0.0.1"), 60028);
                con.Dispose();
                server.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [TestMethod]
        public void Connect_ByAddressIPv6()
        {
            try
            {
                var server = new SimpleServer(AddressFamily.InterNetworkV6, 60028);
                server.Start();
                var con = new FtpConnection();
                con.Connect(IPAddress.Parse("::1"), 60028);
                con.Dispose();
                server.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message, ex);
            }
        }

        [TestMethod]
        public void NoConnectionBeforeCommand()
        {
            var con = new FtpConnection();

            Action<Action> check = (a) =>
                {
                    try
                    {
                        a();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message, ex);
                    }
                };

            check(() => con.Login("", ""));
            check(() => con.Account(""));
            check(() => con.Reinitialize());
            check(() => con.Quit());
            check(() => con.AuthTls());
            check(() => con.ProtectionBufferSize(0));
            check(() => con.DataChannelProtectionLevel(FtpDataChannelProtectionLevel.Clear));
            check(() => con.ClearCommandChannel());
            check(() => con.ChangeWorkingDirectory(""));
            check(() => con.ChangeToParentDirectory());
            check(() => con.EnterPassiveMode());
            check(() => con.EnterPassiveModeEx());
            check(() => con.EnterActiveMode(IPAddress.None, 0));
            check(() => con.EnterActiveModeEx(IPAddress.None, 0));
            check(() => con.SetTransferMode(FtpTransferMode.Block));
            check(() => con.SetRepresentationType("", ""));
            check(() => con.RetrieveFile("", null));
            check(() => con.StoreFile("", null));
            check(() => con.AppendFile("", null));
            check(() => con.List(""));
            check(() => con.NameList(""));
            check(() => con.DeleteFile(""));
            check(() => con.MakeDirectory(""));
            check(() => con.RemoveDirectory(""));
            check(() => con.MachineListingSingle(""));
            check(() => con.MachineListingMany(""));
            check(() => con.System());
            check(() => con.Help(""));
            check(() => con.Features());
            check(() => con.Site(""));
            check(() => con.Noop());
            check(() => con.Quit());
            check(() => con.Quit());
        }

        [TestMethod]
        public void MultilineReplyParse()
        {
            var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
            server.Greeting =
                "220-First line" + Constants.TelnetEndOfLine +
                "Second line" + Constants.TelnetEndOfLine +
                "221  A line beginning with numbers" + Constants.TelnetEndOfLine +
                "220 The last line";
            server.Start();
            var con = new FtpConnection();
            con.Connect("localhost", 60028);
            con.Dispose();
            server.Dispose();

            Assert.AreEqual(con.Greeting, server.Greeting);
        }

        [TestMethod]
        public void Login_Simple()
        {
            var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
            string user = null;
            server.ReplyFunction = (cmd) =>
                {
                    if (cmd.StartsWith("USER "))
                    {
                        user = cmd.Substring(5);
                        return "331 User name okay, need password.";
                    }
                    if (cmd.StartsWith("PASS "))
                    {
                        string pass = cmd.Substring(5);
                        if (user == "user" && pass == "123456")
                            return "230 User logged in, proceed.";
                        else
                            return "530 Not logged in.";
                    }
                    throw new NotImplementedException();
                };
            server.Start();
            var con = new FtpConnection();
            con.Connect("localhost", 60028);
            con.Login("user", "123456");
            con.Dispose();
            server.Dispose();
        }

        [TestMethod]
        public void Login_InvalidPass()
        {
            var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
            string user = null;
            server.ReplyFunction = (cmd) =>
            {
                if (cmd.StartsWith("USER "))
                {
                    user = cmd.Substring(5);
                    return "331 User name okay, need password.";
                }
                if (cmd.StartsWith("PASS "))
                {
                    string pass = cmd.Substring(5);
                    if (user == "user" && pass == "123456")
                        return "230 User logged in, proceed.";
                    else
                        return "530 Not logged in.";
                }
                throw new NotImplementedException();
            };
            server.Start();
            var con = new FtpConnection();
            con.Connect("localhost", 60028);
            try
            {
                con.Login("user", "1234567");
                Assert.Fail();
            }
            catch (FtpServerException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            con.Dispose();
            server.Dispose();
        }

        [TestMethod]
        public void Login_InvalidUser()
        {
            var server = new SimpleServer(AddressFamily.InterNetwork, 60028);
            server.ReplyFunction = (cmd) =>
            {
                if (cmd.StartsWith("USER "))
                    return "530 Invalid user";
                throw new NotImplementedException();
            };
            server.Start();
            var con = new FtpConnection();
            con.Connect("localhost", 60028);
            try
            {
                con.Login("user", "1234567");
                Assert.Fail();
            }
            catch (FtpServerException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            con.Dispose();
            server.Dispose();
        }
    }
}