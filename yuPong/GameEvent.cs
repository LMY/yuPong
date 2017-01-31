using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;


namespace yuPong
{
    class ExpiringObject
    {
       private int time;
       public int TimeLeft { get { return time; } set { time = value; } }

   
       public ExpiringObject(float duration)        // ExpiringObject(-1) means "forever"
       {
           time = duration > 0 ? (int)(yPhysics.Instance.fps * duration) : -1;
       }
   
       public bool check_expires()
       {
           if (time > 0)
               return (--time == 0);
           else
               return false;
       }

       public bool is_expired()
       {
           return (time==0);
       }
    }




    abstract class FieldEvent
    {
       public FieldEvent() {}

       public abstract void apply(Box target, GameStateMatch g);
    }



    class FieldEventNull : FieldEvent
    {
       public FieldEventNull() : base() {}
       public override void apply(Box target, GameStateMatch g) { }
    }



    class FieldEventForceConst : FieldEvent
    {
       private Vector3 force;
       public Vector3 Force { get { return force; } set { force = value; } }

   
       public FieldEventForceConst(Vector3 f) : base()
       {
           force = f;
       }

       public override void apply(Box target, GameStateMatch g)
       {
           target.F += force;
       }
    }



    class FieldEventForceRadial : FieldEvent
    {
       private Vector3 center;
       private float force;

       public Vector3 Center { get { return center; } set { center = value; } }
       public float Force { get { return force; } set { force = value; } }

   
       public FieldEventForceRadial(Vector3 c, float f) : base()
       {
           center = c;
           force = f;        
       }

       public override void apply(Box target, GameStateMatch g)
       {
           target.F = ((float)(force/Math.Pow((target.Center-center).Length, 3))) * (target.Center-center);    // F = force (a-b)/|a-b|^3
       }
    }


    class FieldEventInvisible: FieldEvent
    {
       public FieldEventInvisible() : base()
       {}

       public override void apply(Box target, GameStateMatch g)
       {
           if (target.Visible)
           {
               target.Visible = false;
               g.theEvents.addEvent(new GameEventVisibilize(yPhysics.Instance.DT * 3, target, true));
           }
       }
    }

    class FieldEventVisible : FieldEvent
    {
       public FieldEventVisible(): base()
       {}

       public override void apply(Box target, GameStateMatch g)
       {
           target.Visible = true;
       }
    }



    abstract class Field : ExpiringObject
    {
       private FieldEvent eenter;
       private FieldEvent eexit;
       private FieldEvent estay;

       private bool affect_pads;
       private bool affect_balls;

   
       public FieldEvent EventEnter { get { return eenter; } set { eenter = value; } }
       public FieldEvent EventExit { get { return eexit; } set { eexit = value; } }
       public FieldEvent EventStay { get { return estay; } set { estay = value; } }

       public bool AffectPads { get { return affect_pads; } set { affect_pads = value; } }
       public bool AffectBalls { get { return affect_balls; } set { affect_balls = value; } }
   

       public Field(float duration, FieldEvent enter, FieldEvent exit, FieldEvent stay) : base(duration)
       {
           eenter = enter;
           eexit = exit;
           estay = stay;

           affect_pads = true;
           affect_balls = true;
       }


       public void apply_field(GameStateMatch g, Box b)
       {
           bool was_inside = is_inside(b.Center0);
           bool now_inside = is_inside(b.Center);

           if (!was_inside && now_inside)
               object_enters(g, b);
           else if (was_inside && !now_inside)
               object_exits(g, b);
           else if (was_inside && now_inside)
               object_stays(g, b);
       }

       public abstract bool is_inside(Vector3 point);

       public void object_enters(GameStateMatch g, Box b) { if (eenter != null) eenter.apply(b, g); }
       public void object_exits(GameStateMatch g, Box b) { if (eexit != null) eexit.apply(b, g); }
       public void object_stays(GameStateMatch g, Box b) { if (estay != null) estay.apply(b, g); }
    }


    class FieldList
    {
        private LinkedList<Field> fields;
        public LinkedList<Field> Fields { get { return fields; } }

        public FieldList()
        {
            fields = new LinkedList<Field>();
        }


        public void addField(Field f)
        {
            Fields.AddLast(f);
        }


