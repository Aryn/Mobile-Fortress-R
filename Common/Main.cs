using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Lidgren.Network;
using Common.Net;
using Microsoft.Xna.Framework;

namespace Common
{
    class Server : INetInterpreter, IDisposable
    {
        #region Shutdown
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    Net.Shutdown();
                    return false;
            }
        }
        #endregion
        const double programSpeed = 1d / 30d;
        static double lastUpdate = NetTime.Now;

        public static Network Net;

#if DEBUG
        static Sector tsec = new Sector(new Point(0, 1));
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            double time = NetTime.Now;
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            using(Server server = new Server())
            {
                server.Initialize();
                while (true)
                {
                    time = NetTime.Now;
                    server.Update(time);
                }
            }
        }

        void Initialize()
        {
            Console.WriteLine("Mobile Fortress Server Alpha Build 1");
            Net = new Network(false, this);
        }

        void Update(double time)
        {
            if (time > lastUpdate)
            {
                Net.Cycle();
                lastUpdate += programSpeed;
            }
        }

        public void Interpret(NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                    break;
                case NetIncomingMessageType.StatusChanged:
                    var status = (NetConnectionStatus)msg.ReadByte();
                    switch (status)
                    {
                        case NetConnectionStatus.Connected:
                            Console.WriteLine("Successful Connection: " + msg.SenderEndPoint.Address);
                            Net.AddToUsers(msg.SenderConnection);
                            break;
                        default:
                            Console.WriteLine(status + ": " + msg.SenderEndPoint.Address);
                            break;
                    }
                    break;
                case NetIncomingMessageType.Data:
                    break;
            }
        }

        public void Dispose()
        {
            Net.Shutdown();
        }
    }
}
