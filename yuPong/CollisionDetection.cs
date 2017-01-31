using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;





namespace yuPong
{
    // this is the object where a result for a collision is stored
    // keeps track of: collision or not, colliding obj and target, position and speed after collision (x1, speed) and modified x0 to calculate next collisions (if any)
    class CollisionInformations
    {
        public enum CollisionStatus { NO_COLLISION = 0, COLLISION_FIXED = 1, COLLISION_MOVING = 2 };


        private CollisionStatus collision_type;
        private float t;
        private Box colliding_obj;
        private Box target_obj;

        private Vector3 suggested_x0;
        private Vector3 suggested_x1;
        private Vector3 suggested_p0;
        private Vector3 suggested_p1;

        private Vector3 suggested_speed0;
        private Vector3 suggested_speed1;
        private Vector3 suggested_speed20;
        private Vector3 suggested_speed21;

        private Vector3 suggested_av;
        private Vector3 suggested_aw;

        public CollisionStatus CollisionType { get { return collision_type; } set { collision_type = value; } }
        public float CollisionT { get { return t; } set { t = value; } }
        public Box CollidingObj { get { return colliding_obj; } set { colliding_obj = value; } }
        public Box TargetObj { get { return target_obj; } set { target_obj = value; } }

        public Vector3 SuggestedX0 { get { return suggested_x0; } set { suggested_x0 = value; } }
        public Vector3 SuggestedX1 { get { return suggested_x1; } set { suggested_x1 = value; } }
        public Vector3 SuggestedP0 { get { return suggested_p0; } set { suggested_p0 = value; } }
        public Vector3 SuggestedP1 { get { return suggested_p1; } set { suggested_p1 = value; } }

        public Vector3 SuggestedSpeed0 { get { return suggested_speed0; } set { suggested_speed0 = value; } }
        public Vector3 SuggestedSpeed1 { get { return suggested_speed1; } set { suggested_speed1 = value; } }
        public Vector3 SuggestedSpeed20 { get { return suggested_speed20; } set { suggested_speed20 = value; } }
        public Vector3 SuggestedSpeed21 { get { return suggested_speed21; } set { suggested_speed21 = value; } }

        public Vector3 AV { get { return suggested_av; } set { suggested_av = value; } }
        public Vector3 AW { get { return suggested_aw; } set { suggested_aw = value; } }

        public bool Colliding { get { return (collision_type != CollisionStatus.NO_COLLISION); } }


        // use this to create an instance that indicates no collision
        public static CollisionInformations NOT_COLLIDING { get { return new CollisionInformations(); } }



        private CollisionInformations()
        {
            collision_type = CollisionStatus.NO_COLLISION;
            suggested_speed0 = Vector3.Zero;
            suggested_speed1 = Vector3.Zero;
            suggested_x0 = Vector3.Zero;
            suggested_x1 = Vector3.Zero;

            suggested_speed20 = Vector3.Zero;
            suggested_speed21 = Vector3.Zero;
            suggested_p0 = Vector3.Zero;
            suggested_p1 = Vector3.Zero;

            suggested_av = Vector3.Zero;
            suggested_aw = Vector3.Zero;

            colliding_obj = null;
            target_obj = null;

            t = 1;  // whole deltaT passed
        }

        public CollisionInformations(Vector3 x0p, Vector3 x1p, Vector3 spd1, float tc)
        {
            collision_type = CollisionStatus.COLLISION_FIXED;
            suggested_speed0 = spd1;
            suggested_speed1 = spd1;
            suggested_x0 = x0p;
            suggested_x1 = x1p;

            suggested_speed20 = Vector3.Zero;
            suggested_speed21 = Vector3.Zero;
            suggested_p0 = Vector3.Zero;
            suggested_p1 = Vector3.Zero;

            suggested_av = Vector3.Zero;
            suggested_aw = Vector3.Zero;

            colliding_obj = null;
            target_obj = null;

            t = tc;
        }

