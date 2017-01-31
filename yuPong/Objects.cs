using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;


namespace yuPong
{
    public abstract class Object
    {
        private float mass;
        public float Mass { get { return mass; } set { mass = value; } }

        private bool movable;
        public bool Movable { get { return movable; } set { movable = value; } }

        private bool visible;
        public bool Visible { get { return visible; } set { visible = value; } }


        protected Vector3 center;
        public Vector3 Center { get { return center; } set { center = value; } }

        protected Vector3 center0;
        public Vector3 Center0 { get { return center0; } set { center0 = value; } }

        protected Vector3 speed;
        public Vector3 Speed { get { return speed; } set { speed = value; } }

        protected Vector3 speed0;
        public Vector3 Speed0 { get { return speed0; } set { speed0 = value; } }
        

        protected Vector3 a;
        public Vector3 A { get { return a; } set { a = value; } }

        private float w;
        public float W { get { return w; } set { w = value; } }

        private Vector3 f;
        public Vector3 F { get { return f; } set { f = value; } }


        public void setSpeed(Vector3 newspeed) { speed0 = speed = newspeed; }
        public void setCenter(Vector3 newcenter) { center0 = center = newcenter; }



        public void verlet(float dt)
        {
            verlet(dt, 0);
        }

        public virtual void verlet(float dt, float friction_constant)
        {
            if (!movable) return;
            else
            {
                center0 = center;   // save previous position
                speed0 = speed;     // save previous speed

                a = F * (1 / Mass) - speed * friction_constant;
                center += speed * dt + a * (float)Math.Pow(dt, 2) / 2;
                speed += a * dt;

                F = Vector3.Zero;
            }
        }



        public Object() : this(Vector3.Zero, Vector3.Zero) { }
        public Object(Vector3 c) : this(c, Vector3.Zero) { }
        public Object(Vector3 c, Vector3 s)
        {
            mass = 1;
            movable = true;
            visible = true;
            setCenter(c);
            setSpeed(s);
            a = Vector3.Zero;
            f = Vector3.Zero;
            w = 0;
        }

        public abstract void draw();
    }



    class Box : Object
    {
        protected Vector3 size;
        public Vector3 Size { get { return size; } set { size = value; } }

        public Vector3 MinPoint { get { return Vector3.Subtract(Center, Vector3.Multiply(size, 0.5f)); } }
        public Vector3 MaxPoint { get { return Vector3.Add(Center, Vector3.Multiply(size, 0.5f)); } }


        private Appearance appearance;
        public Appearance theAppearance { get { return appearance; } set { appearance = value; } }


        public Box(Appearance app) : this(app, Vector3.Zero) { }
        public Box(Appearance app, Vector3 c) : base(c)
        {
            size = new Vector3(1, 1, 1);
            appearance = app; // Color4.DarkSlateBlue;
        }


