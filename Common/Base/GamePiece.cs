using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Common.Net;
using Lidgren.Network;

namespace Common.Base
{
    /* GamePiece - Parent class of all objects in a sector, including particles and anything controlled by physics.
     * Requirements:
     * - Has a position, rotation and velocity.
     * - Can send these over the network with one function call, ideally automated. (Server)
     * - Position, rotation and velocity have an error of < 1.5 under standard conditions. (Client)
     */
    public abstract class GamePiece
    {
        static Random idrng = new Random();
        static byte[] tmpbytes = new byte[8];
        public static ulong CreateNewID()
        {
            idrng.NextBytes(tmpbytes);
            return BitConverter.ToUInt64(tmpbytes, 0);
        }

        public static TGamePiece ReadCreation<TGamePiece>(NetIncomingMessage msg)
            where TGamePiece : GamePiece
        {
            string typename = msg.ReadString();
            Type t = Type.GetType(typename);
            if (t == null) throw new NetException();
            GamePiece P = (GamePiece)t.GetConstructor(new Type[0]).Invoke(new object[0]);
            P.id = msg.ReadUInt64();
            P.ReadData(msg);
            return (TGamePiece)P;
        }
        

        ulong id = 0xFFFFFFFFFFFFFFFF;
        public ulong ID() { return id; }

        internal void CreateOnServer()
        {
            id = CreateNewID();
        }
        internal void WriteCreation(NetOutgoingMessage msg)
        {
            msg.Write(GetType().Name);
            msg.Write(id);
            WriteData(msg);
        }
        protected abstract void WriteData(NetOutgoingMessage msg);
        protected abstract void ReadData(NetIncomingMessage msg);

        protected abstract void WriteUpdateData(NetOutgoingMessage msg);

        internal void WriteUpdate(NetOutgoingMessage msg)
        {
            msg.Write(id);
            msg.Write(Position());
            msg.WriteRotation(Rotation(), 16);
            msg.Write(Velocity());
            WriteUpdateData(msg);
        }
        public void ReadUpdate(NetIncomingMessage msg, float dt)
        {
            var pos = msg.ReadVector3();
            var rot = msg.ReadRotation(16);
            var vel = msg.ReadVector3();

            SetPRV(pos + vel * dt, rot, vel);
            ReadUpdateData(msg, dt);
        }
        public abstract void ReadUpdateData(NetIncomingMessage msg, float dt);
        public abstract void SetPRV(Vector3 pos, Quaternion rot, Vector3 vel);

        public abstract Vector3 Position();
        public abstract Quaternion Rotation();
        public abstract Vector3 Velocity();
        public Matrix World()
        {
            return Matrix.CreateFromQuaternion(Rotation())*Matrix.CreateTranslation(Position());
        }
        public abstract void Update(float dt);

        public override string ToString()
        {
            return GetType().Name + ": " + id.ToString("X");
        }
    }
}
