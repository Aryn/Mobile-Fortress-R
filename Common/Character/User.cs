using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Common.Character
{
    enum ControlState { Released, Pressed, Held }
    class User
    {
        string name = "User";
        NetConnection connection;
        //Mouse
        float pitch = 0;
        float yaw = 0;
        //Keyboard
        SortedDictionary<Keys, ControlState> controls = new SortedDictionary<Keys, ControlState>();

        public User(NetConnection connection, string name)
        {
            this.name = name;
            this.connection = connection;
        }

        public bool IsKeyDown(Keys K)
        {
            if (!controls.ContainsKey(K)) controls.Add(K, ControlState.Released);
            return controls[K] != ControlState.Released;
        }
        public bool WasKeyPressed(Keys K)
        {
            if (!controls.ContainsKey(K)) controls.Add(K, ControlState.Released);
            return controls[K] == ControlState.Pressed;
        }

        public Vector3 Forward { get { return Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(yaw, 0, pitch)); } }
        public Vector3 Left { get { return Vector3.Transform(Vector3.Left, Matrix.CreateFromYawPitchRoll(yaw, 0, pitch)); } }
        public Vector3 Right { get { return Vector3.Transform(Vector3.Right, Matrix.CreateFromYawPitchRoll(yaw, 0, pitch)); } }

        public override string ToString()
        {
            return name;
        }

        public void UpdateMouse(NetIncomingMessage msg)
        {
            pitch = msg.ReadFloat();
            yaw = msg.ReadFloat();
        }
        public void UpdateKeys(NetIncomingMessage msg)
        {
            var K = (Keys)msg.ReadUInt16();
            if (!controls.ContainsKey(K)) controls.Add(K, ControlState.Released);
            controls[K] = ControlState.Pressed;
        }

    }
}
