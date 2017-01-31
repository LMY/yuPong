using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace yuPong
{
    class Player
    {
        private Healthbar healthbar;

        private Pad pad;
        private GameStateMatch game;
        private Input input;
        private Character character;
        public int[] cooldowns;

        public Healthbar Healthbar { get { return healthbar; } set { healthbar = value; healthbar.reset_to(character.MaxHp); } }

        public Pad thePad { get { return pad; } set { pad = value; pad.Size = new Vector3(pad.Size.X, character.Length, pad.Size.Z); } }
        public GameStateMatch theGame { get { return game; } set { game = value; } }
        public Character theCharacter { get { return character; } set { character = value; } }
        public Input theInput { get { return input; } set { input = value; } }
        public Appearance theAppearance { get { return thePad.theAppearance; } set { thePad.theAppearance = value; } }
        public int[] Cooldowns { get { return cooldowns; } set { cooldowns = value; } }



        public Player(GameStateMatch g, Character c, Pad p, Input i, Appearance app)
        {
            game = g;

            character = c;
            thePad = p;
            input = i;
            thePad.theAppearance = app;
            cooldowns = new int[character.SpecialMovesN];

            healthbar = null;
        }


        public PongMove getInput(GameStateMatch game, KeyboardDevice Keyboard)
        {
            // decrease cooldowns
            for (int i = 0; i < cooldowns.Length; i++)
                if (cooldowns[i] > 0)
                    cooldowns[i]--;

            return theInput.GetInput(game, this, character, cooldowns, Keyboard, yPhysics.Instance.FrameN);
        }


        public Pad createPad(GameStateMatch g, int appnumber)
        {
            return new Pad(g.theLevel, g.isPlayer1(this) ? 0 : 1, character.DefaultAppearance);
        }
    }



    abstract class Character
    {
        private Appearance default_appearance;
        public Appearance DefaultAppearance { get { return default_appearance; } set { default_appearance = value; } }

        private LinkedList<Appearance> appearances;
        public LinkedList<Appearance> Appearances { get { return appearances; } set { appearances = value; } }

        protected LinkedList<PongMoveSpecial> special_moves;
        public LinkedList<PongMoveSpecial> SpecialMoves { get { return special_moves; } set { special_moves = value; } }
        public int SpecialMovesN { get { return special_moves.Count; } }

        public abstract int MaxHp { get; }
        public abstract float Length { get; }
        public abstract float MaxSpeed { get; }

        public static readonly int HP0 = 100;
        public static readonly int HPp = HP0 * 12 / 10;
        public static readonly int HPpp = HP0 * 15 / 10;
        public static readonly int HPm = HP0 * 8 / 10;
        public static readonly int HPmm = HP0 * 65 / 100;

        public static readonly float Speed0 = 20;
        public static readonly float Speedp = Speed0 * 12 / 10;
        public static readonly float Speedpp = Speed0 * 15 / 10;
        public static readonly float Speedm = Speed0 * 8 / 10;
        public static readonly float Speedmm = Speed0 * 65 / 100;

        public static readonly float Length0 = 2.25f;
        public static readonly float Lengthp = Length0 * 12 / 10;
        public static readonly float Lengthpp = Length0 * 15 / 10;
        public static readonly float Lengthm = Length0 * 8 / 10;
        public static readonly float Lengthmm = Length0 * 65 / 100;

        public Character()
        {
            special_moves = new LinkedList<PongMoveSpecial>();
            appearances = new LinkedList<Appearance>();

            add_appearances();
            add_special_moves();
        }

        protected abstract void add_special_moves();
        protected abstract void add_appearances();
    }




    class CharacterPong : Character
    {
        public CharacterPong() : base() { }

        public override int MaxHp { get { return Character.HPpp; } }
        public override float MaxSpeed { get { return Character.Speedp; } }
        public override float Length { get { return Character.Lengthp; } }

        protected override void add_special_moves()
        {}

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterPongAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                            DefaultAppearance.Color.A)));
        }
    }




    class CharacterWarry : Character
    {
        public CharacterWarry() : base() {}

        public override int MaxHp       { get { return Character.HP0; } }
        public override float MaxSpeed { get { return Character.Speedp; } }
        public override float Length { get { return Character.Length0; } }
        
        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Warry1());
            special_moves.AddLast(new PongMoveSpecial_Warry2());
            special_moves.AddLast(new PongMoveSpecial_Warry3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterWarryAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                            DefaultAppearance.Color.A)));
        }
    }

    class CharacterDia : Character
    {
        public CharacterDia() : base() { }

        public override int MaxHp { get { return Character.HP0; } }
        public override float MaxSpeed { get { return Character.Speed0; } }
        public override float Length { get { return Character.Length0; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Dia1());
            special_moves.AddLast(new PongMoveSpecial_MirrorMirror());
            special_moves.AddLast(new PongMoveSpecial_Dia3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterDiaAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                            DefaultAppearance.Color.A)));
        }
    }


    class CharacterGaia : Character
    {
        public CharacterGaia() : base() { }

        public override int MaxHp { get { return Character.HPp; } }
        public override float MaxSpeed { get { return Character.Speedm; } }
        public override float Length { get { return Character.Lengthp; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Gaia1());
            special_moves.AddLast(new PongMoveSpecial_Gaia2());
            special_moves.AddLast(new PongMoveSpecial_Gaia3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterGaiaAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                            Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                            DefaultAppearance.Color.A)));
        }
    }

    class CharacterPortal : Character
    {
        public CharacterPortal() : base() { }

        public override int MaxHp { get { return Character.HP0; } }
        public override float MaxSpeed { get { return Character.Speed0; } }
        public override float Length { get { return Character.Lengthp; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Portal1());
            special_moves.AddLast(new PongMoveSpecial_Portal2());
            special_moves.AddLast(new PongMoveSpecial_Portal3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterPortalAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }

    class CharacterWTF : Character
    {
        public CharacterWTF() : base() { }

        public override int MaxHp { get { return Character.HPmm; } }
        public override float MaxSpeed { get { return Character.Speed0; } }
        public override float Length { get { return Character.Lengthpp; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_WTF1());
            special_moves.AddLast(new PongMoveSpecial_WTF2());
            special_moves.AddLast(new PongMoveSpecial_WTF3());
            special_moves.AddLast(new PongMoveSpecial_WTF4());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterWTFAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }

    class CharacterRetardin : Character
    {
        public CharacterRetardin() : base() { }

        public override int MaxHp { get { return Character.HPpp; } }
        public override float MaxSpeed { get { return Character.Speedm; } }
        public override float Length { get { return Character.Length0; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Retardin1());
            special_moves.AddLast(new PongMoveSpecial_Retardin2());
            special_moves.AddLast(new PongMoveSpecial_Retardin3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterRetardinAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }

    class CharacterOOM : Character
    {
        public CharacterOOM() : base() { }

        public override int MaxHp { get { return Character.HPpp; } }
        public override float MaxSpeed { get { return Character.Speedmm; } }
        public override float Length { get { return Character.Lengthp; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_OOM1());
            special_moves.AddLast(new PongMoveSpecial_OOM2());
            special_moves.AddLast(new PongMoveSpecial_OOM3());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterOOMAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }

    class CharacterNini : Character
    {
        public CharacterNini() : base() { }

        public override int MaxHp { get { return Character.HPm; } }
        public override float MaxSpeed { get { return Character.Speedpp; } }
        public override float Length { get { return Character.Length0; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Nini1());
            special_moves.AddLast(new PongMoveSpecial_Nini2());
            special_moves.AddLast(new PongMoveSpecial_Nini3());
        }
        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterNiniAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }


    class CharacterFusrodah : Character
    {
        public CharacterFusrodah() : base() { }

        public override int MaxHp { get { return Character.HPp; } }
        public override float MaxSpeed { get { return Character.Speedm; } }
        public override float Length { get { return Character.Lengthp; } }

        protected override void add_special_moves()
        {
            special_moves.AddLast(new PongMoveSpecial_Warry2());
            special_moves.AddLast(new PongMoveSpecial_Warry3());
            special_moves.AddLast(new PongMoveSpecial_Fusrodah1());
        }

        protected override void add_appearances()
        {
            DefaultAppearance = Appearance.CharacterFusrodahAppearance;
            Appearances.AddLast(DefaultAppearance);
            Appearances.AddLast(new Appearance(new Color4(Math.Max(DefaultAppearance.Color.R - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.G - 0.1f, 0),
                                                Math.Max(DefaultAppearance.Color.B - 0.1f, 0),
                                                DefaultAppearance.Color.A)));
        }
    }
}

