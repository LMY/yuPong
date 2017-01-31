using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using System.Drawing;
using System.Drawing.Imaging;


namespace yuPong
{
    interface GameState
    {
        void Draw();
        GameState Evolve(KeyboardDevice keyboard);
        void Exit();
    }

/*
    abstract class GameStateMenu : GameState
    {
        public GameStateMenu() { }
    }
    */

    class GameStateMainMenu : GameState
    {
        private Level lvl;
        private Pad pad0;
        private Pad pad1;

        private Character[] characters = { new CharacterDia(), new CharacterFusrodah(), new CharacterGaia(), new CharacterNini(), new CharacterOOM(), new CharacterPong(),
                                           new CharacterPortal(), new CharacterRetardin(), new CharacterWarry(), new CharacterWTF() };

        private int selc0;
        private int selc1;

        private int app0;
        private int app1;



        public GameStateMainMenu() : base()
        {
            lvl = new Level();

            pad0 = new Pad(lvl, 0, Appearance.PongGenericAppearance);
            pad1 = new Pad(lvl, 1, Appearance.PongGenericAppearance);

            selc0 = selc1 = 0;
            app0 = app1 = 0;
        }

        public void Draw()
        {
            pad0.Size = new Vector3(pad0.Size.X, characters[selc0].Length, pad0.Size.Z);
            pad1.Size = new Vector3(pad1.Size.X, characters[selc1].Length, pad1.Size.Z);

            pad0.theAppearance = characters[selc0].Appearances.ElementAt(app0);
            pad1.theAppearance = characters[selc1].Appearances.ElementAt(app1);

            lvl.draw();
            pad0.draw();
            pad1.draw();
        }


        public GameState Evolve(KeyboardDevice keyboard)
        {
            if (keyboard[Key.Up])
            {
                app0 = 0;
                if (++selc0 >= characters.Length) selc0 = 0;
            }
            if (keyboard[Key.Down])
            {
                app0 = 0;
                if (--selc0 < 0) selc0 = characters.Length - 1;
            }
            if (keyboard[Key.Right])
                if (++app0 >= characters[selc0].Appearances.Count) app0 = 0;
            if (keyboard[Key.Left])
                if (--app0 < 0) app0 = characters[selc0].Appearances.Count - 1;



            if (keyboard[Key.W])
            {
                app1 = 0;
                if (++selc1 >= characters.Length) selc1 = 0;
            }
            if (keyboard[Key.S])
            {
                app1 = 0;
                if (--selc1 < 0) selc1 = characters.Length - 1;
            }
            if (keyboard[Key.D])
                if (++app1 >= characters[selc1].Appearances.Count) app1 = 0;
            if (keyboard[Key.A])
                if (--app1 < 0) app1 = characters[selc1].Appearances.Count - 1;


            if (keyboard[Key.Enter] || keyboard[KeyMap.KeyMap_player1.KeySPECIAL1] || keyboard[KeyMap.KeyMap_player2.KeySPECIAL1])
                return new GameStateMatch(characters[selc0], characters[selc1], true, false, GameStateMatch.MatchType.THREEROUND);
            else
                return this; 
        }
        public void Exit() { }
    }


    class GameStateMatch : GameState
    {
        private Player player1;
        private Player player2;

        private Level lvl;
        private Pad[] pads;
        private Ball[] balls;


        private GameEventList gameevents;
        private FieldList fieldlist;

        private Color4 light5_ambient;
        private Color4 light5_diffuse;
        private Color4 light5_specular;
        private Vector4 light5_pos;
        private Vector4 light5_dir;

        private Color4 light6_ambient;
        private Color4 light6_diffuse;
        private Color4 light6_specular;
        private Vector4 light6_pos;
        private Vector4 light6_dir;

        private bool pause;
        public bool Pause { get { return pause; } set { pause = value; } }

        public Player Player1 { get { return player1; } set { player1 = value; } }
        public Player Player2 { get { return player2; } set { player2 = value; } }

        public GameEventList theEvents { get { return gameevents; } set { gameevents = value; } }
        public FieldList theFieldlist { get { return fieldlist; } set { fieldlist = value; } }

        public Level theLevel { get { return lvl; } set { lvl = value; } }
        public Pad[] Pads { get { return pads; } set { pads = value; } }
        public Ball[] Balls { get { return balls; } set { balls = value; } }

