using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_game_camera
{
    public class Sphere
    {
        VertexPositionColor[] vertices; //later, I will provide another example with VertexPositionNormalTexture
        VertexBuffer vbuffer;
        short[] indices; //my laptop can only afford Reach, no HiDef :(
        IndexBuffer ibuffer;
        float radius;
        int nvertices, nindices;
        BasicEffect effect;
        GraphicsDevice graphicd;
		int Resolution = 90;

        public Sphere(float Radius, GraphicsDevice graphics)
        {
            radius = Radius;
            graphicd = graphics;
            effect = new BasicEffect(graphicd);
            nvertices = Resolution * Resolution; // Resolution vertices in a circle, Resolution circles in a sphere
            nindices = Resolution * Resolution * 6;
            vbuffer = new VertexBuffer(graphics, typeof(VertexPositionNormalTexture), nvertices, BufferUsage.WriteOnly);
            ibuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, nindices, BufferUsage.WriteOnly);
            createspherevertices();
            createindices();
            vbuffer.SetData<VertexPositionColor>(vertices);
            ibuffer.SetData<short>(indices);
            effect.VertexColorEnabled = true;
        }
        void createspherevertices()
        {
            vertices = new VertexPositionColor[nvertices];
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 rad = new Vector3((float)Math.Abs(radius), 0, 0);
            for (int x = 0; x < Resolution; x++) //Resolution circles, difference between each is 4 degrees
            {
                float difx = 360.0f / Resolution;
                for (int y = 0; y < Resolution; y++) //Resolution veritces, difference between each is 4 degrees 
                {
                    float dify = 360.0f / Resolution;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify));
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx));
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot);//transformation

                    vertices[x + y * Resolution] = new VertexPositionColor(point, Color.Red);
                }
            }
        }
        void createindices()
        {
            indices = new short[nindices];
            int i = 0;
            for (int x = 0; x < Resolution; x++)
            {
                for (int y = 0; y < Resolution; y++)
                {
					int s1 = x == Resolution-1 ? 0 : x + 1;
					int s2 = y == Resolution-1 ? 0 : y + 1;
                    short upperLeft = (short)(x * Resolution + y);
                    short upperRight = (short)(s1 * Resolution + y);
                    short lowerLeft = (short)(x * Resolution + s2);
                    short lowerRight = (short)(s1 * Resolution + s2);
                    indices[i++] = upperLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerLeft;
                    indices[i++] = lowerLeft;
                    indices[i++] = upperRight;
                    indices[i++] = lowerRight;
                }
            }
        }
        public void Draw(Camera3D cam)
        {
            effect.View = cam.View;
            effect.Projection = cam.Projection;
            effect.World = Matrix.CreateWorld(new Vector3(50, 60, 80), Vector3.Forward, Vector3.Up);
            graphicd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid }; // Wireframe as in the picture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicd.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, nvertices, indices, 0, indices.Length / 3);
            }
            graphicd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid };
        }
    }
}
