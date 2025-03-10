using System;
using System.ComponentModel;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianStressPattern
    {
        private readonly byte _data;

        public RussianStress Main => (RussianStress)(_data & 0x0F);
        public RussianStress Alt => (RussianStress)(_data >> 4);

        internal RussianStressPattern(byte data)
            => _data = data;
        public RussianStressPattern(RussianStress main)
            : this(main, RussianStress.Zero) { }
        public RussianStressPattern(RussianStress main, RussianStress alt)
        {
            if ((uint)main > (uint)RussianStress.Fpp || main == (RussianStress)0b_0111)
                throw new InvalidEnumArgumentException(nameof(main), (int)main, typeof(RussianStress));
            if ((uint)alt > (uint)RussianStress.Fpp || alt == (RussianStress)0b_0111)
                throw new InvalidEnumArgumentException(nameof(alt), (int)alt, typeof(RussianStress));

            _data = (byte)((int)main | ((int)alt << 4));
        }

        public RussianStressPattern(string stressPattern)
            => this = Parse(stressPattern);
        public RussianStressPattern(ReadOnlySpan<char> stressPattern)
            => this = Parse(stressPattern);

        public static RussianStressPattern A => new((int)RussianStress.A);
        public static RussianStressPattern B => new((int)RussianStress.B);
        public static RussianStressPattern C => new((int)RussianStress.C);
        public static RussianStressPattern D => new((int)RussianStress.D);
        public static RussianStressPattern E => new((int)RussianStress.E);
        public static RussianStressPattern F => new((int)RussianStress.F);

        public void Deconstruct(out RussianStress main, out RussianStress alt)
        {
            main = Main;
            alt = Alt;
        }

        public RussianStressPattern NormalizeForNoun()
        {
            var (main, alt) = this;

            if (alt != RussianStress.Zero && alt != main)
                throw new InvalidOperationException(); // TODO: exception

            return new((byte)((int)main | ((int)main << 4)));
        }
        public RussianStressPattern NormalizeForAdjective()
        {
            var (main, alt) = this;

            if (alt == RussianStress.Zero)
            {
                alt = main;
                if (main > RussianStress.F)
                {
                    if (main <= RussianStress.Fp)
                        main -= 0b_0111;
                    else
                        main = main == RussianStress.Cpp ? RussianStress.C : RussianStress.F;
                }
            }

            return new((byte)((int)main | ((int)alt << 4)));
        }
        public RussianStressPattern NormalizeForVerb()
        {
            var (main, alt) = this;

            if (alt == RussianStress.Zero)
                alt = RussianStress.A;

            return new((byte)((int)main | ((int)alt << 4)));
        }

    }
}