        public bool isPlayer1(Player p1) { return (player1 == p1); }


        public enum MatchType { FREEPLAY = 0, SINGLEROUND, THREEROUND };
        private MatchType matchtype;
//        public MatchType TypeOfMatch { get { return matchtype; } set { matchtype = value; } }

        private int won_p1;
        private int won_p2;
        public bool MatchEnded
        {
            get
            {
                if (matchtype == MatchType.SINGLEROUND) return (won_p1 > 0 || won_p2 > 0);
                else if (matchtype == MatchType.THREEROUND) return (won_p1 > 1 || won_p2 > 1);
                else return false;
            }
        }

        public int Won
        {
            get
            {
                if (matchtype == MatchType.SINGLEROUND)
                {
                    if (won_p1 > 0 && won_p2 > 0) return 0;
                    else if (won_p1 > 0) return 1;
                    else if (won_p2 > 0) return -1;
                    else return 0;
                }
                else if (matchtype == MatchType.THREEROUND)
                {
                    if (won_p1 > 1 && won_p2 > 1) return 0;
                    else if (won_p1 > 1) return 1;
                    else if (won_p2 > 1) return -1;
                    else return 0;
                }
                else
                    return 0;
            }
        }


        public GameStateMatch(Character char1, Character char2, bool human1, bool human2, MatchType type)
        {
            matchtype = type;
            won_p1 = won_p2 = 0;

            pause = true;

            GL.Enable(EnableCap.Texture2D);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.ClearDepth(1.0f);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Blend);

//            GL.DepthFunc(DepthFunction.Lequal); 
            GL.Enable(EnableCap.DepthTest);
//            human2 = true;


            Appearance.init();

            lvl = new Level();

            pads = new Pad[2];
            pads[0] = new Pad(lvl, 0, Appearance.PongGenericAppearance);
            pads[1] = new Pad(lvl, 1, Appearance.PongGenericAppearance);

            balls = new Ball[1];
            balls[0] = new Ball();
            balls[0].Speed = Vector3.UnitX * 7;

            if (human1)
                player1 = new Player(this, char1, pads[0], InputHuman.input1, char1.DefaultAppearance);
            else
                player1 = new Player(this, char1, pads[0], new InputAI(), char1.DefaultAppearance);

            if (human2)
                player2 = new Player(this, char2, pads[1], InputHuman.input2, char2.DefaultAppearance);
            else
                player2 = new Player(this, char2, pads[1], new InputAI(), char2.DefaultAppearance);


            theEvents = new GameEventList();
            theFieldlist = new FieldList();

            const float hbarsize = 0.85f;
            player1.Healthbar = new Healthbar(new Vector3(lvl.SizeX / 2 - hbarsize / 2 * lvl.SizeX / 2, lvl.SizeY / 2 * 1.2f, 0), false, new Vector3(lvl.SizeX / 2 * hbarsize, 0.4f, 1));
            player2.Healthbar = new Healthbar(new Vector3(-lvl.SizeX / 2 + hbarsize / 2 * lvl.SizeX / 2, lvl.SizeY / 2 * 1.2f, 0), true, new Vector3(lvl.SizeX / 2 * hbarsize, 0.4f, 1));



            GL.Enable(EnableCap.Lighting);      // Let there Be Light

            float R0 = pads[0].theAppearance.Color.R;
            float G0 = pads[0].theAppearance.Color.G;
            float B0 = pads[0].theAppearance.Color.B;

            float R1 = pads[1].theAppearance.Color.R;
            float G1 = pads[1].theAppearance.Color.G;
            float B1 = pads[1].theAppearance.Color.B;

            const float light_height = 12.5f;

            light5_ambient = new Color4(0.6f, 0.6f, 0.6f, 1);
            light5_diffuse = new Color4(0.6f, 0.6f, 0.6f, 1);
            light5_specular = new Color4(1.0f, 1.0f, 1.0f, 1);
            light5_pos = new Vector4(lvl.SizeX / 4, 0, light_height, 1);
            light5_dir = new Vector4(0, 0, -1, 0);

            light6_ambient = new Color4(0.6f, 0.6f, 0.6f, 1);
            light6_diffuse = new Color4(0.6f, 0.6f, 0.6f, 1);
            light6_specular = new Color4(1.0f, 1.0f, 1.0f, 1);

