using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mobile_Fortress_R.Rendering
{
    class MFMath
    {
        public static void CalculateTangentArray(ref VertexMultitextured[] vertices, int[] indices)
        {
            Console.WriteLine("Calculating tangent vectors...");
            for(int tri = 0; tri < indices.Length; tri+=3)
            {
                var A = vertices[indices[tri]];
                var B = vertices[indices[tri+1]];
                var C = vertices[indices[tri+2]];

                Vector2 TU, TV;
                Vector3 Diff1, Diff2;

                Diff1 = B.Position - A.Position;
                Diff2 = C.Position - A.Position;

                TU.X = B.TextureCoordinate.X - A.TextureCoordinate.X;
                TV.X = B.TextureCoordinate.Y - A.TextureCoordinate.Y;

                TU.Y = C.TextureCoordinate.X - A.TextureCoordinate.X;
                TV.Y = C.TextureCoordinate.Y - A.TextureCoordinate.Y;

                float scalar = 1f / (TU.X * TV.Y - TU.Y * TV.X);

                Vector3 tangent;
                tangent.X = (TV.Y * Diff1.X - TV.X * Diff2.X) * scalar;
                tangent.Y = (TV.Y * Diff1.Y - TV.X * Diff2.Y) * scalar;
                tangent.Z = (TV.Y * Diff1.Z - TV.X * Diff2.Z) * scalar;

                Vector3 bitangent;
                bitangent.X = (TU.X * Diff2.X - TU.Y * Diff1.X) * scalar;
                bitangent.Y = (TU.X * Diff2.Y - TU.Y * Diff1.Y) * scalar;
                bitangent.Z = (TU.X * Diff2.Z - TU.Y * Diff1.Z) * scalar;

                tangent.Normalize();
                bitangent.Normalize();

                A.AddTangent(tangent);
                B.AddTangent(tangent);
                C.AddTangent(tangent);
                A.AddBitangent(bitangent);
                B.AddBitangent(bitangent);
                C.AddBitangent(bitangent);

                A.AddNormal(-Vector3.Cross(bitangent, tangent));
                B.AddNormal(-Vector3.Cross(bitangent, tangent));
                C.AddNormal(-Vector3.Cross(bitangent, tangent));

                vertices[indices[tri]] = A;
                vertices[indices[tri + 1]] = B;
                vertices[indices[tri + 2]] = C;
            }
        }
    }
}
