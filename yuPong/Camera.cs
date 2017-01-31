using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace yuPong
{
    class Camera
    {
        protected float alpha;
        protected float gamma;

        protected Vector3 from;

        public Camera()
        {
            from = Vector3.Zero;
            alpha = 0.0f;
            gamma = 0.00f;
        }

        public float Alpha { get { return alpha; } set { alpha = value; } }
        public float Gamma { get { return gamma; } set { gamma = value; } }

        public Vector3 From { get { return from; } set { from = value; } }
        public Vector3 To
        {
            get
            {
                return Vector3.Add(from, new Vector3((float)Math.Cos((float)alpha) * (float)Math.Sin((float)gamma),
                                                     (float)Math.Sin((float)alpha),
                                                     (float)Math.Cos((float)alpha) * (float)Math.Cos((float)gamma)));
            }
        }

        public Vector3 Up
        {
            get
            {
                return new Vector3((float)-Math.Sin((float)alpha) * (float)Math.Sin((float)gamma),
                                   (float)+Math.Cos((float)alpha),
                                   (float)-Math.Sin((float)alpha) * (float)Math.Cos((float)gamma));
            }
        }


        public void Strafe(float distance)
        {
            float k = distance * 0.5f;
            Vector3 perp = Vector3.Cross(To - From, Vector3.UnitY) + From;
            from = from * (1 - k) + perp * k;
        }

        public void Walk(float distance)
        {
            float k = distance * 0.5f;
            from = from * (1 - k) + To * k;
        }


        public void apply()
        {
            Matrix4 modelview = Matrix4.LookAt(From, To, Up);

//            GL.MatrixMode(MatrixMode.Projection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }
    }
}
