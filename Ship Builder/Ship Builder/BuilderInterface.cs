using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ship_Builder.ShipSpace;
using Ship_Builder.UserInterface;

namespace Ship_Builder
{
    class BuilderInterface
    {
        static Point noPoint = new Point(-1, -1);

        public Rectangle Dimensions { get; private set; }
        public int RightWindowBoundary { get; private set; }
        public int BottomWindowBoundary { get; private set; }
        public int Spacing { get; private set; }
        GraphicsDevice device;
        Texture2D background;
        Texture2D fill;
        Texture2D border;
        Point blinkingSquare = noPoint;

        Room[,] grid;
        RoomType selectedType = RoomType.Normal;
        int selectedNum = 0;

        Counter roomIndex;

        public BuilderInterface(GraphicsDevice device)
        {
            this.device = device;
            RightWindowBoundary = (int)(device.Viewport.Width * 0.25f);
            BottomWindowBoundary = (int)(device.Viewport.Height * 0.20f);
            Dimensions = new Rectangle(0, 0, device.Viewport.Width - RightWindowBoundary, device.Viewport.Height - BottomWindowBoundary);
            Spacing = 16;
            GenerateBackground();
            grid = new Room[Dimensions.Width / Spacing+1, Dimensions.Height / Spacing+1];
            for (int x = 0; x < Dimensions.Width / Spacing+1; x++)
            {
                for (int y = 0; y < Dimensions.Height / Spacing+1; y++)
                {
                    grid[x, y] = null;
                }
            }
            GenerateUI();
        }

        void GenerateBackground()
        {
            background = new Texture2D(device, Spacing, Spacing);
            Color[] data = new Color[Spacing * Spacing];
            for (int y = 0; y < Spacing; y++)
            {
                for (int x = 0; x < Spacing; x++)
                {
                    data[Spacing * y + x] = (x == (Spacing-1) || y == (Spacing-1)) ? Color.White : Color.Black;
                }
            }
            background.SetData<Color>(data);
            border = new Texture2D(device, Spacing, Spacing);
            for (int y = 0; y < Spacing; y++)
            {
                for (int x = 0; x < Spacing; x++)
                {
                    data[Spacing * y + x] = (x == (Spacing - 1) || y == (Spacing - 1) || x == 0 || y == 0) ? Color.White : Color.Black;
                }
            }
            border.SetData<Color>(data);
            fill = new Texture2D(device, Spacing, Spacing);
            for (int y = 0; y < Spacing; y++)
            {
                for (int x = 0; x < Spacing; x++)
                {
                    data[Spacing * y + x] = Color.White;
                }
            }
            fill.SetData<Color>(data);
        }

        void GenerateUI()
        {
            roomIndex = new Counter(Dimensions.X + Dimensions.Width + 24, 12);
        }

        public void Draw(SpriteBatch batch)
        {
            DrawRooms(batch);
            DrawUI(batch);
        }

        void DrawRooms(SpriteBatch batch)
        {
            for (int x = 0; x < Dimensions.Width; x += Spacing)
            {
                for (int y = 0; y < Dimensions.Height; y += Spacing)
                {
                    Room room = grid[x / Spacing, y / Spacing];
                    if (room != null)
                    {
                        room.Draw(batch, fill, x, y);
                    }
                    else
                        batch.Draw(background, new Rectangle(x, y, Spacing, Spacing), new Rectangle(0, 0, background.Height, background.Width), Color.Teal);
                }
            }
            if (blinkingSquare != noPoint)
            {
                batch.Draw(border, new Rectangle(blinkingSquare.X * Spacing, blinkingSquare.Y * Spacing, Spacing, Spacing), Color.Red);
            }
        }
        void DrawUI(SpriteBatch batch)
        {
            roomIndex.Draw(batch);
        }

        public void Update(float dt)
        {
            blinkingSquare = noPoint;
        }

        public void LeftClicked(int X, int Y)
        {
            if (roomIndex.Contains(X, Y))
            {
                roomIndex.LeftClicked(X, Y);
                return;
            }
            Point gridSquare = new Point(X / Spacing, Y / Spacing);
            blinkingSquare = gridSquare;
            PlaceRoom(Room.GetRoom(selectedType,selectedNum), gridSquare);
        }

        void PlaceRoom(Room room, Point square)
        {
            Room prevRoom = grid[square.X, square.Y];
            if (prevRoom == room) return;
            if (adjacentToSameRoom(room, square) || room.Quantity == 0)
            {
                if (prevRoom != null) prevRoom.Removed();
                grid[square.X, square.Y] = room;
                room.Placed();
            }
        }
        bool adjacentToSameRoom(Room room, Point square)
        {
            Point left = new Point(square.X - 1, square.Y);
            Point right = new Point(square.X + 1, square.Y);
            Point up = new Point(square.X, square.Y - 1);
            Point down = new Point(square.X, square.Y + 1);

            if (grid[left.X, left.Y] == room) return true;
            if (grid[right.X, right.Y] == room) return true;
            if (grid[up.X, up.Y] == room) return true;
            if(grid[down.X, down.Y] == room) return true;
            return false;
        }
    }
}
