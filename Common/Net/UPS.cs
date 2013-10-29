using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Common.Net
{
    enum PacketTypes { Creation, Deletion, Update, Controls, Keypress, ShipData } 
    class UPS
    {
        public void WritePacket()
        {
            NetOutgoingMessage msg = Server.Net.Outgoing();
        }
    }
}