            light6_pos = new Vector4(-lvl.SizeX / 4, 0, light_height, 1);
            light6_dir = new Vector4(0, 0, -1, 0);

            SetAmbientLight();
            GL.Enable(EnableCap.Light5);
            GL.Enable(EnableCap.Light6);

            ResetRound();

            pause = false;
        }



        public void Exit()
        {
            GL.Disable(EnableCap.Light5);
            GL.Disable(EnableCap.Light6);
        }



        private void SetAmbientLight()
        {
            GL.Light(LightName.Light5, LightParameter.Ambient, light5_ambient);
            GL.Light(LightName.Light5, LightParameter.Diffuse, light5_diffuse);
            GL.Light(LightName.Light5, LightParameter.Specular, light5_specular);
            GL.Light(LightName.Light5, LightParameter.Position, light5_pos);
            GL.Light(LightName.Light5, LightParameter.SpotDirection, light5_dir);
            GL.Light(LightName.Light5, LightParameter.SpotCutoff, 26f);

            GL.Light(LightName.Light6, LightParameter.Ambient, light6_ambient);
            GL.Light(LightName.Light6, LightParameter.Diffuse, light6_diffuse);
            GL.Light(LightName.Light6, LightParameter.Specular, light6_specular);
            GL.Light(LightName.Light6, LightParameter.Position, light6_pos);
            GL.Light(LightName.Light6, LightParameter.SpotDirection, light6_dir);
            GL.Light(LightName.Light6, LightParameter.SpotCutoff, 26f);
        }


        public void Draw() 
        {
            player1.Healthbar.draw();
            player2.Healthbar.draw();
            drawPoints();

            foreach (Pad p in pads) p.setLight();
            SetAmbientLight();

            lvl.draw();
            foreach (Pad p in pads) p.draw();
            foreach (Ball b in balls) b.draw();
        }

        public void drawPoints()
        {
            const float latus = 0.5f;
            const float spacing = 0.2f;
            const float dy = 2 * latus;

            for (int i=0; i<won_p1; i++)
            {
                Box b = new Box(Appearance.Point1Appearance, new Vector3((i + 0.5f) * (latus + spacing), lvl.SizeY / 2 + dy, 0));
                b.Size = new Vector3(latus,latus,latus);
                b.draw();
            }

            for (int i = 0; i < won_p2; i++)
            {
                Box b = new Box(Appearance.Point2Appearance, new Vector3(-(i + 0.5f) * (latus + spacing), lvl.SizeY / 2 + dy, 0));
                b.Size = new Vector3(latus, latus, latus);
                b.draw();
            }

        }

