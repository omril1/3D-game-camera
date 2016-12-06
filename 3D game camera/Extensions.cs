using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _3D_game_camera
{
    static class Extensions
    {
        public static void DrawLine(this SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 1);
        }
        public static Vector3 Round(this Vector3 value)
        {
            value.X = (float)Math.Round(value.X, 2);
            value.Y = (float)Math.Round(value.Y, 2);
            value.Z = (float)Math.Round(value.Z, 2);
            return value;
        }
    }
}
