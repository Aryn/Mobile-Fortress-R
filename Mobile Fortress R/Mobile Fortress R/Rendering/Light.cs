using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mobile_Fortress_R.Rendering
{
    struct Light
    {
        public Vector3 position;
        public Vector3 color;
        public float radius;
        public Light(Vector3 pos, Vector3 col, float r)
        {
            position = pos;
            color = col;
            radius = r;
        }
    }
}
