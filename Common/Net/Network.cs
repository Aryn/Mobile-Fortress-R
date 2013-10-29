using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Common.Character;

namespace Common.Net
{
    public enum DataType {Creation, Deletion, Positional, Sector}
    /* Network - Handles all network transmissions between clients and servers.
     * Requirements:
     *  - Can handle both client and server incoming and outgoing data.
     *  - Later, can connect to the DB to get user data.
     */
    public class Network
    {
        const string remoteServer = "localhost";
        const int remotePort = 5000;

        public Action onConnect;
        public Action onConnectionFail;

        INetInterpreter interpreter;

        Dictionary<long, User> lusers = new Dictionary<long, User>();

        NetPeer root;
        public Network(bool isClient, INetInterpreter interpreter)
        {
            this.interpreter = interpreter;
            NetPeerConfiguration config = new NetPeerConfiguration("Mobile Fortress");
            config.AcceptIncomingConnections = true;
            config.Port = 5000;
            if (isClient)
            {
                config.Port = 5100;
                root = new Lidgren.Network.NetClient(config);
            }
            else
            {
                root = new Lidgren.Network.NetServer(config);
            }
            root.Start();
            Console.WriteLine("Mobile Fortress " + (isClient?"Client":"Server") + " Online");
            Console.WriteLine("UID: " + root.UniqueIdentifier.ToString("X"));
        }
        public void Shutdown()
        {
            root.Shutdown("Shut down by program.");
        }

        public bool TryConnect()
        {
            root.Connect(remoteServer, remotePort);
            return true;
        }

        internal void AddToUsers(NetConnection C)
        {
            
            var U = new User(C, C.RemoteUniqueIdentifier.ToString("X"));
            Console.WriteLine("Adding user: " + U);
            lusers.Add(C.RemoteUniqueIdentifier, U);
            Atlas.GetSector(0, 0).SendCreation(C);
        }

        public NetOutgoingMessage Outgoing()
        {
            return root.CreateMessage();
        }
        public void Cycle()
        {
            NetIncomingMessage msg;
            while ((msg = root.ReadMessage()) != null)
            {
                Cycle(msg);
            }
        }

        public void Cycle(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Console.WriteLine(msg.ReadString());
                    break;
                default:
                    interpreter.Interpret(msg);
                    break;
            }
            root.Recycle(msg);
        }
    }
}