        public GameState Evolve(KeyboardDevice keyboard)
        {
            if (pause)
                return this;

            yPhysics.Instance.Beat();
            gameevents.newTime(yPhysics.Instance.FrameN);
    
            fieldlist.newTime(yPhysics.Instance.FrameN, this);

            // GET INPUT
            Keypress[] move0 = KeyMap.KeyMap_system.translateInput(this, keyboard);

            PongMove move1 = player1.getInput(this, keyboard);
            PongMove move2 = player2.getInput(this, keyboard);
            //move2 = AI.SuggestForce(1, lvl, balls, pads[1]);

            // APPLY INPUT
            move1.Apply(this, player1);
            move2.Apply(this, player2);
    
            // MOVE PADS
            float force_constant = 400f * pads[0].Mass;
            float resistence_constant = 18.0f;
            pads[0].verlet(yPhysics.Instance.DT, resistence_constant);
            pads[1].verlet(yPhysics.Instance.DT, resistence_constant);

            // MOVE BALLS
            AgeBallsAndKillThem();

            for (int b = 0; b < balls.Length; b++)
                balls[b].verlet(yPhysics.Instance.DT);


            // CALCULATE COLLISIONS
            CollisionInformations[] collisions = CollisionDetection.CheckCollisions(lvl, pads, balls);

            // slow balls, for they tend to spin fast.
            const float max_ball_speed = 17.0f;

            for (int b = 0; b < balls.Length; b++)
                if (balls[b].Speed.Length > max_ball_speed)
                    balls[b].Speed = balls[b].Speed * max_ball_speed / balls[b].Speed.Length;


            // HP loss, deaths, etc...

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].CollidingObj is Ball)
                {
                    Ball theball = ((Ball)collisions[i].CollidingObj);

                    if (collisions[i].TargetObj.Equals(lvl.BoxLeft))
                    {
                        theball.hitWall(this, lvl.BoxLeft);
                        player1.Healthbar.increase(-7);
                    }
                    else if (collisions[i].TargetObj.Equals(lvl.BoxRight))
                    {
                        theball.hitWall(this, lvl.BoxRight);
                        player2.Healthbar.increase(-7);
                    }


                    else if (collisions[i].TargetObj.Equals(player1.thePad))
                    {
                        theball.hitPlayer(this, player1);
                        KillExpiredBalls();
                    }
                    else if (collisions[i].TargetObj.Equals(player2.thePad))
                    {
                        theball.hitPlayer(this, player2);
                        KillExpiredBalls();
                    }
                }
            }


            /*          
                        if (collisions.Length > 0)
                        {
            //                pause = true;
            //                Console.WriteLine("Collisions done: " + collisions.Length);

                            for (int i = 0; i < collisions.Length; i++)
                            {
                                if (collisions[i].CollidingObj is Ball)
                                    Console.Write("ball ");
                                else if (collisions[i].CollidingObj is Pad)
                                    Console.Write("pad ");
                                else if (collisions[i].CollidingObj is Box)
                                    Console.Write("wall ");

                                if (collisions[i].TargetObj is Ball)
                                    Console.WriteLine("ball");
                                else if (collisions[i].TargetObj is Pad)
                                    Console.WriteLine("pad");
                                else if (collisions[i].TargetObj is Box)
                                    Console.WriteLine("wall");
                            }

                        }
            */


            bool changes = false;

            if (player1.Healthbar.HP <= 0)
            {
                won_p2++;
                changes = true;
            }
            if (player2.Healthbar.HP <= 0)
            {
                won_p1++;
                changes = true;
            }

            if (MatchEnded)
            {
                int winner = Won;
                won_p1 = won_p2 = 0;
            }

            if (matchtype != MatchType.FREEPLAY && changes)
                ResetRound();


            return this;
        }


        public void ResetRound()
        {
            player1.Healthbar.HP = player1.Healthbar.MAXHP;
            player2.Healthbar.HP = player2.Healthbar.MAXHP;

            balls = new Ball[1];
            balls[0] = new Ball();
            balls[0].Speed = Vector3.UnitX * 7;

            const float rear_spacing = 1.0f;
            pads[0].Center = new Vector3(lvl.SizeX / 2 - rear_spacing - pads[0].Size.X / 2, 0, 0);
            pads[1].Center = new Vector3(-lvl.SizeX / 2 + rear_spacing + pads[1].Size.X / 2, 0, 0);
        }


        public void newBall(Vector3 center, Vector3 speed, int duration)
        {
            newBall(center, speed, duration, null);
        }


        public void newBall(Vector3 center, Vector3 speed, int duration, GameEventModifyPlayer onhit_event)
        {
            Ball[] newballs = new Ball[balls.Length + 1];

            for (int b = 0; b < balls.Length; b++)
                newballs[b] = balls[b];

            newballs[balls.Length] = onhit_event == null ? new Ball(duration) : new BastardBall(onhit_event, duration);

            newballs[balls.Length].setCenter(center);
            newballs[balls.Length].setSpeed(speed);

            balls = newballs;
        }


        protected void AgeBallsAndKillThem()
        {
            int deleted = 0;

            for (int b = 0; b < balls.Length; b++)
                if (balls[b].Age > 0)
                {
                    balls[b].DecreaseAge();

                    if (balls[b].Age == 0)
                    {
                        deleted++;
                        balls[b] = null;
                    }
                }

            if (deleted > 0)
            {
                Ball[] newballs = new Ball[balls.Length - deleted];
                int i = 0;

                for (int b = 0; b < balls.Length; b++)
                    if (balls[b] != null)
                        newballs[i++] = balls[b];

                balls = newballs;
            }
        }

        protected void KillExpiredBalls()
        {
            int deleted = 0;

            for (int b = 0; b < balls.Length; b++)
                if (balls[b].Age == 0)
                {
                    deleted++;
                    balls[b] = null;
                }

            if (deleted > 0)
            {
                Ball[] newballs = new Ball[balls.Length - deleted];
                int i = 0;

                for (int b = 0; b < balls.Length; b++)
                    if (balls[b] != null)
                        newballs[i++] = balls[b];

                balls = newballs;
            }
        }




    }
}
