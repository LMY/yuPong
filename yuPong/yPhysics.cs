using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yuPong
{
    class yPhysics
    {
        private static yPhysics instance;
        public static yPhysics Instance { get { return instance; } }

        private float dt;
        private float g;
        private float katt;
        private float kattg;
        private Random random;

        public float DT { get { return dt; } set { dt = value; } }
        public int fps { get { return dt==0?0: (int)(1 / dt); } }

        public float G { get { return g; } set { g = value; } }
        public float Katt { get { return katt; } set { katt = value; } }
        public float Kattg { get { return kattg; } set { kattg = value; } }


        private int framen;
        public int FrameN { get { return framen; } set { framen = value; } }
        private float Time { get { return dt * FrameN; } }


        public int Beat()
        {
            return ++framen;
        }

        public void Restart()
        {
            framen = 0;
        }


        private yPhysics()
        {
            random = new Random();
            g = 9.81f;
            katt = 0.25f;
            kattg = 0.1f;

            framen = 0;
        }


        public static void init()
        {
            instance = new yPhysics();
        }

        public static float Random()
        {
            return (float) Instance.random.NextDouble();
        }
    }
}
