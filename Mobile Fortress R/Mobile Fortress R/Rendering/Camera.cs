using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mobile_Fortress_R.Rendering
{
    class Camera
    {
        static Vector3 position;
        public static Vector3 Position { get { return position; } set { Move(value); } }
        public static Matrix World;
        public static Matrix View;
        public static Matrix Projection;

        static float yaw = 0;
        static float pitch = 0;
        static float hSensitivity = 1;
        static float vSensitivity = 1;

        const float minPitch = -85 * (MathHelper.Pi / 180);
        const float maxPitch = 85 * (MathHelper.Pi / 180);

        public static Vector3 Target;

        static Viewport port;
        static MouseState oldMouse;

        public static void Setup(Viewport p)
        {
            oldMouse = Mouse.GetState();
            port = p;
            Position = Vector3.Backward * 3 + Vector3.Up * 4 + Vector3.Left*4;
            View = Matrix.CreateLookAt(Position, Vector3.Up * 4, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), port.AspectRatio, 0.5f, 1800f);
            World = Matrix.CreateWorld(Position, Target - Position, Vector3.Up);
        }

        public static void Move(Vector3 newPosition)
        {
            position = newPosition;
            World = Matrix.CreateTranslation(Position) * Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Target = newPosition + World.Forward;
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }

        public static void Update(float dt)
        {
            MouseState M = Mouse.GetState();
            yaw += (250-M.X) * dt * hSensitivity;
            pitch += (250-M.Y) * dt * vSensitivity;
            pitch = MathHelper.Clamp(pitch, minPitch, maxPitch);
            yaw = MathHelper.WrapAngle(yaw);
            oldMouse = M;
            Move(position);
            Mouse.SetPosition(250, 250);
        }

        /*
         * Vector3.Backward * 8 + Vector3.Up * 4
         */
    }
}
