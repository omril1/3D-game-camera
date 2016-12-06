using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3D_game_camera
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model Ship, SkyBox;
        Sphere sphere;

        Vector3 Position, LastPosition, Forward = Vector3.Backward;

        Camera3D camera1, camera2;
        Viewport leftView, rightView, defaultView;
        Texture2D Line;

        MouseState oldState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.PreferredBackBufferHeight = 800;
            //graphics.PreferredBackBufferWidth = 1200;
        }

        protected override void Initialize()
        {
            oldState = Mouse.GetState();

            defaultView = rightView = leftView = GraphicsDevice.Viewport;
            rightView.Width = leftView.Width /= 2;
            rightView.Width = --leftView.Width;
            rightView.X = leftView.Width + 2;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Ship = Content.Load<Model>("ship//ship");
            SkyBox = Content.Load<Model>("SkyBox//skybox");
            Line = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Line.SetData(new Color[] { Color.White });
            sphere = new Sphere(30, GraphicsDevice);
            InitCameras();
        }

        private void InitCameras()
        {
            camera1 = new Camera3D(new Vector3(0, 50, 0), leftView);
            camera2 = new Camera3D(new Vector3(0, 50, 0), rightView);
            camera2.Active = false;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            bool ForwardDirty = false;
            var State = Keyboard.GetState();
            var ElapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds;
            if (State.IsKeyDown(Keys.E))
            {
                Position = new Vector3();
                Forward = Vector3.Backward;
            }
            LastPosition = Position;

            if (State.IsKeyDown(Keys.Escape))
                this.Exit();
            camera1.Update(gameTime, leftView);
            this.GraphicsDevice.Viewport = rightView;
            camera2.Update2(gameTime);
            this.GraphicsDevice.Viewport = defaultView;

            if (State.IsKeyDown(Keys.R)) { InitCameras(); }

            if (State.IsKeyDown(Keys.Left))
            {
                Position -= Vector3.UnitX * 0.1f * ElapsedTime;
                ForwardDirty = true;
            }
            if (State.IsKeyDown(Keys.Right))
            {
                Position += Vector3.UnitX * 0.1f * ElapsedTime;
                ForwardDirty = true;
            }
            if (State.IsKeyDown(Keys.Up))
            {
                Position -= Vector3.UnitZ * 0.1f * ElapsedTime;
                ForwardDirty = true;
            }
            if (State.IsKeyDown(Keys.Down))
            {
                Position += Vector3.UnitZ * 0.1f * ElapsedTime;
                ForwardDirty = true;
            }
            if (ForwardDirty)
            {
                Forward -= (Position - LastPosition) / 3f;
                Forward.Normalize();
                Forward = Forward.Round();
                ForwardDirty = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Drawing on the left half of the screen
            this.GraphicsDevice.Viewport = leftView;
            DrawSkyBox(camera1);
            DrawModel(Ship, camera1, Matrix.CreateWorld(Position, Forward, Vector3.Up) * Matrix.CreateRotationX(MathHelper.Pi), Vector3.One);
            sphere.Draw(camera1);


            // Drawing on the right half of the screen
            this.GraphicsDevice.Viewport = rightView;
            DrawSkyBox(camera2);
            DrawModel(Ship, camera2, Matrix.CreateWorld(Position, Forward, Vector3.Up) * Matrix.CreateRotationX(MathHelper.Pi), Vector3.One);
            sphere.Draw(camera2);


            // Drawing on the whole screen
            this.GraphicsDevice.Viewport = defaultView;
            spriteBatch.Begin();
            spriteBatch.DrawLine(Line, 3, Color.White, new Vector2(leftView.Width + 3, 0), new Vector2(leftView.Width + 3, leftView.Height));
            spriteBatch.End();

            //spriteBatch changes our depthStencile, so we need to change it back.
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //DrawModel(Ship, camera1, Matrix.CreateWorld(Position, Forward, Vector3.Up), new Vector3(0.5f, 1, 1)); //draw the ship over the whole screen too
        }

        #region drawing

        private void DrawModel(Model model, Camera3D camera, Matrix World, Vector3 Scale)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = World * Matrix.CreateRotationX(MathHelper.Pi);
                    effect.View = camera.View;

                    effect.Projection = camera.Projection * Matrix.CreateScale(Scale);
                }
                mesh.Draw();
            }
        }

        private void DrawSkyBox(Camera3D camera)
        {
            foreach (var mesh in SkyBox.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Matrix temp = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateScale(300);
                    effect.World = temp;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }
        #endregion
        protected override void OnActivated(object sender, EventArgs args)
        {
            camera1.Active = true;
            base.OnActivated(sender, args);
        }
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            camera1.Active = false;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            base.OnDeactivated(sender, args);
        }

    }
}
