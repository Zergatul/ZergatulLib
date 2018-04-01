using System;
using System.Collections.Generic;

namespace Zergatul.Network.Tls
{
    public partial class TlsStream
    {
        private class MessageFlow
        {
            public ConnectionState State;
            public MessageFlowCondition[] Flows;
        }

        private class MessageFlowCondition
        {
            public MessageFlow Next;
            public Func<TlsStream, bool> Condition;
        }

        private class Flows
        {
            private TlsStream _tls;

            public Flows(TlsStream tls)
            {
                this._tls = tls;
            }

            public bool IsServerCertificateAfterServerHelloValid()
            {
                if (_tls._reuseSession)
                    return false;
                if (_tls.SelectedCipher.KeyExchange.ServerCertificateMessage == MessageInfo.Forbidden)
                    return false;
                if (_tls.SelectedCipher.KeyExchange.ServerCertificateMessage == MessageInfo.Required)
                    return true;
                throw new InvalidOperationException();
            }

            public bool IsServerKeyExchangeAfterServerHelloValid()
            {
                if (_tls._reuseSession)
                    return false;
                if (IsServerCertificateAfterServerHelloValid())
                    return false;
                return _tls.SelectedCipher.KeyExchange.ShouldSendServerKeyExchange();
            }

            public bool IsServerHelloDoneAfterServerHelloValid()
            {
                if (_tls._reuseSession)
                    return false;
                return
                    !IsServerCertificateAfterServerHelloValid() &&
                    !IsServerKeyExchangeAfterServerHelloValid();
            }

            public bool IsServerKeyExchangeAfterServerCertificateValid()
            {
                if (_tls._reuseSession)
                    return false;
                return _tls.SelectedCipher.KeyExchange.ShouldSendServerKeyExchange();
            }

            public bool IsCertificateRequestAfterServercertificateValid()
            {
                if (_tls._reuseSession)
                    return false;
                return !IsServerKeyExchangeAfterServerCertificateValid();
            }

            public bool IsClientCertificateAfterServerHelloDoneValid()
            {
                if (_tls._reuseSession)
                    return false;
                return _tls.SecurityParameters.CertificateRequested;
            }

            public bool IsClientKeyExchangeAfterServerHelloDoneValid()
            {
                if (_tls._reuseSession)
                    return false;
                return !_tls.SecurityParameters.CertificateRequested;
            }

            public bool IsCertificateVerifyAfterClientKeyExchangeValid()
            {
                if (_tls._reuseSession)
                    return false;
                return _tls.SecurityParameters.CertificateRequested;
            }

            public bool IsClientCCSAfterClientKeyExchangeValid()
            {
                if (_tls._reuseSession)
                    return false;
                return !_tls.SecurityParameters.CertificateRequested;
            }

            public bool IsAppDataAfterServerFinishedValid()
            {
                return !_tls._reuseSession;
            }

            public bool IsServerCCSAfterServerHelloValid()
            {
                return _tls._reuseSession;
            }

            public bool IsClientCCSAfterServerFinishedValid()
            {
                return _tls._reuseSession;
            }

            public bool IsServerCCSAfterClientFinishedValid()
            {
                return !_tls._reuseSession;
            }

            public bool IsAppDataAfterClientFinishedValid()
            {
                return _tls._reuseSession;
            }
        }

        static readonly MessageFlow Tls12Flow;

