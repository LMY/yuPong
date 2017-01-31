
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;


namespace yuPong
{
    class Game : GameWindow
    {
        private Camera camera;
        private GameState gamestate;
        
        
        public Game() : base(1280, 800, GraphicsMode.Default, "Y U PONG?!")
        {
            mousex = mousey = -1;

            VSync = VSyncMode.On;
            camera = new Camera();
            camera.From = new Vector3(0, 0, -16);
            UseDebugCamera = false;

            GL.Enable(EnableCap.Texture2D);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            Appearance.init();

            y_already_pressed = false;

            gamestate = new GameStateMainMenu();
            //gamestate = new GameStateMatch(new CharacterNini(), new CharacterOOM(), true, false, GameStateMatch.MatchType.FREEPLAY);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }



        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }


        private bool use_debug_camera;
        public bool UseDebugCamera { get { return use_debug_camera; } set { use_debug_camera = value; } }


        private int mousex;       // consider mouse[x|y] STATIC in OnUpdateFrame. used only in OnUpdateFrame to calculate mouse delta[x|y]
        private int mousey;       // do not use these vars in any other funcion!

        private bool y_already_pressed;


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.Y])
            {
                if (!y_already_pressed)
                    use_debug_camera = !use_debug_camera;

                y_already_pressed = true;
            }
            else
                y_already_pressed = false;


            if (use_debug_camera)
            {
                if (mousex == -1 && mousey == -1)
                {
                    mousex = Mouse.X;
                    mousey = Mouse.Y;
                }
                else
                {
                    int deltax = Mouse.X - mousex;
                    int deltay = Mouse.Y - mousey;


                    if (Mouse[MouseButton.Left] == true)
                    {
                        if (deltax != 0)
                            camera.Gamma = camera.Gamma - 0.005f * deltax;

                        if (deltay != 0)
                            camera.Alpha = camera.Alpha - 0.005f * deltay;
                    }

                    mousex = Mouse.X;
                    mousey = Mouse.Y;
                }

                if (Keyboard[Key.Up])
                    camera.Walk(1);
                if (Keyboard[Key.Down])
                    camera.Walk(-1);
                if (Keyboard[Key.Right])
                    camera.Strafe(1);
                if (Keyboard[Key.Left])
                    camera.Strafe(-1);
            }

            gamestate = gamestate.Evolve(Keyboard);
        }



    
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            camera.apply();

            gamestate.Draw();

            SwapBuffers();
        }

    

        [STAThread]
        static void Main()
        {
            float fps = 40.0f;

            yPhysics.init();
            yPhysics.Instance.DT = 1 / fps;

            Game game = new Game();
            game.Run(fps);
        }
    }
}