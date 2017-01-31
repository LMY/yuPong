using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace yuPong
{
    abstract class Input
    {
        public abstract PongMove GetInput(GameStateMatch game, Player player, Character c, int[] cooldowns, KeyboardDevice k, int time);
    }


    class InputHuman : Input
    {
        public static InputHuman input1 = new InputHuman(KeyMap.KeyMap_player1, true);
        public static InputHuman input2 = new InputHuman(KeyMap.KeyMap_player2, false);


        private KeypressQueue queue;

        public InputHuman(KeyMap keymap, bool p1)
        {

            queue = new KeypressQueue(keymap, p1);
        }


        public override PongMove GetInput(GameStateMatch game, Player player, Character c, int[] cooldowns, KeyboardDevice k, int time)
        {
            return queue.GetInput(game, player, c, cooldowns, k, time);
        }
    }


    class InputAI : Input
    {
        public InputAI() { }

        public override PongMove GetInput(GameStateMatch game, Player player, Character c, int[] cooldowns, KeyboardDevice k, int time)
        {
            return AI.SuggestForce(game.isPlayer1(player) ? 0 : 1, game.theLevel, game.Balls, game.Pads[game.isPlayer1(player) ? 0 : 1]);
        }
    }



    class Keypress
    {
        public enum KeypressType { NULL = 0, RIGHT, LEFT, FRONT, BACK, UP, DOWN, SPECIAL1, SPECIAL2, PAUSE, MAINMENU, GAMEMENU, DEBUG };
        public enum KeypressState { NOT_SPECIFIED = 0, RELEASE, HOLD, PRESS };

        public static Keypress KeyNULL = new Keypress(KeypressType.NULL, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyFRONT = new Keypress(KeypressType.FRONT, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyBACK = new Keypress(KeypressType.BACK, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyLEFT = new Keypress(KeypressType.LEFT, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyRIGHT = new Keypress(KeypressType.RIGHT, KeypressState.NOT_SPECIFIED);
        public static Keypress KeySPECIAL1 = new Keypress(KeypressType.SPECIAL1, KeypressState.NOT_SPECIFIED);
        public static Keypress KeySPECIAL2 = new Keypress(KeypressType.SPECIAL2, KeypressState.NOT_SPECIFIED);

        public static Keypress KeyPAUSE = new Keypress(KeypressType.PAUSE, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyMAINMENU = new Keypress(KeypressType.MAINMENU, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyGAMEMENU = new Keypress(KeypressType.GAMEMENU, KeypressState.NOT_SPECIFIED);
        public static Keypress KeyDEBUG = new Keypress(KeypressType.DEBUG, KeypressState.NOT_SPECIFIED);


        private KeypressType keypresstype;
        public KeypressType Type { get { return keypresstype; } set { keypresstype = value; } }

        private KeypressState commandstate;
        public KeypressState State { get { return commandstate; } set { commandstate = value; } }


        public Keypress(KeypressType type, KeypressState state)
        {
            keypresstype = type;
            commandstate = state;
        }

        public Keypress(Keypress cmd, KeypressState state)
        {
            keypresstype = cmd.Type;
            commandstate = state;
        }

        public String toString()
        {
            String s = "";

            if (keypresstype == KeypressType.NULL) s += "0";
            else if (keypresstype == KeypressType.RIGHT) s += "R";
            else if (keypresstype == KeypressType.LEFT) s += "L";
            else if (keypresstype == KeypressType.FRONT) s += "F";
            else if (keypresstype == KeypressType.BACK) s += "B";
            else if (keypresstype == KeypressType.UP) s += "U";
            else if (keypresstype == KeypressType.DOWN) s += "D";
            else if (keypresstype == KeypressType.SPECIAL1) s += "1";
            else if (keypresstype == KeypressType.SPECIAL2) s += "2";

//            s += " ";
            /*
            if (commandstate == CommandState.HOLD) s += "h";
            else if (commandstate == CommandState.PRESS) s += "!";
            else if (commandstate == CommandState.RELEASE) s += ".";
            else if (commandstate == CommandState.NOT_SPECIFIED) s += "?!";
            */
            return s;
        }
    }


    class KeyMap
    {
        private static KeyMap keymap_system = new KeyMap(-1);
        private static KeyMap keymap_p0 = new KeyMap(0);
        private static KeyMap keymap_p1 = new KeyMap(1);

        public static KeyMap KeyMap_player1 { get { return keymap_p0; } set { keymap_p0 = value; } }
        public static KeyMap KeyMap_player2 { get { return keymap_p1; } set { keymap_p1 = value; } }
        public static KeyMap KeyMap_system { get { return keymap_system; } set { keymap_system = value; } }


        public Key KeyBACK { get { return KeyFor(Keypress.KeypressType.BACK); } }
        public Key KeyFRONT { get { return KeyFor(Keypress.KeypressType.FRONT); } }
        public Key KeyLEFT { get { return KeyFor(Keypress.KeypressType.LEFT); } }
        public Key KeyRIGHT { get { return KeyFor(Keypress.KeypressType.RIGHT); } }
        public Key KeySPECIAL1 { get { return KeyFor(Keypress.KeypressType.SPECIAL1); } }

        private Keypress.KeypressType[] commands;
        private Key[] keys;

        private bool[] kxpressed;

        public KeyMap()
        {
        }

        public KeyMap(int player)
            : this()
        {
            loadPlayer(player);
        }


        public Key KeyFor(Keypress.KeypressType command)
        {
            for (int i = 0; i < commands.Length; i++)
                if (commands[i] == command)
                    return keys[i];

            return Key.Number9;
        }

        public void loadPlayer(int player)
        {
            if (player == 0 || player == 1)
            {
                keys = new Key[6];
                commands = new Keypress.KeypressType[6];

                commands[0] = Keypress.KeypressType.BACK;
                commands[1] = Keypress.KeypressType.FRONT;
                commands[2] = Keypress.KeypressType.LEFT;
                commands[3] = Keypress.KeypressType.RIGHT;
                commands[4] = Keypress.KeypressType.SPECIAL1;
                commands[5] = Keypress.KeypressType.SPECIAL2;

                if (player == 0)
                {
                    keys[0] = Key.Left;
                    keys[1] = Key.Right;
                    keys[2] = Key.Up;
                    keys[3] = Key.Down;
                    keys[4] = Key.Insert;
                    keys[5] = Key.Delete;
                }
                else if (player == 1)
                {
                    keys[0] = Key.D;
                    keys[1] = Key.A;
                    keys[2] = Key.S;
                    keys[3] = Key.W;
                    keys[4] = Key.Space;
                    keys[5] = Key.Number2;
                }
            }
            else if (player == -1)
            {
                keys = new Key[4];
                commands = new Keypress.KeypressType[4];

                commands[0] = Keypress.KeypressType.PAUSE;
                commands[1] = Keypress.KeypressType.GAMEMENU;
                commands[2] = Keypress.KeypressType.MAINMENU;
                commands[3] = Keypress.KeypressType.DEBUG;

                keys[0] = Key.P;
                keys[1] = Key.F1;
                keys[2] = Key.F5;
                keys[3] = Key.Y;
            }

            kxpressed = new bool[commands.Length];
            for (int i = 0; i < kxpressed.Length; i++)
                kxpressed[i] = false;
        }

        public Keypress[] translateInput(GameStateMatch game, KeyboardDevice keyboard)
        {
            Keypress[] cmds = new Keypress[keys.Length];
            int i = 0;  // length/first_free index in cmds array

            for (int k = 0; k < keys.Length; k++)
                if (keyboard[keys[k]])
                {
                    if (kxpressed[k])
                        cmds[i++] = new Keypress(commands[k], Keypress.KeypressState.HOLD);
                    else
                    {
                        kxpressed[k] = true;
                        cmds[i++] = new Keypress(commands[k], Keypress.KeypressState.PRESS);
                    }
                }
                else
                {
                    if (kxpressed[k])
                    {
                        kxpressed[k] = false;
                        cmds[i++] = new Keypress(commands[k], Keypress.KeypressState.RELEASE);
                    }
                }

            if (i == 0)
                return null;

            Keypress[] cmds2 = new Keypress[i];
            int p = 0;
            for (int k = 0; k < keys.Length; k++)
                if (cmds[k] != null)
                    cmds2[p++] = cmds[k];

            return cmds2;
        }
    }



    class KeypressQueue
    {
        private LinkedList<Keypress> keypress;
        public LinkedList<Keypress> MoveList { get { return keypress; } set { keypress = value; } }

        private LinkedList<int> times;
        public LinkedList<int> KeypressTimes { get { return times; } set { times = value; } }

        private KeyMap input;

        private PongMove[] pongmoves;


        public Keypress getLastCommand()
        {
            if (keypress.Count == 0)
                return Keypress.KeyNULL;

            return keypress.First.Value;
        }


        public KeypressQueue(KeyMap the_input, bool p1)
        {
            keypress = new LinkedList<Keypress>();
            times = new LinkedList<int>();

            input = the_input;

            pongmoves = new PongMove[4] { new PongMoveMovementBACK(p1), new PongMoveMovementFRONT(p1), new PongMoveMovementLEFT(p1), new PongMoveMovementRIGHT(p1) };
        }

        public void clear()
        {
            keypress.Clear();
            times.Clear();
        }

        public void addKeypress(Keypress c, int time)
        {
            keypress.AddFirst(c);
            times.AddFirst(time);
        }


        public static int persistence_time = (int)(40*0.75f);    // 1/deltaT = 40 = 1sec

        public void NewTime(int time)
        {
            while (times.Count > 0  &&  time > persistence_time + times.Last.Value)
            {
                times.RemoveLast();
                keypress.RemoveLast();
            }
        }


        public PongMove GetInput(GameStateMatch game, Player player, Character c, int[] cooldowns, KeyboardDevice k, int time)
        {
            NewTime(time);

            Keypress[] newmoves = input.translateInput(game, k);

            if (newmoves == null)    // no new commands
                return new PongMoveNULL();

            for (int i = 0; i < newmoves.Length; i++)
                    addKeypress(newmoves[i], time);


            Keypress[] validmoves = CurrentMoveList();

/*
            Console.Write("Commands# " + validmoves.Length+"   :");

            for (int i = 0; i < validmoves.Length; i++)
                Console.Write(" " + validmoves[i].toString());

            Console.WriteLine();
*/
            if (validmoves.Length == 0)
                return new PongMoveNULL();

            LinkedList<PongMoveSpecial> specialmoves = c.SpecialMoves;
            for (int i = 0; i < specialmoves.Count; i++)
                if (cooldowns[i] == 0)
                    if (specialmoves.ElementAt(i).isTriggeredByKeySequence(validmoves))
                    {
                        clear();
                        PongMoveSpecial fired_specialmove = specialmoves.ElementAt(i);
                        cooldowns[i] = fired_specialmove.CoolDown;
                        return fired_specialmove;
                    }

            
            if (newmoves.Length == 1)
            {
                for (int i = 0; i < pongmoves.Length; i++)
                    if (pongmoves[i].getKeySequence()[0].Type == newmoves[0].Type)
                        return pongmoves[i];
            }
            else
            {
                LinkedList<PongMove> moves = new LinkedList<PongMove>();

                for (int w = 0; w < newmoves.Length; w++)
                    if (newmoves[w].State == Keypress.KeypressState.PRESS || newmoves[w].State == Keypress.KeypressState.HOLD)
                        for (int i = 0; i < pongmoves.Length; i++)
                            if (pongmoves[i].getKeySequence()[0].Type == newmoves[w].Type)
                                moves.AddLast(pongmoves[i]);
                
                return new PongMoveComposite(moves);
            }


            return new PongMoveNULL();
        }
        /*
        public bool CommandAtIs(int time, Keypress c)
        {
            for (int i=0; i<times.Count; i++)
                if (times.ElementAt(i) == time)
                    if (keypress.ElementAt(i).Type == c.Type && (keypress.ElementAt(i).State == Keypress.KeypressState.PRESS || keypress.ElementAt(i).State == Keypress.KeypressState.HOLD))
                        return true;

            return false;
        }*/



        public Keypress[] CurrentMoveList()
        {
            LinkedList<Keypress> valid = new LinkedList<Keypress>();

            for (int i = 0; i < keypress.Count; i++)
            {
                Keypress c = keypress.ElementAt(i);

                if (c.State == Keypress.KeypressState.PRESS)
                    valid.AddFirst(c);
//                else if (c.State == Command.CommandState.HOLD)
//                    valid.AddLast(c);
//                else if (c.State == Command.CommandState.RELEASE)
//                    valid.AddLast(c);
            }

            return valid.ToArray<Keypress>();
        }
    }



    abstract class PongMove
    {
        public enum PongMoveType { NULL = 0, MOVEMENT, SPECIAL, FATALITY, COMPOSITE, SYSTEM };

        private PongMoveType movetype;
        public PongMoveType Movetype { get { return movetype; } set { movetype = value; } }

        public PongMove(PongMoveType type)
        {
            movetype = type;
        }

        public PongMove() : this(PongMove.PongMoveType.NULL) { }

        public abstract void Apply(GameStateMatch g, Player p);
        
        public virtual Keypress[] getKeySequence()
        {
            Keypress[] kp = new Keypress[1];
            kp[0] = Keypress.KeyNULL;
            return kp;
        }
    }



    class PongMoveNULL : PongMove
    {
        public PongMoveNULL() : base(PongMoveType.NULL) {}

        public override void Apply(GameStateMatch g, Player p) { }
    }


    class PongMoveComposite : PongMove
    {
        private LinkedList<PongMove> moves;
        public LinkedList<PongMove> Moves { get { return moves; } set { moves = value; } }
        public void add(PongMove m) { moves.AddLast(m); }

        public PongMoveComposite() : base(PongMoveType.COMPOSITE)                       { moves = new LinkedList<PongMove>(); }
        public PongMoveComposite(LinkedList<PongMove> m) : base(PongMoveType.COMPOSITE) { moves = m; }

        public override void Apply(GameStateMatch g, Player p)
        {
            foreach (PongMove m in moves)
                m.Apply(g, p);
        }

        public override Keypress[] getKeySequence()
        {
            LinkedList<Keypress> kp = new LinkedList<Keypress>();

            for (int i = 0; i < moves.Count; i++)
            {
                Keypress[] nkp = moves.ElementAt(i).getKeySequence();
                for (int k = 0; k < nkp.Length; k++)
                    kp.AddLast(nkp.ElementAt(k));
            }

            return kp.ToArray<Keypress>();
        }
    }

    class PongMovePause : PongMove
    {
        public PongMovePause(Vector3 force)
            : base(PongMoveType.SYSTEM)
        {
        }

        public override void Apply(GameStateMatch g, Player p)
        {
            g.Pause = !g.Pause;
        }
    }

    class PongMoveMovement : PongMove
    {
        public static float pad_force_constant = 200000.0f;

        private Vector3 movement;
        public Vector3 Movement { get { return movement; } set { movement = value; } }

        public PongMoveMovement(Vector3 force)
            : base(PongMoveType.MOVEMENT)
        {
            movement = force;
        }

        public override void Apply(GameStateMatch g, Player p)
        {
            bool isp1 = g.isPlayer1(p);
            Pad thepad = g.Pads[isp1 ? 0 : 1];

            thepad.F += movement;
        }
    }


    class PongMoveMovementRIGHT : PongMoveMovement
    {
        public PongMoveMovementRIGHT(bool p1) : base(new Vector3(0, (p1 ? -1 : +1) * pad_force_constant, 0)) { }
        public override Keypress[] getKeySequence() { return new Keypress[1] { Keypress.KeyRIGHT }; }
    }

    class PongMoveMovementLEFT : PongMoveMovement
    {
        public PongMoveMovementLEFT(bool p1) : base(new Vector3(0, (p1 ? +1 : -1) * pad_force_constant, 0)) { }
        public override Keypress[] getKeySequence() { return new Keypress[1] { Keypress.KeyLEFT }; }
    }

    class PongMoveMovementFRONT : PongMoveMovement
    {
        public PongMoveMovementFRONT(bool p1) : base(new Vector3((p1 ? -1 : +1) * pad_force_constant, 0, 0)) { }
        public override Keypress[] getKeySequence() { return new Keypress[1] { Keypress.KeyFRONT }; }
    }

    class PongMoveMovementBACK : PongMoveMovement
    {
        public PongMoveMovementBACK(bool p1) : base(new Vector3((p1 ? +1 : -1) * pad_force_constant, 0, 0)) { }
        public override Keypress[] getKeySequence() { return new Keypress[1] { Keypress.KeyBACK }; }
    }



    abstract class PongMoveSpecial : PongMove
    {
        public PongMoveSpecial()
            : base(PongMoveType.SPECIAL)
        {
        }

        public abstract override void Apply(GameStateMatch g, Player p);


        public bool isTriggeredByKeySequence(Keypress[] validmoves)
        {
            Keypress[] code = getKeySequence();

            if (code.Length <= validmoves.Length)
            {
                for (int i = 0; i < code.Length; i++)
                    if (code[i].Type != validmoves[validmoves.Length - code.Length+i].Type)
                        return false;
            }
            else
                return false;

            return true;
        }

        public static readonly float CoolDown0 = 2;
        public static readonly float CoolDownp = 4;
        public static readonly float CoolDownpp = 6;
        public static readonly float CoolDownm = 1;
        public static readonly float CoolDownmm = 0.5f;

        public abstract int CoolDown { get; }


        protected static Ball create_ball_for(GameStateMatch g, Player p, float duration)
        {
            return create_ball_for(g, p, duration, null);
        }

        protected static Ball create_ball_for(GameStateMatch g, Player p, float duration, GameEventModifyPlayer onball_hit)
        {
            bool isp1 = g.isPlayer1(p);
            g.newBall(p.thePad.Center + Vector3.UnitX * (isp1 ? -1 : +1) * g.Balls[0].Size.X, Vector3.UnitX * (isp1 ? -1 : +1) * Ball.TypicalSpeed * 2.1f, (int)(duration * yPhysics.Instance.fps), onball_hit);
            return g.Balls[g.Balls.Length - 1];
        }
    }


    // WARRY
    class PongMoveSpecial_Warry1 : PongMoveSpecial
    {
        public PongMoveSpecial_Warry1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            for (int i = 0; i < g.Balls.Length; i++)
                g.Balls[i].Speed = Vector3.Zero;
        }
    }

    class PongMoveSpecial_Warry2 : PongMoveSpecial
    {
        public PongMoveSpecial_Warry2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDown0 * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            create_ball_for(g, p, 3);
        }
    }


    class PongMoveSpecial_Warry3 : PongMoveSpecial
    {
        public PongMoveSpecial_Warry3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            bool isp1 = g.isPlayer1(p);

            foreach (Ball b in g.Balls)
                if (b.Speed.X * (isp1 ? +1 : -1) > 0)
                    b.setSpeed(new Vector3(-b.Speed.X, b.Speed.Y, b.Speed.Z));
        }
    }

    // DIA
    class PongMoveSpecial_Dia1 : PongMoveSpecial
    {
        public PongMoveSpecial_Dia1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            g.theEvents.addHot_or_Dot(g.isPlayer1(p) ? g.Player2 : g.Player1, 6, -2);
        }
    }

    class PongMoveSpecial_MirrorMirror : PongMoveSpecial
    {
        public PongMoveSpecial_MirrorMirror() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDown0 * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            foreach (Ball b in g.Balls)
                b.setSpeed(-b.Speed);
        }
    }

    class PongMoveSpecial_Dia3 : PongMoveSpecial
    {
        public PongMoveSpecial_Dia3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            Player target = g.isPlayer1(p) ? g.Player2 : g.Player1;

            g.theEvents.addEvent(new GameEventPlayerChangeFriction(6.0f, target, target.thePad.FrictionConstant));
            target.thePad.FrictionConstant = 4 * target.thePad.FrictionConstant;
        }
    }


    // GAIA
    class PongMoveSpecial_Gaia1 : PongMoveSpecial
    {
        public PongMoveSpecial_Gaia1() : base() {}
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            Ball b = create_ball_for(g, p, 0);

            float len = b.Speed.Length/1.5f;
            float angle = (float)(-Math.PI / 4 + Math.PI / 2 * yPhysics.Random()) + (float)Math.PI * (g.isPlayer1(p) ? 0 : 1);
            b.setSpeed(new Vector3(-len * (float)Math.Cos(angle), len * (float)Math.Sin(angle), 0));
        }
    }

    class PongMoveSpecial_Gaia2 : PongMoveSpecial
    {
        public PongMoveSpecial_Gaia2() : base() {}
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            g.theEvents.addEvent(new GameEventPlayerResize(3.0f, p, p.thePad.Size.Y));
            p.thePad.Size = new Vector3(p.thePad.Size.X, 2*p.thePad.Size.Y, p.thePad.Size.Z);
        }
    }

    class PongMoveSpecial_Gaia3 : PongMoveSpecial
    {
        public PongMoveSpecial_Gaia3() : base() {}
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            float  duration = 3.0f;

            float angle = (float)(-Math.PI / 4 + Math.PI / 2 * yPhysics.Random()) + (float)Math.PI * (g.isPlayer1(p) ? 0 : 1);


            for (int i = 0; i < 2; i++)
            {
                Ball b = create_ball_for(g, p, duration);
                float len = b.Speed.Length / 1.5f;
                b.setSpeed(new Vector3(-len * (float)Math.Cos(angle), len * (float)Math.Sin(angle), 0));

                angle *= -1;
            }
        }
    }



    // PORTAL
    class PongMoveSpecial_Portal1 : PongMoveSpecial
    {
        public PongMoveSpecial_Portal1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return 0; } }

        public override void Apply(GameStateMatch g, Player p)
        {
            if ((p.thePad.Center.X < 0 && p.thePad.Center.X > -g.theLevel.SizeX / 2 && g.isPlayer1(p)) || (p.thePad.Center.X > 0 && p.thePad.Center.X < g.theLevel.SizeX / 2 && !g.isPlayer1(p)))
                p.thePad.setCenter(p.thePad.Center + (g.isPlayer1(p) ? +1 : -1) * Vector3.UnitX * g.theLevel.SizeX / 2);
        }
    }

    class PongMoveSpecial_Portal2 : PongMoveSpecial
    {
        public PongMoveSpecial_Portal2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return 0; } }

        public override void Apply(GameStateMatch g, Player p)
        {
            if ((p.thePad.Center.X > 0 && p.thePad.Center.X < g.theLevel.SizeX / 2 && g.isPlayer1(p)) || (p.thePad.Center.X < 0 && p.thePad.Center.X > -g.theLevel.SizeX / 2 && !g.isPlayer1(p)))
                p.thePad.setCenter(p.thePad.Center - (g.isPlayer1(p) ? +1 : -1) * Vector3.UnitX * g.theLevel.SizeX / 2);
        }
    }

    class PongMoveSpecial_Portal3 : PongMoveSpecial
    {
        public PongMoveSpecial_Portal3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDown0 * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            /*
            foreach (Ball b in g.Balls)
            {
                if ((b.Center.X > 0 && b.Center.X < g.theLevel.SizeX / 2 && g.isPlayer1(p)) || (b.Center.X < 0 && b.Center.X > -g.theLevel.SizeX / 2 && !g.isPlayer1(p)))
                {
                    b.Center = new Vector3(-b.Center.X, b.Center.Y, b.Center.Z);
                    b.Speed = new Vector3(-b.Speed.X, b.Speed.Y, b.Speed.Z);
                }
            }*/

            bool p1 = g.isPlayer1(p);
            Player target = p1 ? g.Player2 : g.Player1;
            const float border = 1.2f;

            float randx = (p1?-1:+1)*g.theLevel.SizeX/4 + 2*(yPhysics.Random()-1) * (g.theLevel.SizeX/2 - target.thePad.Size.X/2 - border);
            float randy =                               + 2*(yPhysics.Random()-1) * (g.theLevel.SizeY/2 - target.thePad.Size.Y/2 - border);

            Ball b = create_ball_for(g, p, 3.0f, new GameEventTeleportInOwnSpace(0, target, g));
        }
    }


    // WTF
    class PongMoveSpecial_WTF1 : PongMoveSpecial
    {
        public PongMoveSpecial_WTF1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return 0; } }

        public override void Apply(GameStateMatch g, Player p)
        {
            p.thePad.Visible = !p.thePad.Visible;
        }
    }

    class PongMoveSpecial_WTF2 : PongMoveSpecial
    {
        public PongMoveSpecial_WTF2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            const float duration = 8.0f;

            float angle = (float) (2*Math.PI * yPhysics.Random());
            float len = 7.0f;

            for (int i = 0; i < 4; i++)
                g.newBall(Vector3.Zero, new Vector3(len * (float)Math.Cos(angle + i * Math.PI / 2), len * (float)Math.Sin(angle + i * Math.PI / 2), 0), (int)(duration * yPhysics.Instance.fps));
        }
    }

    class PongMoveSpecial_WTF3 : PongMoveSpecial
    {
        public PongMoveSpecial_WTF3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            int limit = g.Balls.Length;

            for (int i = 0; i < limit; i++)
            {
                Ball b = create_ball_for(g, p, g.Balls[i].Age * yPhysics.Instance.DT);
                b.setCenter(new Vector3(-g.Balls[i].Center.X, g.Balls[i].Center.Y, g.Balls[i].Center.Z));
                b.setSpeed(new Vector3(-g.Balls[i].Speed.X, g.Balls[i].Speed.Y, g.Balls[i].Speed.Z));
            }
        }
    }

    class PongMoveSpecial_WTF4 : PongMoveSpecial
    {
        public PongMoveSpecial_WTF4() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            const float lendiff = 1.2f;

            bool isp1 = g.isPlayer1(p);
            Vector3 pmin = new Vector3(p.thePad.MaxPoint.X - g.theLevel.SizeX, p.thePad.MinPoint.Y - lendiff * p.thePad.Size.Y, p.thePad.MinPoint.Z);
            Vector3 pmax = new Vector3(p.thePad.MaxPoint.X, p.thePad.MaxPoint.Y + lendiff * p.thePad.Size.Y, p.thePad.MaxPoint.Z);


            FieldEventInvisible finside = new FieldEventInvisible();
            FieldEventVisible finexit = new FieldEventVisible();

            g.theFieldlist.addField(new FieldRadial(6.0f, finside, finexit, finside, Vector3.Zero, g.theLevel.SizeY/3));
        }
    }




    // RETARDIN
    class PongMoveSpecial_Retardin1 : PongMoveSpecial
    {
        public PongMoveSpecial_Retardin1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            g.theEvents.addEvent(new GameEventPlayerChangeFriction(6.0f, p, p.thePad.FrictionConstant));
            p.thePad.FrictionConstant = 0.5f * p.thePad.FrictionConstant;
        }
    }

    class PongMoveSpecial_Retardin2 : PongMoveSpecial
    {
        public PongMoveSpecial_Retardin2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            p.Healthbar.HP += (int)(p.theCharacter.MaxHp * 0.15f);

            if (p.Healthbar.HP > p.Healthbar.MAXHP)
                p.Healthbar.HP = p.Healthbar.MAXHP;
        }
    }

    class PongMoveSpecial_Retardin3 : PongMoveSpecial
    {
        public PongMoveSpecial_Retardin3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            Ball b = create_ball_for(g, p, 10);

            float len = b.Speed.Length / 1.5f;
            float angle = (float)(-Math.PI / 4 + Math.PI / 2 * yPhysics.Random()) + (float)Math.PI * (g.isPlayer1(p) ? 0 : 1);
            b.setSpeed(new Vector3(-len * (float)Math.Cos(angle), len * (float)Math.Sin(angle), 0));
        }
    }


    // OOM
    class PongMoveSpecial_OOM1 : PongMoveSpecial
    {
        public PongMoveSpecial_OOM1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            g.theEvents.addHot_or_Dot(p, 6, 4);
        }
    }

    class PongMoveSpecial_OOM2 : PongMoveSpecial
    {
        public PongMoveSpecial_OOM2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            p.Healthbar.HP += (int)(p.theCharacter.MaxHp * 0.10f);

            if (p.Healthbar.HP > p.Healthbar.MAXHP)
                p.Healthbar.HP = p.Healthbar.MAXHP;
        }
    }

    class PongMoveSpecial_OOM3 : PongMoveSpecial
    {
        public PongMoveSpecial_OOM3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            Ball b = create_ball_for(g, p, 10.0f);

            float len = b.Speed.Length / 1.5f;
            float angle = (float)(-Math.PI / 4 + Math.PI / 2 * yPhysics.Random()) + (float)Math.PI * (g.isPlayer1(p) ? 0 : 1);
            b.setSpeed(new Vector3(-len * (float)Math.Cos(angle), len * (float)Math.Sin(angle), 0));

            foreach (Ball ba in g.Balls)
                if (ba.Age < yPhysics.Instance.fps * 10.0f && ba.Age >= 0)
                    ba.Age = (int) (yPhysics.Instance.fps * 10.0f);                    
        }
    }


    // NINI
    class PongMoveSpecial_Nini1 : PongMoveSpecial
    {
        public PongMoveSpecial_Nini1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            for (int s=-1; s<=+1; s+=2)
            {
                Ball b = create_ball_for(g, p, 4.0f);
                b.setCenter(b.Center + new Vector3(0, s*p.thePad.Size.Y/2*0.7f, 0));
            }
        }
    }

    class PongMoveSpecial_Nini2 : PongMoveSpecial
    {
        public PongMoveSpecial_Nini2() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyFRONT, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownpp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            bool p1 = g.isPlayer1(p);
            Player target = p1 ? g.Player2 : g.Player1;
            Ball b = create_ball_for(g, p, 3.0f, new GameEventSlowForPeriod(0, target, g, 24.0f));
        }
    }

    class PongMoveSpecial_Nini3 : PongMoveSpecial
    {
        public PongMoveSpecial_Nini3() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDownp * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
        }
    }


    class PongMoveSpecial_Fusrodah1 : PongMoveSpecial
    {
        public PongMoveSpecial_Fusrodah1() : base() { }
        public override Keypress[] getKeySequence() { return new Keypress[] { Keypress.KeyBACK, Keypress.KeyBACK, Keypress.KeyFRONT, Keypress.KeySPECIAL1 }; }
        public override int CoolDown { get { return (int)(CoolDown0 * yPhysics.Instance.fps); } }

        public override void Apply(GameStateMatch g, Player p)
        {
            const float lendiff = 1.2f;
            const float F = 60.0f;

            bool isp1 = g.isPlayer1(p);
            Vector3 pmin = new Vector3(p.thePad.MaxPoint.X - g.theLevel.SizeX, p.thePad.MinPoint.Y - lendiff * p.thePad.Size.Y, p.thePad.MinPoint.Z);
            Vector3 pmax = new Vector3(p.thePad.MaxPoint.X, p.thePad.MaxPoint.Y + lendiff * p.thePad.Size.Y, p.thePad.MaxPoint.Z);

            FieldEventForceConst fe = new FieldEventForceConst(new Vector3((isp1 ? -1 : +1) * F, 0, 0));

            g.theFieldlist.addField(new FieldBox(1.0f, fe, fe, fe, pmin, pmax));
        }
    }

    /*
     * Dia
        special1: circle of teleport, cast
        special2: circle of teleport, teleport
     
     * Subzero
        stats: Hp0, Speed++, Len-
        special1: speeder ball. if opponent touches it, iceball is destroyed but opponent freezed _2?_ secs
        special2: stop all balls
        special3: icepit. in the enemy space a icearea is created, lasts _6?_ secs, if a pad enters the area is frozen _3?_ secs
      
     * Liu Kang
        stats: Hp0, Speed++, Len-
        appearance: black red stripe, red black stripe
        special1: hurl ball
        special2: hurl self
        special3: hurl all the things (force field x direction, duration _2?_ secs)
     
     * Nini
        special3: same as 2, but switch positions of the created ball and ball[0]... :P
     */
}
