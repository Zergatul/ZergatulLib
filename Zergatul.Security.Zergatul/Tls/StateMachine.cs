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
        public HandshakeState HState;

        public void Reset()
        {
            State = MessageFlowState.Init;
            HState = HandshakeState.Init;
        }
    }
}