using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Common.Sectors
{
    struct DSSquare
    {
        public const float MinLevel = -5.0f;
        public const float MaxLevel = 45.0f;
        const float MaxDisplacement = 6.0f;

        float displacementConstant;

        Point nw;
        Point ne;
        Point sw;
        Point se;

        Point mid;

        public Point Midpoint
        {
            get { return mid; }
        }

        public int Width
        {
            get { return (ne.X - nw.X) / 2; }
        }
        public int Height
        {
            get { return (se.Y - ne.Y) / 2; }
        }

        Point n
        {
            get { return new Point(mid.X, ne.Y); }
        }
        Point s
        {
            get { return new Point(mid.X, se.Y); }
        }
        Point e
        {
            get { return new Point(ne.X, mid.Y); }
        }
        Point w
        {
            get { return new Point(nw.X, mid.Y); }
        }

        Heightmap heightmap;

        public DSSquare(Heightmap heightmap, Point nw, Point ne, Point sw, Point se, float displacement)
        {
            this.heightmap = heightmap;
            this.nw = nw;
            this.ne = ne;
            this.sw = sw;
            this.se = se;
            this.displacementConstant = displacement;
            this.mid = new Point(nw.X + (ne.X - nw.X) / 2, nw.Y + (se.Y - ne.Y) / 2);
        }

        public void SetMid(ref float[,] map)
        {
            float nwValue = map[nw.X, nw.Y];
            float neValue = map[ne.X, ne.Y];
            float swValue = map[sw.X, sw.Y];
            float seValue = map[se.X, se.Y];

            float displacement = DisplacementValue();

            map[mid.X, mid.Y] = Clamp((nwValue + neValue + swValue + seValue) / 4 + displacement);
        }

        public void SetEdges(ref float[,] map)
        {
            //north edge
            SetNorthEdge(ref map);
            SetEastEdge(ref map);
            SetWestEdge(ref map);
            SetSouthEdge(ref map);
        }

        public List<DSSquare> Subdivide()
        {
            var squares = new List<DSSquare>(4);
            float newDisp = displacementConstant / 2f;
            squares.Add(new DSSquare(heightmap, nw, n, w, mid, newDisp));
            squares.Add(new DSSquare(heightmap, n, ne, mid, e, newDisp));
            squares.Add(new DSSquare(heightmap, w, mid, sw, s, newDisp));
            squares.Add(new DSSquare(heightmap, mid, e, s, se, newDisp));
            return squares;
        }

        void SetNorthEdge(ref float[,] map)
        {
            Point farPoint = new Point(mid.X, ne.Y - Height);
            Point firstPoint = nw;
            Point secondPoint = ne;

            float firstValue = map[firstPoint.X, firstPoint.Y];
            float secondValue = map[secondPoint.X, secondPoint.Y];
            float midValue = map[mid.X, mid.Y];
            float avg;

            float displacement = DisplacementValue();

            if (DoesNotExist(farPoint, map.GetLength(0)))
            {
                avg = MinLevel;
            }
            else
            {
                float farValue = map[farPoint.X, farPoint.Y];
                avg = Clamp((farValue + firstValue + secondValue + midValue) / 4 + displacement);
            }
            Point edge = n;
            map[edge.X, edge.Y] = avg;
        }
        void SetSouthEdge(ref float[,] map)
        {
            Point farPoint = new Point(mid.X, se.Y + Height);
            Point firstPoint = sw;
            Point secondPoint = se;

            float firstValue = map[firstPoint.X, firstPoint.Y];
            float secondValue = map[secondPoint.X, secondPoint.Y];
            float midValue = map[mid.X, mid.Y];
            float avg;

            float displacement = DisplacementValue();

            if (DoesNotExist(farPoint, map.GetLength(0)))
            {
                avg = MinLevel;
            }
            else
            {
                float farValue = map[farPoint.X, farPoint.Y];
                avg = Clamp((farValue + firstValue + secondValue + midValue) / 4 + displacement);
            }
            Point edge = s;
            map[edge.X, edge.Y] = avg;
        }
        void SetEastEdge(ref float[,] map)
        {
            Point farPoint = new Point(ne.X + Width, mid.Y);
            Point firstPoint = se;
            Point secondPoint = ne;

            float firstValue = map[firstPoint.X, firstPoint.Y];
            float secondValue = map[secondPoint.X, secondPoint.Y];
            float midValue = map[mid.X, mid.Y];
            float avg;

            float displacement = DisplacementValue();

            if (DoesNotExist(farPoint, map.GetLength(0)))
            {
                avg = MinLevel;
            }
            else
            {
                float farValue = map[farPoint.X, farPoint.Y];
                avg = Clamp((farValue + firstValue + secondValue + midValue) / 4 + displacement);
            }
            Point edge = e;
            map[edge.X, edge.Y] = avg;
        }
        void SetWestEdge(ref float[,] map)
        {
            Point farPoint = new Point(nw.X - Width, mid.Y);
            Point firstPoint = nw;
            Point secondPoint = sw;

            float firstValue = map[firstPoint.X, firstPoint.Y];
            float secondValue = map[secondPoint.X, secondPoint.Y];
            float midValue = map[mid.X, mid.Y];
            float avg;

            float displacement = DisplacementValue();

            if (DoesNotExist(farPoint, map.GetLength(0)))
            {
                avg = MinLevel;
            }
            else
            {
                float farValue = map[farPoint.X, farPoint.Y];
                avg = Clamp((farValue + firstValue + secondValue + midValue) / 4 + displacement);
            }
            Point edge = w;
            map[edge.X, edge.Y] = avg;
        }

        public void FlattenMin(ref float[,] map)
        {
            float nwValue = map[nw.X, nw.Y];
            float neValue = map[ne.X, ne.Y];
            float swValue = map[sw.X, sw.Y];
            float seValue = map[se.X, se.Y];
            float midValue = map[mid.X, mid.Y];

            float flat = Math.Min(nwValue, Math.Min(neValue, Math.Min(swValue, Math.Min(seValue, midValue))));
            map[nw.X, nw.Y] = flat;
            map[ne.X, ne.Y] = flat;
            map[sw.X, sw.Y] = flat;
            map[se.X, se.Y] = flat;
            map[mid.X, mid.Y] = flat;
            map[n.X, n.Y] = flat;
            map[s.X, s.Y] = flat;
            map[e.X, e.Y] = flat;
            map[w.X, w.Y] = flat;
        }
        public void FlattenMax(ref float[,] map)
        {
            float nwValue = map[nw.X, nw.Y];
            float neValue = map[ne.X, ne.Y];
            float swValue = map[sw.X, sw.Y];
            float seValue = map[se.X, se.Y];
            float midValue = map[mid.X, mid.Y];

            float flat = Math.Max(nwValue, Math.Max(neValue, Math.Max(swValue, Math.Max(seValue, midValue))));
            map[nw.X, nw.Y] = flat;
            map[ne.X, ne.Y] = flat;
            map[sw.X, sw.Y] = flat;
            map[se.X, se.Y] = flat;
            map[mid.X, mid.Y] = flat;
            map[n.X, n.Y] = flat;
            map[s.X, s.Y] = flat;
            map[e.X, e.Y] = flat;
            map[w.X, w.Y] = flat;
        }

        float Clamp(float value)
        {
            return MathHelper.Clamp(value, MinLevel, MaxLevel);
        }

        float DisplacementValue()
        {
            return MathHelper.Clamp(heightmap.GetRandomFloat() * displacementConstant, -MaxDisplacement, MaxDisplacement);
        }

        bool DoesNotExist(Point point, int mapSize)
        {
            if (point.X < 0 || point.Y < 0) return true;
            if (point.X >= mapSize || point.Y >= mapSize) return true;
            return false;
        }

    }
}
