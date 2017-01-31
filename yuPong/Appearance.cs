using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;


namespace yuPong
{
    class Appearance
    {
        public static void init()
        {
            CharacterPongAppearance.LoadTexture("textures/character_pong1.png");
            CharacterWarryAppearance.LoadTexture("textures/character_warry1.png");
            CharacterDiaAppearance.LoadTexture("textures/character_dia1.png");
            CharacterGaiaAppearance.LoadTexture("textures/character_gaia1.png");
            CharacterPortalAppearance.LoadTexture("textures/character_portal1.png");
            CharacterWTFAppearance.LoadTexture("textures/character_wtf1.png");
            CharacterRetardinAppearance.LoadTexture("textures/character_retardin1.png");
            CharacterOOMAppearance.LoadTexture("textures/character_oom1.png");
            CharacterNiniAppearance.LoadTexture("textures/character_nini1.png");
            CharacterFusrodahAppearance.LoadTexture("textures/character_fusrodah1.png");

            BallAppearance.LoadTexture("textures/texture_ball.png");
            WallAppearance.LoadTexture("textures/texture_wall.png");
            GroundAppearance.LoadTexture("textures/texture_ground.png");
        }


        public static readonly Appearance BallAppearance = new Appearance(new Color4(1.0f, 1.0f, 1.0f, 1.0f));
        public static readonly Appearance WallAppearance = new Appearance(new Color4(0.8f, 0.8f, 0.8f, .8f));
        public static readonly Appearance GroundAppearance = new Appearance(new Color4(0.8f, 0.8f, 0.8f, 1.0f));

        public static readonly Appearance CharacterPongAppearance = new Appearance(new Color4(0.6f, 0.6f, 0.6f, 1.0f));
        public static readonly Appearance CharacterWarryAppearance = new Appearance(new Color4(0.8f, 0.4f, 0.4f, 1.0f));
        public static readonly Appearance CharacterDiaAppearance = new Appearance(new Color4(0.4f, 0.1f, 0.4f, 1.0f));
        public static readonly Appearance CharacterGaiaAppearance = new Appearance(new Color4(0.05f, 0.45f, 0.15f, 1.0f));
        public static readonly Appearance CharacterPortalAppearance = new Appearance(new Color4(0.1f, 0.6f, 0.6f, 1.0f));
        public static readonly Appearance CharacterWTFAppearance = new Appearance(new Color4(0.4f, 0.4f, 0.4f, 0.7f));
        public static readonly Appearance CharacterRetardinAppearance = new Appearance(Color4.HotPink);
        public static readonly Appearance CharacterOOMAppearance = new Appearance(Color4.White);
        public static readonly Appearance CharacterNiniAppearance = new Appearance(Color4.Yellow);
        public static readonly Appearance CharacterFusrodahAppearance = new Appearance(new Color4(0.8f, 0.1f, 0.1f, 1.0f));

        public static readonly Appearance HB1Appearance = new Appearance(Color4.LightGreen);
        public static readonly Appearance HB2Appearance = new Appearance(Color4.Green);
        public static readonly Appearance HB3Appearance = new Appearance(Color4.YellowGreen);
        public static readonly Appearance HB4Appearance = new Appearance(Color4.Yellow);
        public static readonly Appearance HB5Appearance = new Appearance(Color4.Orange);
        public static readonly Appearance HB6Appearance = new Appearance(Color4.Red);
        public static readonly Appearance HB7Appearance = new Appearance(Color4.DarkGray);

        public static readonly Appearance Point1Appearance = new Appearance(new Color4(1.0f, 1.0f, 1.0f, 1.0f));
        public static readonly Appearance Point2Appearance = new Appearance(new Color4(1.0f, 1.0f, 1.0f, 1.0f));



        public static readonly Appearance PongGenericAppearance = new Appearance(Color4.DarkGray);

        public static readonly Appearance PongWarryAppearance1 = new Appearance(Color4.DarkSlateBlue);
        public static readonly Appearance PongWarryAppearance2 = new Appearance(Color4.DarkSlateGray);

        public static readonly Appearance PongOOMAppearance1 = new Appearance(Color4.DarkGreen);
        public static readonly Appearance PongOOMAppearance2 = new Appearance(Color4.Green);


        protected Color4 color;
        public Color4 Color { get { return color; } set { color = value; } }

        protected Color4 colorBlend;
        public Color4 ColorBlend { get { return colorBlend; } set { colorBlend = value; } }

        protected Color4 colorClear;
        public Color4 ColorClear { get { return colorClear; } set { colorClear = value; } }

        private int textureid;
        public int TextureID { get { return textureid; } }

        public bool hasTextureID() { return (textureid > 0); }



        public Appearance() : this(Color4.White) { }
        public Appearance(Color4 acolor) : this(acolor, acolor, acolor) {}
        public Appearance(Color4 clr, Color4 blend, Color4 clear)
        {
            textureid = 0;
            color = clr;
            colorBlend = blend;
            colorClear = clear;
        }

        public Appearance(Color4 acolor, String texture_filename) : this(acolor, acolor, acolor)
        {
            LoadTexture(texture_filename);
        }

        public Appearance(Color4 clr, Color4 blend, Color4 clear, String texture_filename) : this(clr, blend, clear)
        {
            LoadTexture(texture_filename);
        }


        public int LoadTexture(String filename)
        {
            try
            {
                if (String.IsNullOrEmpty(filename))
                {
                    textureid = -1;
                    return textureid;
                }

//              GL.GenTextures(1, out textureid);
                textureid = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureid);

//                Console.WriteLine(filename + " textid: " + textureid);

                Bitmap bmp = new Bitmap(filename);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                /*
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
                    Glu.Build2DMipmap(TextureTarget.Texture2D, (int)PixelInternalFormat.Three, TextureBitmap.Width, TextureBitmap.Height, PixelFormat.Bgr, PixelType.UnsignedByte, TextureData.Scan0);
                */

                return textureid;
            }
            catch (Exception) { textureid = -1; return textureid; }
        }

    }
}
