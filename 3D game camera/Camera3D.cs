using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3D_game_camera
{
    public struct Camera3D
    {
        float yaw;
        float pitch;

        public Vector3 position;
        public Matrix View;
		public Matrix Projection;
		public float FiledOfView;
        Viewport viewport;


        MouseState oldState;
        MouseState currentState;

        const float MaxValue = 100, MinValue = 10;
        float scrolls;


        public Camera3D(Vector3 Position, Viewport viewport, float FOV = 4)
        {
            active = true;
            scrolls = 50;
            position = Position;
            View = new Matrix();
            yaw = 0;
            this.viewport = viewport;

            Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
            pitch = -MathHelper.PiOver2;
            currentState = oldState = Mouse.GetState();
            scrolls = 50;
			FiledOfView = FOV;

			//Projection = Matrix.CreatePerspectiveFieldOfView(FiledOfView, this.viewport.AspectRatio, 0.1f, 10000.0f);
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.viewport.AspectRatio, 0.1f, 10000.0f);
        }

        public void Update(GameTime gameTime, Viewport viewport)
        {
            this.viewport = viewport;
            //~~~~~~~~~~~~~~~ Keyboard ~~~~~~~~~~~~~~~
            KeyboardState state = Keyboard.GetState();
            currentState = Mouse.GetState();

            float ForwardMovement = 0;
            float SideMovement = 0;
            float UpMovement = 0;
            float Speed;
            if (state.IsKeyDown(Keys.LeftShift))
                Speed = 3;
            else
                Speed = 1;
            if (state.IsKeyDown(Keys.LeftControl))
                Speed = 0.06f;
			if(state.IsKeyDown(Keys.P))
				FiledOfView += 0.1f;
			if (state.IsKeyDown(Keys.O))
				FiledOfView -= 0.1f;
			FiledOfView = MathHelper.Clamp(FiledOfView, 1, 100);
			float f = MathHelper.Pi / FiledOfView -0.0001f;
			Projection = Matrix.CreatePerspectiveFieldOfView(f, this.viewport.AspectRatio, 0.1f, 10000.0f);

            if (state.IsKeyDown(Keys.W))
                ForwardMovement -= Speed;
            if (state.IsKeyDown(Keys.S))
                ForwardMovement += Speed;

            if (state.IsKeyDown(Keys.A))
                SideMovement -= Speed;
            if (state.IsKeyDown(Keys.D))
                SideMovement += Speed;

            if (currentState.LeftButton == ButtonState.Pressed)
                UpMovement -= Speed;
            if (currentState.RightButton == ButtonState.Pressed)
                UpMovement += Speed;
             
            Matrix forwardMovement = Matrix.CreateRotationY(yaw);
            Vector3 v = new Vector3(SideMovement, UpMovement, ForwardMovement);
            v = Vector3.Transform(v, forwardMovement);
            position.Z += v.Z;
            position.X += v.X;
            position.Y += v.Y;

            //~~~~~~~~~~~~~~~ Mouse ~~~~~~~~~~~~~~~
            if (Active)
            {
                float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

                yaw -= (currentState.X - oldState.X) * 0.2f * timeDifference;
                pitch -= (currentState.Y - oldState.Y) * 0.2f * timeDifference;
                pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.0001f, MathHelper.PiOver2 - 0.0001f);

                Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);

                Matrix RotationMatrix = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);


                Vector3 traget = Vector3.Transform(new Vector3(0, 0, -1), RotationMatrix);
                View = Matrix.CreateLookAt(position, traget + position, Vector3.Up);
            }
        }
        public void Update2(GameTime gameTime)
        {
            oldState = currentState;
            currentState = Mouse.GetState();
            KeyboardState state = Keyboard.GetState();
            float Speed;
            if (state.IsKeyDown(Keys.LeftShift))
                Speed = 0.3f;
            else
                Speed = 0.1f;
            if (state.IsKeyDown(Keys.LeftControl))
                Speed = 0.03f;

            float dif = 0;
            if (oldState.ScrollWheelValue != currentState.ScrollWheelValue)
                dif = (currentState.ScrollWheelValue - oldState.ScrollWheelValue) * 0.03f;
            scrolls -= dif;
            if (scrolls < MinValue)
                scrolls = MinValue;
            else if (scrolls > MaxValue)
                scrolls = MaxValue;
            if (state.IsKeyDown(Keys.J))
                yaw += Speed;
            if (state.IsKeyDown(Keys.K))
                yaw -= Speed;

            Vector3 vec = new Vector3(0, 0, -0.000001f);
            position.Y = scrolls;
            View = Matrix.CreateLookAt(position, vec, Vector3.Up) * Matrix.CreateRotationZ(yaw);

        }
        private bool active;
        public bool Active
        {
            set
            { 
                if(value)
                    Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
                active = value;
            }
            get { return active; }
        }
    }
}
