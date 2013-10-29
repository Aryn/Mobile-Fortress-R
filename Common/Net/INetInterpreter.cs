using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Net
{
    public interface INetInterpreter
    {
        void Interpret(Lidgren.Network.NetIncomingMessage msg);
    }
}