        static MessageFlow InitTls12Flow()
        {
            var dict = new Dictionary<ConnectionState, MessageFlow>();
            dict.Add(ConnectionState.Start, new MessageFlow { State = ConnectionState.Start });
            dict.Add(ConnectionState.ClientHello, new MessageFlow { State = ConnectionState.ClientHello });
            dict.Add(ConnectionState.ServerHello, new MessageFlow { State = ConnectionState.ServerHello });
            dict.Add(ConnectionState.ServerCertificate, new MessageFlow { State = ConnectionState.ServerCertificate });
            dict.Add(ConnectionState.ServerKeyExchange, new MessageFlow { State = ConnectionState.ServerKeyExchange });
            dict.Add(ConnectionState.CertificateRequest, new MessageFlow { State = ConnectionState.CertificateRequest });
            dict.Add(ConnectionState.ServerHelloDone, new MessageFlow { State = ConnectionState.ServerHelloDone });
            dict.Add(ConnectionState.ClientCertificate, new MessageFlow { State = ConnectionState.ClientCertificate });
            dict.Add(ConnectionState.ClientKeyExchange, new MessageFlow { State = ConnectionState.ClientKeyExchange });
            dict.Add(ConnectionState.ClientChangeCipherSpec, new MessageFlow { State = ConnectionState.ClientChangeCipherSpec });
            dict.Add(ConnectionState.ClientFinished, new MessageFlow { State = ConnectionState.ClientFinished });
            dict.Add(ConnectionState.ServerChangeCipherSpec, new MessageFlow { State = ConnectionState.ServerChangeCipherSpec });
            dict.Add(ConnectionState.ServerFinished, new MessageFlow { State = ConnectionState.ServerFinished });
            dict.Add(ConnectionState.ApplicationData, new MessageFlow { State = ConnectionState.ApplicationData });
            dict.Add(ConnectionState.Closed, new MessageFlow { State = ConnectionState.Closed });

            dict[ConnectionState.Start].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientHello],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ClientHello].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerHello],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ServerHello].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerCertificate],
                    Condition = tls => tls._flows.IsServerCertificateAfterServerHelloValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerKeyExchange],
                    Condition = tls => tls._flows.IsServerKeyExchangeAfterServerHelloValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerHelloDone],
                    Condition = tls => tls._flows.IsServerHelloDoneAfterServerHelloValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerChangeCipherSpec],
                    Condition = tls => tls._flows.IsServerCCSAfterServerHelloValid()
                }
            };
            dict[ConnectionState.ServerCertificate].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerKeyExchange],
                    Condition = tls => tls._flows.IsServerKeyExchangeAfterServerCertificateValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.CertificateRequest],
                    Condition = tls => tls._flows.IsCertificateRequestAfterServercertificateValid()
                },
            };
            dict[ConnectionState.ServerKeyExchange].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.CertificateRequest],
                    Condition = tls => true
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerHelloDone],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ServerHelloDone].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientCertificate],
                    Condition = tls => tls._flows.IsClientCertificateAfterServerHelloDoneValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientKeyExchange],
                    Condition = tls => tls._flows.IsClientKeyExchangeAfterServerHelloDoneValid()
                }
            };
            dict[ConnectionState.ClientCertificate].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientKeyExchange],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ClientKeyExchange].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.CertificateVerify],
                    Condition = tls => tls._flows.IsCertificateVerifyAfterClientKeyExchangeValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientChangeCipherSpec],
                    Condition = tls => tls._flows.IsClientCCSAfterClientKeyExchangeValid()
                }
            };
            dict[ConnectionState.CertificateVerify].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientChangeCipherSpec],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ClientChangeCipherSpec].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientFinished],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ClientFinished].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerChangeCipherSpec],
                    Condition = tls => tls._flows.IsServerCCSAfterClientFinishedValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ApplicationData],
                    Condition = tls => tls._flows.IsAppDataAfterClientFinishedValid()
                }
            };
            dict[ConnectionState.ServerChangeCipherSpec].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerFinished],
                    Condition = tls => true
                }
            };
            dict[ConnectionState.ServerFinished].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ClientChangeCipherSpec],
                    Condition = tls => tls._flows.IsClientCCSAfterServerFinishedValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ApplicationData],
                    Condition = tls => tls._flows.IsAppDataAfterServerFinishedValid()
                },
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.Closed],
                    Condition = tls => tls._flows.IsAppDataAfterServerFinishedValid()
                }
            };
            dict[ConnectionState.ApplicationData].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.Closed],
                    Condition = tls => true
                }
            };

            return dict[ConnectionState.Start];
        }

        static TlsStream()
        {
            Tls12Flow = InitTls12Flow();
        }
    }
}