        public CollisionInformations(CollisionStatus status, Vector3 x0p, Vector3 x1p, Vector3 spd1)
        {
            collision_type = status;
            suggested_speed0 = spd1; 
            suggested_speed1 = spd1;
            suggested_x0 = x0p;
            suggested_x1 = x1p;

            suggested_speed20 = Vector3.Zero;
            suggested_speed21 = Vector3.Zero;
            suggested_p0 = Vector3.Zero;
            suggested_p1 = Vector3.Zero;

            suggested_av = Vector3.Zero;
            suggested_aw = Vector3.Zero;

            colliding_obj = null;
            target_obj = null;

            t = 1;
        }

        public CollisionInformations(CollisionStatus status, Vector3 x0p, Vector3 x1p, Vector3 spd1, Box obj, Box target, float tc)
        {
            collision_type = status;
            suggested_speed0 = spd1; 
            suggested_speed1 = spd1;
            suggested_x0 = x0p;
            suggested_x1 = x1p;

            suggested_speed20 = Vector3.Zero;
            suggested_speed21 = Vector3.Zero;
            suggested_p0 = Vector3.Zero;
            suggested_p1 = Vector3.Zero;

            suggested_av = Vector3.Zero;
            suggested_aw = Vector3.Zero;

            colliding_obj = obj;
            target_obj = target;

            t = tc;
        }

        public CollisionInformations(CollisionStatus status, Box obj, Box target, float tc, Vector3 x0p, Vector3 x1p, Vector3 p0p, Vector3 p1p, Vector3 speed0, Vector3 speed1, Vector3 speed20, Vector3 speed21, Vector3 av, Vector3 aw)
        {
            collision_type = status;
            colliding_obj = obj;
            target_obj = target;
            t = tc;

            suggested_x0 = x0p;
            suggested_x1 = x1p;
            suggested_p0 = p0p;
            suggested_p1 = p1p;

            suggested_av = av;
            suggested_aw = aw;

            suggested_speed0 = speed0;
            suggested_speed1 = speed1;
            suggested_speed20 = speed20;
            suggested_speed21 = speed21;
        }


        public void Apply()
        {
            if (!Colliding) return;
            
            if (colliding_obj != null)
            {
                colliding_obj.Center = suggested_x1;
                colliding_obj.Center0 = suggested_x0;
                colliding_obj.Speed = suggested_speed1;
                colliding_obj.Speed0 = suggested_speed0;

                colliding_obj.A = suggested_av;
            }

            if (target_obj != null)
            {
                target_obj.Center = suggested_p1;
                target_obj.Center0 = suggested_p0;
                target_obj.Speed = suggested_speed21;
                target_obj.Speed0 = suggested_speed20;

                target_obj.A = suggested_aw;
            }
        }

        public bool hasSameObjects(Box obj1, Box obj2)
        {
            return ((colliding_obj == obj1 && target_obj == obj2) || (colliding_obj == obj2 && target_obj == obj1));
//            return ((colliding_obj.Equals(obj1) && target_obj.Equals(obj2)) || (colliding_obj.Equals(obj2) && target_obj.Equals(obj1)));
        }

        public bool hasSameObjects(CollisionInformations c)
        {
            return hasSameObjects(c.CollidingObj, c.TargetObj);
        }
    }