        public void newTime(int time, GameStateMatch g)
        {
            int i = 0;
            
            while (i < fields.Count)
            {
                Field f = fields.ElementAt(i);

                if (!f.check_expires())  //CheckTimeAndFire(time))
                {
                    if (f.AffectPads)
                        for (int k = 0; k < g.Pads.Length; k++)
                            f.apply_field(g, g.Pads[k]);

                    if (f.AffectBalls)
                        for (int k = 0; k < g.Balls.Length; k++)
                            f.apply_field(g, g.Balls[k]);

                    i++;
                }
                else
                    fields.Remove(f);
            }
        }
    }



    class FieldRadial : Field
    {
       private Vector3 center;
       private float radius;
   
       public Vector3 Center { get { return center; } set { center = value; } }
       public float Radius { get { return radius; } set { radius = value; } }


       public FieldRadial(float duration, FieldEvent enter, FieldEvent exit, FieldEvent stay, Vector3 c, float r)
           : base(duration, enter, exit, stay)
       {
           center = c;
           radius = r;
       }

       public override bool is_inside(Vector3 point)
       {
           return ((point-center).Length < radius);
       }
    }


    class FieldBox : Field
    {
       private Vector3 minp;
       private Vector3 maxp;
   
       public Vector3 MinPoint { get { return minp; } set { minp = value; } }
       public Vector3 Center    { get { return (minp+maxp)*0.5f; } }
       public Vector3 MaxPoint { get { return maxp; } set { maxp = value; } }


       public FieldBox(float duration, FieldEvent enter, FieldEvent exit, FieldEvent stay, Vector3 min_point, Vector3 max_point)
           : base(duration, enter, exit, stay)
       {
           minp = min_point;
           maxp = max_point;
       }

       public override bool is_inside(Vector3 point)
       {
           return (point.X >= minp.X && point.X <= maxp.X  &&  point.Y >= minp.Y && point.Y <= maxp.Y  &&  point.Z >= minp.Z && point.Z <= maxp.Z);
       }
    }



    class GameEventList
    {
        private LinkedList<GameEvent> events;
        public LinkedList<GameEvent> Events { get { return events; } }

        public GameEventList()
        {
            events = new LinkedList<GameEvent>();
        }


        public void addEvent(GameEvent e)
        {
            events.AddLast(e);
        }


        public void newTime(int time)
        {
            int i = 0;

            while (i < events.Count)
            {
                GameEvent e = events.ElementAt(i);

                if (e.CheckTimeAndFire(time))
                    events.Remove(e);
                else
                    i++;
            }
        }



        public void addHot_or_Dot(Player target, int nticks, int tick_hp)
        {
            for (int i=0; i<nticks; i++)
                addEvent(new GameEventModifyHealth(i+1, target, tick_hp));
        }
    }


    abstract class GameEvent
    {
        protected int time;
/*        protected Game game;
        protected Player caster;
        protected Player target;
        */
        /*
        public GameEvent(int trigger_time, Game g, Player casterP, Player targetplayer)
        {
            time = trigger_time;
            game = g;
            caster = casterP;
            target = targetplayer;
        }*/
        public GameEvent(float trigger_time)
        {
            time = yPhysics.Instance.FrameN + (int)(trigger_time * yPhysics.Instance.fps);
        }

        public bool CheckTimeAndFire(int newtime)
        {
            if (newtime >= time)
            {
                Apply();
                return true;
            }

            return false;
        }

        public abstract void Apply();
    }

    abstract class GameEventModifyPlayer : GameEvent
    {
        protected Player target;
        public Player Target { get { return target; } set { target = value; } }

        public GameEventModifyPlayer(float trigger_time, Player targetplayer)
            : base(trigger_time)
        {
            target = targetplayer;
        }
    }

    class GameEventVisibilize : GameEvent
    {
        private Box box;
        private bool state;

        public GameEventVisibilize(float trigger_time, Box target, bool s)
            : base(trigger_time)
        {
            state = s;
            box = target;
        }

        public override void Apply()
        {
            box.Visible = state;
        }
    }



    class GameEventModifyHealth : GameEventModifyPlayer
    {
        private int dhp;

        public GameEventModifyHealth(float trigger_time, Player targetplayer, int delta_hp)
            : base(trigger_time, targetplayer)
        {
            dhp = delta_hp;
        }