        public override void draw()
        {
            if (!Visible)
                return;


            Vector3 hx = new Vector3(Size.X/2, 0, 0);
            Vector3 hy = new Vector3(0, Size.Y / 2, 0);
            Vector3 hz = new Vector3(0, 0, Size.Z / 2);

            if (appearance != null && appearance.hasTextureID())
                GL.BindTexture(TextureTarget.Texture2D, appearance.TextureID);
            else
                GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Begin(BeginMode.Quads);
            GL.BlendColor(appearance.ColorBlend); GL.ClearColor(appearance.ColorClear); GL.Color4(appearance.Color); 

            GL.Normal3(0, 1, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(center - hx + hy - hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center - hx + hy + hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center + hx + hy + hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center + hx + hy - hz);

            GL.Normal3(0, -1, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(center - hx - hy - hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center - hx - hy + hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center + hx - hy + hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center + hx - hy - hz);

            GL.Normal3(-1, 0, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(center - hx - hy - hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center - hx - hy + hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center - hx + hy + hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center - hx + hy - hz);

            GL.Normal3(1, 0, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(center + hx - hy - hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center + hx - hy + hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center + hx + hy + hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center + hx + hy - hz);

            GL.Normal3(0, 0, -1);
            GL.TexCoord2(0, 0); GL.Vertex3(center - hx - hy - hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center + hx - hy - hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center + hx + hy - hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center - hx + hy - hz);

            GL.Normal3(0, 0, +1);
            GL.TexCoord2(0, 0); GL.Vertex3(center - hx - hy + hz);
            GL.TexCoord2(0, 1); GL.Vertex3(center + hx - hy + hz);
            GL.TexCoord2(1, 1); GL.Vertex3(center + hx + hy + hz);
            GL.TexCoord2(1, 0); GL.Vertex3(center - hx + hy + hz);
            GL.End();
/*
            const float scale_speed = 0.25f;

            GL.Begin(BeginMode.Lines);
            GL.Color4(Color4.Green); GL.Vertex3(Center0);
            GL.Color4(Color4.Green); GL.Vertex3(Center);

            GL.Color4(Color4.Red); GL.Vertex3(Center);
            GL.Color4(Color4.Red); GL.Vertex3(Center + Speed * scale_speed);
            GL.End();
*/
        }

    }


    class Wall : Box
    {
        private Vector3 spacing;
        public Vector3 Spacing { get { return spacing; } set { spacing = value; } }

        public Wall()
            : this(Vector3.Zero) {}

        public Wall(Vector3 c)
            : base(Appearance.WallAppearance, c)
        {
            size = new Vector3(1, 1, 1);
            spacing = new Vector3(1, 1, 1);
        }
        

        public override void draw()
        {

            Vector3 hx = new Vector3(Size.X / 2, 0, 0);
            Vector3 hy = new Vector3(0, Size.Y / 2, 0);
            Vector3 hz = new Vector3(0, 0, Size.Z / 2);

            Vector3 dx = new Vector3(Spacing.X, 0, 0);
            Vector3 dy = new Vector3(0, Spacing.Y, 0);
            Vector3 dz = new Vector3(0, 0, Spacing.Z);


            int nx = (int)Math.Ceiling(Size.X / Spacing.X);
            int ny = (int)Math.Ceiling(Size.Y / Spacing.Y);
            int nz = (int)Math.Ceiling(Size.Z / Spacing.Z);

            if (theAppearance != null && theAppearance.hasTextureID())
                GL.BindTexture(TextureTarget.Texture2D, theAppearance.TextureID);
            else
                GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Begin(BeginMode.Quads);

            GL.BlendColor(theAppearance.ColorBlend); GL.ClearColor(theAppearance.ColorClear); GL.Color4(theAppearance.Color); 

            for (int y = 0; y < ny; y++)
            {
                for (int s=-1; s<=1; s+=2)  // sign loop
                for (int x = 0; x < nx; x++)
                {
                    GL.Normal3(0, 0, s);
                    Vector3 x0 = center - hx - hy - s * hz;

                    Vector3 xmin = x * dx; // (x == nx - 1 ? (x + 1) * dx : x * dx);
                    Vector3 ymin = y * dy; // (y == ny - 1 ? (y + 1) * dy : y * dy);
                    Vector3 xplus = (x == nx - 1  ?  2 * hx  :  (x + 1) * dx);
                    Vector3 yplus = (y == ny - 1  ?  2 * hy  :  (y + 1) * dy);

                    
                    GL.TexCoord2((float)(y + 0) / ny, (float)(x + 0) / nx); GL.Vertex3(x0 + xmin + ymin);
                    GL.TexCoord2((float)(y + 0) / ny, (float)(x + 1) / nx); GL.Vertex3(x0 + xplus + ymin);
                    GL.TexCoord2((float)(y + 1) / ny, (float)(x + 1) / nx); GL.Vertex3(x0 + xplus + yplus);
                    GL.TexCoord2((float)(y + 1) / ny, (float)(x + 0) / nx); GL.Vertex3(x0 + xmin + yplus);
                    /*
                    GL.TexCoord2(0, 0); GL.Vertex3(x0 + xmin + ymin);
                    GL.TexCoord2(0, 1); GL.Vertex3(x0 + xplus + ymin);
                    GL.TexCoord2(1, 1); GL.Vertex3(x0 + xplus + yplus);
                    GL.TexCoord2(1, 0); GL.Vertex3(x0 + xmin + yplus);
                    */
                }


                for (int s = -1; s <= 1; s += 2)  // sign loop
                    for (int z = 0; z < nz; z++)
                    {
                        GL.Normal3(s, 0, 0);
                        Vector3 x0 = center - s * hx - hy - hz;

                        Vector3 zmin = z * dz; // (z == nz - 1 ? (z + 1) * dz : z * dz);
                        Vector3 ymin = y * dy; // (y == ny - 1 ? (y + 1) * dy : y * dy);
                        Vector3 zplus = (z == nz - 1 ? 2 * hz : (z + 1) * dz);
                        Vector3 yplus = (y == ny - 1 ? 2 * hy : (y + 1) * dy);
                        
                        GL.TexCoord2((float)(y + 0) / ny, (float)(z + 0) / nz); GL.Vertex3(x0 + zmin + ymin);
                        GL.TexCoord2((float)(y + 0) / ny, (float)(z + 1) / nz); GL.Vertex3(x0 + zplus + ymin);
                        GL.TexCoord2((float)(y + 1) / ny, (float)(z + 1) / nz); GL.Vertex3(x0 + zplus + yplus);
                        GL.TexCoord2((float)(y + 1) / ny, (float)(z + 0) / nz); GL.Vertex3(x0 + zmin + yplus);
                        /*
                        GL.TexCoord2(0, 0); GL.Vertex3(x0 + zmin + ymin);
                        GL.TexCoord2(0, 1); GL.Vertex3(x0 + zplus + ymin);
                        GL.TexCoord2(1, 1); GL.Vertex3(x0 + zplus + yplus);
                        GL.TexCoord2(1, 0); GL.Vertex3(x0 + zmin + yplus);
                        */
                    }
            }


            for (int x = 0; x < nx; x++)
            {
                for (int s = -1; s <= 1; s += 2)  // sign loop
                    for (int z = 0; z < nz; z++)
                    {
                        GL.Normal3(0, 2, 0);
                        Vector3 x0 = center - hx - s * hy - hz;

                        Vector3 xmin = x * dx; // (x == nx - 1 ? (x + 1) * dx : x * dx);
                        Vector3 zmin = z * dz; // (z == nz - 1 ? (z + 1) * dz : z * dz);
                        Vector3 zplus = (z == nz - 1 ? 2 * hz : (z + 1) * dz);
                        Vector3 xplus = (x == nx - 1 ? 2 * hx : (x + 1) * dx);
                        GL.TexCoord2((float)(x + 0) / nx, (float)(z + 0) / nz); GL.Vertex3(x0 + zmin + xmin);
                        GL.TexCoord2((float)(x + 0) / nx, (float)(z + 1) / nz); GL.Vertex3(x0 + zplus + xmin);
                        GL.TexCoord2((float)(x + 1) / nx, (float)(z + 1) / nz); GL.Vertex3(x0 + zplus + xplus);
                        GL.TexCoord2((float)(x + 1) / nx, (float)(z + 0) / nz); GL.Vertex3(x0 + zmin + xplus);

                        /*
                        GL.TexCoord2(0, 0); GL.Vertex3(x0 + zmin + xmin);
                        GL.TexCoord2(0, 1); GL.Vertex3(x0 + zplus + xmin);
                        GL.TexCoord2(1, 1); GL.Vertex3(x0 + zplus + xplus);
                        GL.TexCoord2(1, 0); GL.Vertex3(x0 + zmin + xplus);
                         */
                    }
            }

            GL.End();
        }
    }


    class Ball : Box
    {
        public Ball() : base(Appearance.BallAppearance) { age = -1;  Size = new Vector3(0.2f, 0.2f, 0.2f); Mass = 1; }
        public Ball(int max_age) : base(Appearance.BallAppearance) { age = max_age; Size = new Vector3(0.2f, 0.2f, 0.2f); Mass = 1; }

        private int age;
        public int Age { get { return age; } set { age = value; } }
        public void DecreaseAge() { if (age > 0) age--; }


        public const float TypicalSpeed = 7;

        public void StartingSpeed()
        {

            float r = yPhysics.Random();
            float angle;

            if (r < 0.5f)
                angle = (float) (2*r * Math.PI / 2 - Math.PI / 4);
            else
                angle = (float) (2 * (r-0.5f) * Math.PI / 2 + 3* Math.PI / 4);


            Speed = new Vector3((float)(TypicalSpeed * Math.Cos(angle)), (float)(TypicalSpeed * Math.Sin(angle)), 0);
        }

        public virtual bool hitWall(GameStateMatch game, Box collidingObj) { return false; }    // return true to delete ball
        public virtual bool hitPlayer(GameStateMatch game, Player targetplayer) { return false; }    // return true to delete ball
    }



    class Pad : Box
    {
        private int playn;

        private float friction_constant;
        private static readonly float friction_constant_default = 18.0f;

        public float FrictionConstant { get { return friction_constant; } set { friction_constant = value; } }


        public Pad(Level level, int playerNumber, Appearance app)
            : base(app)
        {
            const float rear_spacing = 1.0f;
            friction_constant = Pad.friction_constant_default;

            Size = new Vector3(0.2f, 2.0f, 0.6f);

            playn = playerNumber;
            if (playerNumber == 0)
                Center = new Vector3(level.SizeX / 2 - rear_spacing - Size.X / 2, 0, 0);
            else if (playerNumber == 1)
                Center = new Vector3(-level.SizeX / 2 + rear_spacing + Size.X / 2, 0, 0);
            
            setLight();

            Mass = 500;
        }

        public override void verlet(float dt, float friction_constant)
        {
            base.verlet(dt, FrictionConstant);
        }

        public void setLight()
        {
            LightName lightname = LightName.Light0;
            EnableCap enablename = EnableCap.Light0;


            if (playn == 1)
            {
                lightname = LightName.Light1;
                enablename = EnableCap.Light1;
            }
            else if (playn == 2)
            {
                lightname = LightName.Light2;
                enablename = EnableCap.Light2;
            }
            else if (playn == 3)
            {
                lightname = LightName.Light3;
                enablename = EnableCap.Light3;
            }
            else if (playn == 4)
            {
                lightname = LightName.Light4;
                enablename = EnableCap.Light4;
            }

            if (!Visible)
            {
                GL.Disable(enablename);
                return;
            }

            
            const float k_amb = 0.6f;
            const float k_diff = 0.3f;
            const float k_spec = 1.0f;

            Color4 Color = theAppearance.Color;

            Color4 light_ambient = new Color4(Color.R * k_amb, Color.G * k_amb, Color.B * k_amb, 1);
            Color4 light_diffuse = new Color4(Color.R * k_diff, Color.G * k_diff, Color.B * k_diff, 1);
            Color4 light_specular = new Color4(k_spec, k_spec, k_spec, 1.0f);
            Vector4 light_pos = new Vector4(Center.X, Center.Y, Center.Z-4, 1);
            Vector4 light_dir = new Vector4(0, 0, 1, 1);

            GL.Light(lightname, LightParameter.Ambient, light_ambient);
            GL.Light(lightname, LightParameter.Diffuse, light_diffuse);
            GL.Light(lightname, LightParameter.Specular, light_specular);
            GL.Light(lightname, LightParameter.Position, light_pos);
            GL.Light(lightname, LightParameter.SpotDirection, light_dir);
            GL.Light(lightname, LightParameter.SpotCutoff, 30.0f);
            GL.Enable(enablename);
        }
    }


    class Level
    {
        private LinkedList<Box> boxes;

        private Wall box_up;
        private Wall box_down;
        private Wall box_right;
        private Wall box_left;
        private Wall box_floor;
        private Wall box_limit;

        public Wall BoxUp { get { return box_up; } }
        public Wall BoxDown { get { return box_down; } }
        public Wall BoxRight { get { return box_right; } }
        public Wall BoxLeft { get { return box_left; } }
        public Wall BoxFloor { get { return box_floor; } }
        public Wall BoxLimit { get { return box_limit; } }


        private float sizex;
        private float sizey;
        private float sizez;

        public float SizeX { get { return sizex; } set { sizex = value; } }
        public float SizeY { get { return sizey; } set { sizey = value; } }
        public float SizeZ { get { return sizez; } set { sizez = value; } }


        public Level()
        {
            boxes = new LinkedList<Box>();
            sizex = 19.0f;
            sizey = 10.0f;
            sizez = 1.5f;

            initialize(0);
        }

        public void draw()
        {
            for (int i = 0; i < boxes.Count-1; i++)
                boxes.ElementAt(i).draw();
        }

        public void initialize(int map)
        {
            const float border_size = 0.5f;
            const float floor_size = 0.1f;

            Color4 border_color = new Color4(0.2f, 0.2f, 0.2f, 0.5f);

            box_up = new Wall(new Vector3(0, (sizey + border_size) / 2, 0));
            box_up.Size = new Vector3(sizex + 2 * border_size, border_size, sizez);
            box_up.Mass = float.PositiveInfinity;
            boxes.AddLast(box_up);

            box_down = new Wall(new Vector3(0, -(sizey + border_size) / 2, 0));
            box_down.Size = new Vector3(sizex + 2 * border_size, border_size, sizez);
            box_down.Mass = float.PositiveInfinity;
            boxes.AddLast(box_down);

            box_right = new Wall(new Vector3(-(sizex + border_size) / 2, 0, 0));
            box_right.Size = new Vector3(border_size, sizey + 2 * border_size, sizez);
            box_right.Mass = float.PositiveInfinity;
            boxes.AddLast(box_right);

            box_left = new Wall(new Vector3(+(sizex + border_size) / 2, 0, 0));
            box_left.Size = new Vector3(border_size, sizey + 2 * border_size, sizez);
            box_left.Mass = float.PositiveInfinity;
            boxes.AddLast(box_left);

            // initialize floor
            box_floor = new Wall(new Vector3(0, 0, (sizez + floor_size) / 2));
            box_floor.theAppearance = Appearance.GroundAppearance;
            box_floor.Size = new Vector3(sizex, sizey, floor_size);
            box_floor.Mass = float.PositiveInfinity;
            boxes.AddLast(box_floor);

            box_limit = new Wall(Vector3.Zero);
            box_limit.Size = new Vector3(border_size, sizey + 2 * border_size, sizez);
            box_limit.Mass = float.PositiveInfinity;
            boxes.AddLast(box_limit);
        }

        public void verlet(float dt)
        {
            // if u uncomment these, all the boxes will be falling...
//            Vector3 g = new Vector3(0, -9.81f, 0);

//            for (int i = 0; i < boxes.Count; i++)
//                boxes.ElementAt(i).verlet(g, dt);
        }
    }




    class Healthbar : Box
    {
        private bool reverse;
        public bool Reverse { get { return reverse; } set { reverse = value; } }

        private int hp;
        public int HP { get { return hp; } set { hp = value; } }

        private int maxhp;
        public int MAXHP { get { return maxhp; } set { maxhp = value; } }

        private float maxlen;
        public float MAXLEN { get { return maxlen; } set { maxlen = value; } }


        public Healthbar() : base(Appearance.HB1Appearance) { reverse = false; hp = maxhp = 100; maxlen = Size.X; }
        public Healthbar(Vector3 c) : base(Appearance.HB1Appearance, c) { reverse = false; hp = maxhp = 100; maxlen = Size.X; }
        public Healthbar(Vector3 c, bool rev, Vector3 size) : base(Appearance.HB1Appearance, c) { reverse = rev; hp = maxhp = 100; Size = size; maxlen = Size.X; }

        public int increase(int value)
        {
            hp += value;
            if (hp < 0) hp = 0;
            if (hp > maxhp) maxhp = hp;

            return hp;
        }


        public void reset_to(int new_maxhp)
        {
            maxhp = hp = new_maxhp;
        }


        public override void draw()
        {
            if (hp == maxhp)
                theAppearance = Appearance.HB1Appearance;
            else if ((float)hp / maxhp > 0.50f)
                theAppearance = Appearance.HB2Appearance;
            else if ((float)hp / maxhp > 0.30f)
                theAppearance = Appearance.HB3Appearance;
            else if ((float)hp / maxhp > 0.20f)
                theAppearance = Appearance.HB4Appearance;
            else if ((float)hp / maxhp > 0.10f)
                theAppearance = Appearance.HB5Appearance;
            else if ((float)hp / maxhp > 0)
                theAppearance = Appearance.HB6Appearance;
            else
                theAppearance = Appearance.HB7Appearance;

            Size = new Vector3(maxlen*(float)hp / maxhp, Size.Y, Size.Z);

            base.draw();
        }
    }
}