    class CollisionDetection
    {
        // global method
        public static CollisionInformations[] CheckCollisions(Level lvl, Pad[] pads, Ball[] balls)
        {
            LinkedList<CollisionInformations> collisions_done = new LinkedList<CollisionInformations>();

            const int max_iteration = 200;
            const bool collision_between_balls = false;

            int iteration = 0;

            while (true)
            {
                if (++iteration > max_iteration)
                    break;

                CollisionInformations[] collisions = new CollisionInformations[pads.Length + balls.Length + pads.Length*balls.Length + pads.Length*(pads.Length-1)/2 + (collision_between_balls?balls.Length*(balls.Length-1)/2:0) ];
                int collisionn = 0;

                for (int i = 0; i < pads.Length + balls.Length; i++)        // a tricky loop, to do all the balls and all the pads at once
                {
                    CollisionInformations res;

                    if (i < pads.Length)
                        res = CheckCollisionWalls(lvl, (Box)pads[i], true);
                    else
                        res = CheckCollisionWalls(lvl, (Box)balls[i - pads.Length], false);


                    if (res.Colliding)                                      // whenever a collision is found, the collisioninformations object is queued in the collision array
                        collisions[collisionn++] = res;

                    if (i<pads.Length) {
                        for (int b = 0; b < balls.Length; b++)              // check pad<->ball
                        {
                            res = CheckCollisionPadBall(pads[i], balls[b]);

                            if (res.Colliding)
                                collisions[collisionn++] = res;
                        }

                        for (int j = i + 1; j < pads.Length; j++)           // check pad<->pad
                        {
                            res = CheckCollisionsBetweenPads(pads[i], pads[j]);

                            if (res.Colliding)
                                collisions[collisionn++] = res;
                        }
                    }
                }

                if (collision_between_balls)
                    for (int b=0; b<balls.Length; b++)
                        for (int j = b + 1; j < balls.Length; j++)
                        {
                            CollisionInformations res = CheckCollisionBetweenBalls(balls[b], balls[j]);

                            if (res.Colliding)
                                collisions[collisionn++] = res;
                        }


                if (collisionn==0)
                    break;


                int best_i = 0;
                for (int i=1; i<collisionn; i++)
                    if (collisions[i].CollisionT < collisions[best_i].CollisionT)
                    {
                        if (collisions_done.Count > 0)
                        {
                            if (!collisions_done.Last.Value.hasSameObjects(collisions[i]))
                                best_i = i;
                        }
                        else
                            best_i = i;
                    }

                // check if (best_i=0) the only valid collision is the same as the last one Apply()ed
                if (collisions_done.Count > 0)
                    if (collisions_done.Last.Value.hasSameObjects(collisions[best_i]))
                        break;

                collisions[best_i].Apply();
                collisions_done.AddLast(collisions[best_i]);
            }

            return collisions_done.ToArray<CollisionInformations>();
        }

        /*
        public static void CheckCollisions_tiplaseres(Level lvl, Pad[] pads, Ball[] balls)
        {
            bool[] pad_moving = new bool[pads.Length];
            for (int i = 0; i < pads.Length; i++)
                pad_moving[i] = !pads[i].Center.Equals(pads[i].Center0);

            // check collisions: pad/walls
            for (int i = 0; i < pads.Length; i++)
                if (pad_moving[i])
                {
                    CollisionInformations res = CheckCollisionWalls(lvl, pads[i]);
                    // you should not check again the same wall...

                    if (res.Colliding)   // ...enjoy (while)
                    {
                        res = CheckCollisionWalls(lvl, pads[i]);
                    }
                }

            // check collisions: ball/walls
            for (int b = 0; b < balls.Length; b++)
            {
                CollisionInformations res = CheckCollisionWalls(lvl, balls[b]);
                if (res.Colliding)   // ...enjoy (while)
                {
                    res = CheckCollisionWalls(lvl, balls[b]);
                }
            }

            // check collisions: pad/pad
            for (int i = 0; i < pads.Length; i++)
                if (pad_moving[i])
                    for (int k = 0; k < pads.Length; k++)
                        if (i != k)
                            CheckCollisionsBetweenPads(pads[k], pads[i]);

            
            // check collisions: pad/ball
            for (int i = 0; i < pads.Length; i++)
            {
                for (int b = 0; b < balls.Length; b++)
                {
                    CollisionInformations res = CheckCollisionPadBall(pads[i], balls[b]);

                    if (res.Colliding)
                    {
                        // check collisions: ball/walls
                        res = CheckCollisionWalls(lvl, balls[b]);
                        if (res.Colliding)
                            res = CheckCollisionWalls(lvl, balls[b]);

                        res = CheckCollisionPadBall(pads[i], balls[b]);
                    }
                }
            }
        }*/

