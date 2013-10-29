using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Common
{
    class Atlas
    {
        static Dictionary<Point, Sector> sectors = new Dictionary<Point, Sector>();
        public static Sector GetSector(int x, int y)
        {
            return GetSector(new Point(x, y));
        }
        public static Sector GetSector(Point p)
        {
            Sector S = null;
            if (!sectors.TryGetValue(p, out S))
            {
                S = new Sector(p);
                sectors.Add(p, S);
            }
            return S;
        }
    }
}
