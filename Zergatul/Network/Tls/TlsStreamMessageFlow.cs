using System;
using System.Collections.Generic;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    public partial class TlsStream
    {
        private class MessageFlow
        {
            public ConnectionState State;
            public MessageFlowCondition[] Flows;
            public bool IsServer;
            public bool IsClient;
            public Func<TlsStream, ContentMessage, bool> Read;
            public Action<TlsStream> Write;
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

            #region Conditions

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
                if (_tls.Role == Role.Server)
                    return false;
                return !IsServerKeyExchangeAfterServerCertificateValid();
            }

            public bool IsServerHelloDoneAfterServerCertificateValid()
            {
                if (_tls._reuseSession)
                    return false;
                return !_tls.SelectedCipher.KeyExchange.ShouldSendServerKeyExchange();
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

            #endregion

            #region Actions

            #region Write

            public void WriteClientHello()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateClientHello());
            }

            public void WriteServerHello()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateServerHello());
            }

            public void WriteServerCertificate()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateCertificate(_tls._serverCertificate));
            }

            public void WriteServerKeyExchange()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateServerKeyExchange());
            }

            public void WriteServerHelloDone()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateServerHelloDone());
            }

            public void WriteClientKeyExchange()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateClientKeyExchange());
            }

            public void WriteClientChangeCipherSpec()
            {
                _tls.WriteChangeCipherSpec();
                _tls.OnClientChangeCipherSpec();
            }

            public void WriteClientFinished()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateFinished());
            }

            public void WriteServerChangeCipherSpec()
            {
                _tls.WriteChangeCipherSpec();
                _tls.OnServerChangeCipherSpec();
            }

            public void WriteServerFinished()
            {
                _tls.WriteHandshakeMessageBuffered(_tls.GenerateFinished());
            }

            #endregion

            #region Read

            public bool ReadClientHello(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as ClientHello;
                if (message == null)
                    return false;

                _tls.OnClientHello(message);
                return true;
            }

            public bool ReadServerHello(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as ServerHello;
                if (message == null)
                    return false;

                _tls.OnServerHello(message);
                return true;
            }

            public bool ReadServerCertificate(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as Certificate;
                if (message == null)
                    return false;

                _tls.OnServerCertificate(message);
                return true;
            }

            public bool ReadServerKeyExchange(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as ServerKeyExchange;
                if (message == null)
                    return false;

                _tls.OnServerKeyExchange(message);
                return true;
            }

            public bool ReadCertificateRequest(ContentMessage cm)
            {
                return false;
            }

            public bool ReadServerServerHelloDone(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as ServerHelloDone;
                if (message == null)
                    return false;

                _tls.OnServerHelloDone(message);
                return true;
            }

            public bool ReadClientKeyExchange(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as ClientKeyExchange;
                if (message == null)
                    return false;

                _tls.OnClientKeyExchange(message);
                return true;
            }

            public bool ReadClientChangeCipherSpec(ContentMessage cm)
            {
                var message = cm as ChangeCipherSpec;
                if (message == null)
                    return false;

                _tls.OnClientChangeCipherSpec();
                return true;
            }

            public bool ReadClientFinished(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as Finished;
                if (message == null)
                    return false;

                _tls.OnClientFinished(message, true);
                return true;
            }

            public bool ReadServerChangeCipherSpec(ContentMessage cm)
            {
                var message = cm as ChangeCipherSpec;
                if (message == null)
                    return false;

                _tls.OnServerChangeCipherSpec();
                return true;
            }

            public bool ReadServerFinished(ContentMessage cm)
            {
                var message = (cm as HandshakeMessage)?.Body as Finished;
                if (message == null)
                    return false;

                _tls.OnServerFinished(message, true);
                return true;
            }

            #endregion

            #endregion
        }

        static readonly MessageFlow Tls12Flow;

        static MessageFlow InitTls12Flow()
        {
            var dict = new Dictionary<ConnectionState, MessageFlow>();
            dict.Add(ConnectionState.Start, new MessageFlow
            {
                State = ConnectionState.Start,
                IsClient = false,
                IsServer = false
            });
            dict.Add(ConnectionState.ClientHello, new MessageFlow
            {
                State = ConnectionState.ClientHello,
                IsClient = true,
                IsServer = false,
                Read = (tls, cm) => tls._flows.ReadClientHello(cm),
                Write = tls => tls._flows.WriteClientHello()
            });
            dict.Add(ConnectionState.ServerHello, new MessageFlow
            {
                State = ConnectionState.ServerHello,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerHello(cm),
                Write = tls => tls._flows.WriteServerHello(),
            });
            dict.Add(ConnectionState.ServerCertificate, new MessageFlow
            {
                State = ConnectionState.ServerCertificate,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerCertificate(cm),
                Write = tls => tls._flows.WriteServerCertificate(),
            });
            dict.Add(ConnectionState.ServerKeyExchange, new MessageFlow
            {
                State = ConnectionState.ServerKeyExchange,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerKeyExchange(cm),
                Write = tls => tls._flows.WriteServerKeyExchange(),
            });
            dict.Add(ConnectionState.CertificateRequest, new MessageFlow
            {
                State = ConnectionState.CertificateRequest,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadCertificateRequest(cm),
            });
            dict.Add(ConnectionState.ServerHelloDone, new MessageFlow
            {
                State = ConnectionState.ServerHelloDone,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerServerHelloDone(cm),
                Write = tls => tls._flows.WriteServerHelloDone(),
            });
            dict.Add(ConnectionState.ClientCertificate, new MessageFlow
            {
                State = ConnectionState.ClientCertificate,
                IsClient = true,
                IsServer = false
            });
            dict.Add(ConnectionState.ClientKeyExchange, new MessageFlow
            {
                State = ConnectionState.ClientKeyExchange,
                IsClient = true,
                IsServer = false,
                Read = (tls, cm) => tls._flows.ReadClientKeyExchange(cm),
                Write = tls => tls._flows.WriteClientKeyExchange()
            });
            dict.Add(ConnectionState.CertificateVerify, new MessageFlow
            {
                State = ConnectionState.CertificateVerify,
                IsClient = true,
                IsServer = false
            });
            dict.Add(ConnectionState.ClientChangeCipherSpec, new MessageFlow
            {
                State = ConnectionState.ClientChangeCipherSpec,
                IsClient = true,
                IsServer = false,
                Read = (tls, cm) => tls._flows.ReadClientChangeCipherSpec(cm),
                Write = tls => tls._flows.WriteClientChangeCipherSpec()
            });
            dict.Add(ConnectionState.ClientFinished, new MessageFlow
            {
                State = ConnectionState.ClientFinished,
                IsClient = true,
                IsServer = false,
                Read = (tls, cm) => tls._flows.ReadClientFinished(cm),
                Write = tls => tls._flows.WriteClientFinished()
            });
            dict.Add(ConnectionState.ServerChangeCipherSpec, new MessageFlow
            {
                State = ConnectionState.ServerChangeCipherSpec,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerChangeCipherSpec(cm),
                Write = tls => tls._flows.WriteServerChangeCipherSpec()
            });
            dict.Add(ConnectionState.ServerFinished, new MessageFlow
            {
                State = ConnectionState.ServerFinished,
                IsClient = false,
                IsServer = true,
                Read = (tls, cm) => tls._flows.ReadServerFinished(cm),
                Write = tls => tls._flows.WriteServerFinished()
            });
            dict.Add(ConnectionState.ApplicationData, new MessageFlow
            {
                State = ConnectionState.ApplicationData,
                IsClient = true,
                IsServer = true
            });
            dict.Add(ConnectionState.Closed, new MessageFlow
            {
                State = ConnectionState.Closed,
                IsClient = true,
                IsServer = true
            });

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
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.ServerHelloDone],
                    Condition = tls => tls._flows.IsServerHelloDoneAfterServerCertificateValid()
                }
            };
            dict[ConnectionState.ServerKeyExchange].Flows = new[]
            {
                new MessageFlowCondition
                {
                    Next = dict[ConnectionState.CertificateRequest],
                    Condition = tls => false
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