using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianStressPattern : IEquatable<RussianStressPattern>
    {
        // Representation (_data field):
        //   xxxx_1111 - main stress type
        //   1111_xxxx - alternative stress type
        //
        private readonly byte _data;

        public RussianStress Main => (RussianStress)(_data & 0x0F);
        public RussianStress Alt => (RussianStress)(_data >> 4);

        private RussianStressPattern(byte data)
            => _data = data;

        public RussianStressPattern(RussianStress main)
            : this(main, RussianStress.Zero) { }
        public RussianStressPattern(RussianStress main, RussianStress alt)
        {
            if ((uint)main > (uint)RussianStress.Fpp)
                throw new InvalidEnumArgumentException(nameof(main), (int)main, typeof(RussianStress));
            if ((uint)alt > (uint)RussianStress.Fpp)
                throw new InvalidEnumArgumentException(nameof(alt), (int)alt, typeof(RussianStress));

            _data = (byte)((int)main | ((int)alt << 4));
        }

        public RussianStressPattern(string stressPattern)
            => this = Parse(stressPattern);
        public RussianStressPattern(ReadOnlySpan<char> stressPattern)
            => this = Parse(stressPattern);

        public void Deconstruct(out RussianStress main, out RussianStress alt)
        {
            main = Main;
            alt = Alt;
        }

        [Pure] public bool Equals(RussianStressPattern other)
            => _data == other._data;
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianStressPattern other && Equals(other);
        [Pure] public override int GetHashCode()
            => _data;

        [Pure] public static bool operator ==(RussianStressPattern left, RussianStressPattern right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianStressPattern left, RussianStressPattern right)
            => !(left == right);

        [Pure] internal RussianStressPattern NormalizeForNoun(string paramName)
        {
            var (main, alt) = this;

            // Nouns don't have alternative stress
            if (alt != RussianStress.Zero)
                throw new ArgumentException($"{this} is not a valid stress pattern for nouns.", paramName);

            // Noun stress can only be: 0, a through f, b′, d′, f′ and f″
            if ((uint)main > (uint)RussianStress.F && ((uint)main & 1) != 0)
                throw new ArgumentException($"{this} is not a valid stress pattern for nouns.", paramName);

            return new((byte)main);
        }
        [Pure] internal RussianStressPattern NormalizeForAdjective(string paramName)
        {
            var (main, alt) = this;

            // If alternative stress is not specified, default to main
            if (alt == RussianStress.Zero)
            {
                alt = main;

                // If main stress has a prime, remove the stress from it (but keep it in alternative stress)
                if (main >= RussianStress.Ap)
                {
                    if (main <= RussianStress.Cp)
                        main -= (int)RussianStress.F;
                    else if (main == RussianStress.Cpp)
                        main = RussianStress.C;
                    else
                        throw new ArgumentException($"{this} is not a valid stress pattern for adjectives.", paramName);
                }
            }

            // Full-form stress can only be: 0, a, b or c
            if (main > RussianStress.C)
                throw new ArgumentException($"{this} is not a valid stress pattern for adjectives.", paramName);
            // Short-form stress can only be: 0, a, b, c, a′, b′, c′ and c″
            if (alt is > RussianStress.C and not (>= RussianStress.Ap and <= RussianStress.Cp) and not RussianStress.Cpp)
                throw new ArgumentException($"{this} is not a valid stress pattern for adjectives.", paramName);

            return new((byte)((int)main | ((int)alt << 4)));
        }
        [Pure] internal RussianStressPattern NormalizeForPronoun(string paramName)
        {
            var (main, alt) = this;

            // Pronouns don't have alternative stress
            if (alt != RussianStress.Zero)
                throw new ArgumentException($"{this} is not a valid stress pattern for pronouns.", paramName);
            // Pronoun stress can only be: 0, a, b or f
            if (main is > RussianStress.B and not RussianStress.F)
                throw new ArgumentException($"{this} is not a valid stress pattern for pronouns.", paramName);

            return new((byte)((int)main | ((int)alt << 4)));
        }
        [Pure] internal RussianStressPattern NormalizeForVerb(string paramName)
        {
            var (main, alt) = this;

            // If alternative stress is not specified, default to a
            if (alt == RussianStress.Zero)
                alt = RussianStress.A;

            // Present-tense stress can only be: a, b, c or c′
            if (main is RussianStress.Zero or > RussianStress.C and not RussianStress.Cp)
                throw new ArgumentException($"{this} is not a valid stress pattern for verbs.", paramName);
            // Past-tense stress can only be: a, b, c, c′ or c″
            if (alt is > RussianStress.C and not RussianStress.Cp and not RussianStress.Cpp)
                throw new ArgumentException($"{this} is not a valid stress pattern for verbs.", paramName);

            return new((byte)((int)main | ((int)alt << 4)));
        }

    }
}
