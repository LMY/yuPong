using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;


namespace yuPong
{
    class AI
    {
        public static PongMoveMovement SuggestForce(int playern, Level lvl, Ball[] balls, Pad pad)
        {
            float force_constant = 40f * pad.Mass;

            int bestball = -1;
            float best_deltax = float.PositiveInfinity;

            for (int i=0; i<balls.Length; i++)
                if (balls[i].Speed.X < 0 && playern == 1 || balls[i].Speed.X > 0 && playern == 0)
                {
                    if (Math.Abs(balls[i].Center.X - pad.Center.X) <= pad.Size.X*1.002f)
                        continue;


                    float this_deltax = Math.Abs(balls[i].Center.X - pad.Center.X);

                    if (this_deltax > lvl.SizeX * 0.4f)
                        continue;

                    if (bestball < 0 || this_deltax < best_deltax)      // if first valid found or closest one
                    {
                        bestball = i;
                        best_deltax = this_deltax;
                    }
                }

            if (bestball < 0)
            {
                return new PongMoveMovement(new Vector3(0, force_constant * -pad.Center.Y, 0));
            }
            else
            {
                float kk = -(pad.Center.Y - balls[bestball].Center.Y);
                return new PongMoveMovement(new Vector3(0, 2*force_constant*kk, 0));
            }
        }
    }
}
