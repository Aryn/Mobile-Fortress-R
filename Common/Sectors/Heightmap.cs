using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Common.Sectors
{
    class Heightmap
    {
        const int seaLevel = 0;
        float[,] values;
        public int Size { get; private set; }
        int power;
        Random rng;

        public int Seed { get; private set; }
        public float Roughness { get; private set; }

        List<DSSquare> squares = new List<DSSquare>(1);

        public float[,] Values
        {
            get
            {
                return values;
            }
        }

        public Heightmap(int seed, int power, float roughness)
        {
            this.power = power;
            rng = new Random(seed);
            Size = (int)Math.Pow(2, power) + 1;
            Roughness = roughness;
            values = new float[Size, Size];
            GenerateMap();
        }

        void GenerateMap()
        {
            var mapsize = values.GetLength(0) - 1;
            values[0, 0] = seaLevel;
            values[mapsize, 0] = seaLevel;
            values[0, mapsize] = seaLevel;
            values[mapsize, mapsize] = seaLevel;
            squares.Add(new DSSquare(
                heightmap: this,
                nw: new Point(0, 0),
                ne: new Point(mapsize, 0),
                sw: new Point(0, mapsize),
                se: new Point(mapsize, mapsize),
                displacement: Roughness
                ));
            float squareSize = mapsize;
            var newSquares = new List<DSSquare>(4);
            while (squareSize > 1)
            {
                foreach (DSSquare square in squares)
                {
                    square.SetMid(ref values);
                }
                foreach (DSSquare square in squares)
                {
                    square.SetEdges(ref values);
                }
                foreach (DSSquare square in squares)
                {
                    newSquares.AddRange(square.Subdivide());
                }
                squareSize /= 2;
                squares = newSquares;
                newSquares = new List<DSSquare>(4);
            }

            //Final map modifiers
            //Now handled by additive sea level.
            /*for (int x = 0; x <= mapsize; x++)
                for (int y = 0; y <= mapsize; y++)
                {
                    map[x, y] = MathHelper.Max(map[x, y], SeaLevel); //Clamp to sea level.
                }
             */
        }

        public float GetRandomFloat()
        {
            return (float)rng.NextDouble() - 0.5f;
        }
    }
}
