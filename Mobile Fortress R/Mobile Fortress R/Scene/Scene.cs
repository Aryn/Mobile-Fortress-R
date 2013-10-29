using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Net;
using Nuclex.UserInterface;

namespace Mobile_Fortress_R
{
    public abstract class Scene
    {
        public abstract Screen LoadGUI();
        public abstract void DisposeGUI();
        public abstract void Update(float dt);
        public abstract void Render();
        public virtual void Interpret(Lidgren.Network.NetIncomingMessage msg) { throw new NotImplementedException(); }
        public virtual void OnConnect() { throw new NotImplementedException(); }
        public virtual void OnConnectionFail() { throw new NotImplementedException(); }
        public virtual void OnDisconnect() { throw new NotImplementedException(); }
        public int Width { get { return MobileFortress.GraphicsDevice.Viewport.Width;} }
        public int Height { get { return MobileFortress.GraphicsDevice.Viewport.Height; } }
    }
}
