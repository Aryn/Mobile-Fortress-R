using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ship_Builder.ShipSpace
{
    public enum RoomType { Normal, Bridge, Engine, Weapons, Quarters }
    class Room
    {
        static Dictionary<int,Room> Hallways = new Dictionary<int,Room>();
        static Dictionary<int, Room> Bridges = new Dictionary<int, Room>();

        public static Room GetRoom(RoomType type, int n)
        {
            Dictionary<int, Room> rooms = null;
            switch (type)
            {
                case RoomType.Bridge:
                    rooms = Bridges;
                    break;
                default:
                    rooms = Hallways;
                    break;
            }
            Room room = null;
            if (!Hallways.TryGetValue(n, out room))
            {
                room = new Room(RoomType.Normal, n);
                Hallways.Add(n, room);
            }
            return room;
        }

        public int Quantity { get; private set; }
        RoomType type;
        Color color;
        public Room(RoomType type, int number)
        {
            this.type = type;
            switch (type)
            {
                case RoomType.Normal:
                    color = Color.SeaGreen;
                    break;
                case RoomType.Bridge:
                    color = Color.Yellow;
                    break;
                case RoomType.Engine:
                    color = Color.LightGray;
                    break;
                case RoomType.Weapons:
                    color = Color.Blue;
                    break;
                case RoomType.Quarters:
                    color = Color.AliceBlue;
                    break;
                default:
                    color = Color.White;
                    break;
            }
        }
        public void Draw(SpriteBatch batch, Texture2D tex, int x, int y)
        {
            batch.Draw(tex, new Rectangle(x, y, tex.Width, tex.Height), color);
        }
        public void Placed()
        {
            Quantity++;
        }
        public void Removed()
        {
            Quantity--;
        }
    }
}