        public override void Apply()
        {
            target.Healthbar.increase(dhp);
        }
    }

    class GameEventTeleport : GameEventModifyPlayer
    {
        private Vector3 pos;

        public GameEventTeleport(float trigger_time, Player targetplayer, Vector3 where)
            : base(trigger_time, targetplayer) { pos = where; }

        public override void Apply()
        {
            target.thePad.setCenter(pos);
        }
    }

    class GameEventPlayerSpeed : GameEventModifyPlayer
    {
        private Vector3 speed;

        public GameEventPlayerSpeed(float trigger_time, Player targetplayer, Vector3 newspeed)
            : base(trigger_time, targetplayer) { speed = newspeed; }

        public override void Apply()
        {
            target.thePad.setSpeed(speed);
        }
    }


    class GameEventPlayerResize : GameEventModifyPlayer
    {
        private float size;

        public GameEventPlayerResize(float trigger_time, Player targetplayer, float newsize)
            : base(trigger_time, targetplayer) { size = newsize;}

        public override void Apply()
        {
            target.thePad.Size = new Vector3(target.thePad.Size.X, size, target.thePad.Size.Z);
        }
    }


    class GameEventPlayerChangeFriction : GameEventModifyPlayer
    {
        private float friction;

        public GameEventPlayerChangeFriction(float trigger_time, Player targetplayer, float newfriction)
            : base(trigger_time, targetplayer) { friction = newfriction;}

        public override void Apply()
        {
            target.thePad.setSpeed(Vector3.Zero);
            target.thePad.FrictionConstant = friction;
        }
    }



    class GameEventFreeze : GameEventModifyPlayer
    {
        public GameEventFreeze(float trigger_time, Player targetplayer)
            : base(trigger_time, targetplayer) { }

        public override void Apply()
        {
            target.thePad.Mass = float.PositiveInfinity;
            target.thePad.setSpeed(Vector3.Zero);
        }
    }


    class GameEventTeleportInOwnSpace : GameEventModifyPlayer
    {
        private GameStateMatch game;

        public GameEventTeleportInOwnSpace(float trigger_time, Player targetplayer, GameStateMatch thegame)
            : base(trigger_time, targetplayer) { game = thegame; }

        public override void Apply()
        {
            const float border = 1.05f;

            bool p1 = target.thePad.Center.X > 0;

            float randx = (p1 ? +1 : -1) * game.theLevel.SizeX / 4 + 2 * (yPhysics.Random() - 0.5f) * (game.theLevel.SizeX / 4 - target.thePad.Size.X / 2 - border);
            float randy =                                          + 2 * (yPhysics.Random() - 0.5f) * (game.theLevel.SizeY / 2 - target.thePad.Size.Y / 2 - border);

            target.thePad.setSpeed(Vector3.Zero);
            target.thePad.setCenter(new Vector3(randx, randy, 0));
        }
    }


    class GameEventSlowForPeriod : GameEventModifyPlayer
    {
        private GameStateMatch game;
        private float period;


        public GameEventSlowForPeriod(float trigger_time, Player targetplayer, GameStateMatch thegame, float theperiod)
            : base(trigger_time, targetplayer) { game = thegame; period = theperiod;  }

        public override void Apply()
        {
            bool p1 = target.thePad.Center.X > 0;

            game.theEvents.addEvent(new GameEventPlayerChangeFriction(period, target, target.thePad.FrictionConstant));
            target.thePad.FrictionConstant = 4 * target.thePad.FrictionConstant;
        }
    }





    class BastardBall : Ball
    {
        private GameEventModifyPlayer theevent;
        public GameEventModifyPlayer theEvent { get { return theevent; } set { theevent = value; } }

        public BastardBall(GameEventModifyPlayer _event)
            : base()
        {
            theevent = _event;
        }

        public BastardBall(GameEventModifyPlayer _event, int age)
            : base(age)
        {
            theevent = _event;
        }

        //public override bool hitWall(GameStateMatch game, Box collidingObj) { return false; }    // return true to delete ball
        public override bool hitPlayer(GameStateMatch game, Player targetplayer)
        {
            Age = 1;

            theevent.Target = targetplayer;
            theevent.Apply();

            return true;    // return true to delete ball
        }
    }
}