        public static CollisionInformations CheckCollisionsBetweenPads(Pad pad0, Pad pad1)
        {
            Vector3 size = Vector3.Multiply(Vector3.Add(pad0.Size, pad1.Size), 0.5f);

            Vector3[] v1 = {   -Vector3.UnitX * size.X,
                               Vector3.UnitX * size.X,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitY * size.Y };

            Vector3[] v2 = {   Vector3.UnitY * size.Y,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitX *  size.X,
                               -Vector3.UnitX * size.X };

            Vector3[] v3 = {   -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z,
                               -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z };

            CollisionInformations total_res = CollisionInformations.NOT_COLLIDING;

            for (int i = v1.Length-1; i >= 0; i--)    // reverse... we check the right and left faces first
            {
                CollisionInformations res = CalculateCollision(v1[i], v2[i], v3[i], pad1, pad0, yPhysics.Instance.DT, i == 2);

                if (res.Colliding && res.CollisionT < total_res.CollisionT)
                    total_res = res;
            }

            return total_res;
        }


        public static CollisionInformations CheckCollisionBetweenBalls(Ball ball1, Ball ball2)
        {
            Vector3 size = Vector3.Multiply(Vector3.Add(ball1.Size, ball2.Size), 0.5f);

            Vector3[] v1 = {   -Vector3.UnitX * size.X,
                               Vector3.UnitX * size.X,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitY * size.Y };

            Vector3[] v2 = {   Vector3.UnitY * size.Y,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitX * size.X,
                               -Vector3.UnitX * size.X };

            Vector3[] v3 = {   -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z,
                               -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z };

            CollisionInformations total_res = CollisionInformations.NOT_COLLIDING;

            for (int i = v1.Length - 1; i >= 0; i--)    // reverse... we check the right and left faces first
            {
                CollisionInformations res = CalculateCollision(v1[i], v2[i], v3[i], ball1, ball2, yPhysics.Instance.DT, false);

                if (res.Colliding && res.CollisionT < total_res.CollisionT)
                    total_res = res;
            }

            return total_res;
        }


        public static CollisionInformations CheckCollisionPadBall(Pad pad, Ball ball)
        {
            Vector3 size = Vector3.Multiply(Vector3.Add(pad.Size, ball.Size), 0.5f);

            Vector3[] v1 = {   -Vector3.UnitX * size.X,
                               Vector3.UnitX * size.X,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitY * size.Y };

            Vector3[] v2 = {   Vector3.UnitY * size.Y,
                               -Vector3.UnitY * size.Y,
                               Vector3.UnitX * size.X,
                               -Vector3.UnitX * size.X };

            Vector3[] v3 = {   -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z,
                               -Vector3.UnitZ * size.Z,
                               Vector3.UnitZ * size.Z };

            CollisionInformations total_res = CollisionInformations.NOT_COLLIDING;

            for (int i = v1.Length - 1; i >= 0; i--)    // reverse... we check the right and left faces first
            {
                CollisionInformations res = CalculateCollision(v1[i], v2[i], v3[i], ball, pad, yPhysics.Instance.DT, false);

                if (res.Colliding && res.CollisionT < total_res.CollisionT)
                    total_res = res;
            }

            return total_res;
        }



