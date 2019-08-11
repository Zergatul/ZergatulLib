using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Security.Zergatul.Tls
{
    struct StateMachine
    {
        public MessageFlowState State;
        public ReadState RState;
        public WriteState WState;
        public HandshakeState HState;

        public void ResetServer()
        {
            HState = HandshakeState.ServerReadClientHello;
            State = MessageFlowState.Reading;
            RState = ReadState.ReadHeader;
        }

        public void ProcessRead()
        {

        }
    }
}