        public static CollisionInformations CheckCollisionWalls(Level lvl, Box obj, bool ghost_wall)
        {
            // intern face of each lvl.Box*
            Box[] targets = { lvl.BoxDown, lvl.BoxRight, lvl.BoxUp, lvl.BoxLeft, lvl.BoxLimit, lvl.BoxLimit };

            Vector3[] v1 = {   -Vector3.UnitX * (lvl.BoxDown.Size.X+obj.Size.X)/2,
                               -Vector3.UnitY * (lvl.BoxRight.Size.Y+obj.Size.Y)/2,
                               Vector3.UnitX * (lvl.BoxUp.Size.X+obj.Size.X)/2,
                               Vector3.UnitY * (lvl.BoxLeft.Size.Y+obj.Size.Y)/2,
                               Vector3.UnitY * (lvl.BoxLimit.Size.Y+obj.Size.Y)/2,
                               Vector3.UnitY * (lvl.BoxLimit.Size.Y+obj.Size.Y)/2 };

            Vector3[] v2 = {   Vector3.UnitY * (lvl.BoxDown.Size.Y+obj.Size.Y)/2,
                               Vector3.UnitX * (lvl.BoxRight.Size.X+obj.Size.X)/2,
                               -Vector3.UnitY * (lvl.BoxUp.Size.Y+obj.Size.Y)/2,
                               -Vector3.UnitX * (lvl.BoxLeft.Size.X+obj.Size.X)/2,
                               Vector3.UnitX * (lvl.BoxLimit.Size.X+obj.Size.X)/2,
                               -Vector3.UnitX * (lvl.BoxLimit.Size.X+obj.Size.X)/2 };

            Vector3[] v3 = {   -Vector3.UnitZ * (lvl.BoxDown.Size.Z+obj.Size.Z)/2,
                               -Vector3.UnitZ * (lvl.BoxRight.Size.Z+obj.Size.Z)/2,
                               Vector3.UnitZ * (lvl.BoxUp.Size.Z+obj.Size.Z)/2,
                               Vector3.UnitZ * (lvl.BoxLeft.Size.Z+obj.Size.Z)/2,
                               Vector3.UnitZ * (lvl.BoxLimit.Size.Z+obj.Size.Z)/2,
                               Vector3.UnitZ * (lvl.BoxLimit.Size.Z+obj.Size.Z)/2 };

            CollisionInformations total_res = CollisionInformations.NOT_COLLIDING;


            for (int i = 0; i < targets.Length-(ghost_wall?0:2); i++)
            {
                CollisionInformations res = CalculateCollision(v1[i], v2[i], v3[i], obj, targets[i], yPhysics.Instance.DT, false);

                if (res.Colliding && res.CollisionT < total_res.CollisionT)
                    total_res = res;
            }

            return total_res;
        }



        public static CollisionInformations CalculateCollision(Vector3 v1, Vector3 v2, Vector3 v3, Box obj1, Box obj2, float deltaT, bool debug)
        {
            Vector3 q0 = obj2.Center0;

            // change coordinate system: x,z axis are exactly the texture coord, y axis is distance(object, face)
            Matrix4 M = new Matrix4(new Vector4(v1, 0), new Vector4(v2, 0), new Vector4(v3, 0), new Vector4(q0, 1));
            Matrix4 invM;
            try { invM = Matrix4.Invert(M); }
            catch (InvalidOperationException) { return CollisionInformations.NOT_COLLIDING; }

            // position of start and end point in new coordinate space
            Vector4 x0p = Vect4multMatrix4(new Vector4(obj1.Center0, 1), invM);
            Vector4 x1p = Vect4multMatrix4(new Vector4(obj1.Center, 1), invM);
            Vector4 x0w = Vector4.UnitW; // Vect4multMatrix4(new Vector4(obj2.Center0, 1), invM);
            Vector4 x1w = Vect4multMatrix4(new Vector4(obj2.Center, 1), invM);  // p1'=xw   p0'=Vector3.Zero

            if (x0p.Y - 1 < 0 || x1p.Y - (x1w.Y + 1) > 0)        // object starting point and object final point are on opposite sides of the face?
//            if ((x0p.Y - 1)*( x1p.Y - (xw.Y + 1)) > 0)
                return CollisionInformations.NOT_COLLIDING;
            
            // speed
            Vector4 speed0p = Vect4multMatrix4(new Vector4(obj1.Speed0, 0), invM);
            Vector4 speed1p = Vect4multMatrix4(new Vector4(obj1.Speed, 0), invM);
            Vector4 speed20p = Vect4multMatrix4(new Vector4(obj2.Speed0, 0), invM);
            Vector4 speed21p = Vect4multMatrix4(new Vector4(obj2.Speed, 0), invM);

            // a = dv/dt
            Vector4 av = Vect4multMatrix4(new Vector4(obj1.A, 0), invM);
            Vector4 aw = Vect4multMatrix4(new Vector4(obj2.A, 0), invM);

            
            // collision time
            float tc;
            CollisionInformations.CollisionStatus collision_status = CollisionInformations.CollisionStatus.NO_COLLISION;

            if (av.Y - aw.Y == 0)                                 // linear case
            {
                collision_status = CollisionInformations.CollisionStatus.COLLISION_FIXED;

                if (speed0p.Y - speed20p.Y == 0)
                    return CollisionInformations.NOT_COLLIDING;

                tc = deltaT * (1 + x0w.Y - x0p.Y) / (speed0p.Y - speed20p.Y);
                if (tc < 0 || tc > deltaT)                          // if collision is outside the considered period, we found no collisions
                    return CollisionInformations.NOT_COLLIDING;
            }
            else
            {                                                     // calculate [t] solutions for: (av.Y - aw.Y) t^2 + 2(speed0p.Y - speed20p.Y)t + 2*(x0p.Y - 1 -[x0w.Y]) = 0
                collision_status = CollisionInformations.CollisionStatus.COLLISION_MOVING;

                float b = speed20p.Y - speed0p.Y;                                       // -b
                float delta = (float)Math.Pow(b, 2) + 2 * (x0p.Y - 1 - x0w.Y) * (aw.Y - av.Y);  // delta=b^2-ac

                if (delta < 0)
                    return CollisionInformations.NOT_COLLIDING;

                delta = (float)Math.Sqrt(delta);

                if (av.Y - aw.Y > 0)
                {
                    tc = (b - delta) / (av.Y - aw.Y);
                    if (tc > deltaT)
                        return CollisionInformations.NOT_COLLIDING;
                    else if (tc < 0)
                        tc = (b + delta) / (av.Y - aw.Y);
                }
                else
                {
                    tc = (b - delta) / (av.Y - aw.Y);
                    if (tc < 0)
                        return CollisionInformations.NOT_COLLIDING;
                    else if (tc < 0)
                        tc = (b + delta) / (av.Y - aw.Y);
                }

                if (tc < 0 || tc > deltaT)
                    return CollisionInformations.NOT_COLLIDING;

/*
                float tc_1 = (b - delta) / (av.Y - aw.Y);
                float tc_2 = (b + delta) / (av.Y - aw.Y);

                if (tc_1 >= 0 && tc_1 <= deltaT)
                {
                    if (tc_2 >= 0 && tc_2 <= deltaT)
                        tc = Math.Min(tc_1, tc_2);
                    else
                        tc = tc_1;
                }
                else {
                    if (tc_2 >= 0 && tc_2 <= deltaT)
                        tc = tc_2;
                    else
                        return CollisionInformations.NOT_COLLIDING;
                }
*/
                    
//                tc = (b - delta) / (av.Y - aw.Y);
//                if (tc < 0 || tc > deltaT)
//                    tc = (b + delta) / (av.Y - aw.Y);             // keep the minimum positive result
            }

//            Console.WriteLine("tc = " + 100*tc/deltaT);

            float dtc = deltaT - tc;
            float dtc22 = (float)Math.Pow(dtc, 2) / 2;
            float tc22 = (float)Math.Pow(tc, 2) / 2;

            // positions of the two objects at collision time
            Vector4 xc1 = x0p + speed0p * tc + av * tc22;
            Vector4 xc2 = x0w + speed20p * tc + aw * tc22;
            
            // check wheter collision happened outside of the box area
            if (Math.Abs(xc1.X - xc2.X) > 1 || Math.Abs(xc1.Z - xc2.Z) > 1)
                return CollisionInformations.NOT_COLLIDING;

            // speeds of the two objects at collision time
            Vector4 speed1c = speed0p  + av * tc; // Vector4.Lerp(speed0p, speed1p, tc / deltaT);         // speed of target at collision time
            Vector4 speed2c = speed20p + aw * tc; //Vector4.Lerp(speed20p, speed21p, tc/deltaT);       // speed of target at collision time


            // elastic collision
            float m1 = obj1.Mass;
            float m2 = obj2.Mass;
            float mrho11, mrho12, mrho21, mrho22;

            if (m1 == float.PositiveInfinity && m2 == float.PositiveInfinity) { mrho11 =  0; mrho12 = 2; mrho21 = 2; mrho22 =  0; }
            else if (m1 == float.PositiveInfinity)                            { mrho11 = +1; mrho12 = 0; mrho21 = 2; mrho22 = -1; }
            else if (m2 == float.PositiveInfinity)                            { mrho11 = -1; mrho12 = 2; mrho21 = 0; mrho22 = +1; }
            else
            {
                mrho11 = (m1 - m2) / (m1 + m2);
                mrho12 =   2 * m2  / (m1 + m2);
                mrho21 =   2 * m1  / (m1 + m2);
                mrho22 = (m2 - m1) / (m1 + m2);
            }

            // speeds of the objects after the collision
            Vector4 newspeed1c = speed1c;
            Vector4 newspeed2c = speed2c;
            const float attr = 0.095f;
            newspeed1c.X += attr*mrho12 * speed2c.X;
            newspeed1c.Y = mrho11 * speed1c.Y + mrho12 * speed2c.Y;     // -speed1c.Y;
            newspeed1c.Z += attr*mrho12 * speed2c.Z;

            newspeed2c.Y = mrho21 * speed1c.Y + mrho22 * speed2c.Y;     // -speed2c.Y;

            // invert force
//            av.Y = -av.Y;
//            aw.Y = -aw.Y;
            av = Vector4.Zero;
            aw = Vector4.Zero;


            // calculate new final speed, integrating in remaining time
            Vector4 new_init_speed1 = newspeed1c - av * tc;
            Vector4 final_speed1    = newspeed1c + av * dtc;

            Vector4 new_init_speed2 = newspeed2c - aw * tc;
            Vector4 final_speed2    = newspeed2c + aw * dtc;

            // reconvert in space coordinates
            Vector3 suggested_av = Vect4multMatrix4(av, M).Xyz;
            Vector3 suggested_aw = Vect4multMatrix4(aw, M).Xyz;

            Vector3 suggested_speed0 = Vect4multMatrix4(new_init_speed1, M).Xyz;
            Vector3 suggested_speed1 = Vect4multMatrix4(final_speed1, M).Xyz;
            Vector3 suggested_speed20 = Vect4multMatrix4(new_init_speed2, M).Xyz;
            Vector3 suggested_speed21 = Vect4multMatrix4(final_speed2, M).Xyz;


            x1p = xc1 + av * dtc22 + newspeed1c * dtc;
            x1w = xc2 + aw * dtc22 + newspeed2c * dtc;

            // if object1 changes side, hard-adjust the position
            if ((xc1.Y - 1 - xc2.Y) * (x1p.Y - (x1w.Y + 1)) < 0)
                x1p.Y = x1w.Y + 1;

            Vector3 suggested_x0 = Vect4multMatrix4( xc1 - av * tc22  - new_init_speed1 * tc,  M).Xyz;
            Vector3 suggested_x1 = Vect4multMatrix4(x1p, M).Xyz;
            Vector3 suggested_x0w = Vect4multMatrix4(xc2 - aw * tc22 - new_init_speed2 * tc, M).Xyz;
            Vector3 suggested_x1w = Vect4multMatrix4(x1w, M).Xyz;

         //   Console.WriteLine(obj1.Center0+"  -->  "+obj1.Center);


            return new CollisionInformations(collision_status, obj1, obj2, tc,
                        suggested_x0, suggested_x1, suggested_x0w, suggested_x1w,
                        suggested_speed0, suggested_speed1, suggested_speed20, suggested_speed21,
                        suggested_av, suggested_aw);
        }



        public static Vector4 Vect4multMatrix4(Vector4 v, Matrix4 m)
        {
            return new Vector4(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + v.W * m.M41,
                                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + v.W * m.M42,
                                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + v.W * m.M43,
                                v.X * m.M14 + v.Y * m.M24 + v.Z * m.M34 + v.W * m.M44);
        }
    }
}
