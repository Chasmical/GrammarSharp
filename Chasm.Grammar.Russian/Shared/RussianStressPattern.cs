using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianStressPattern : IEquatable<RussianStressPattern>
    {
        // Representation (_data field):
        //   xxxx_1111 - main stress type
        //   1111_xxxx - alternative stress type
        //
        private byte _data;

        public RussianStress Main
        {
            readonly get => (RussianStress)(_data & 0x0F);
            set
            {
                ValidateStress(value);
                _data = (byte)((_data & 0xF0) | (int)value);
            }
        }
        public RussianStress Alt
        {
            readonly get => (RussianStress)(_data >> 4);
            set
            {
                ValidateStress(value);
                _data = (byte)((_data & 0x0F) | ((int)value << 4));
            }
        }

        private RussianStressPattern(byte data)
            => _data = data;

        public RussianStressPattern(RussianStress main)
            : this(main, RussianStress.Zero) { }
        public RussianStressPattern(RussianStress main, RussianStress alt)
        {
            ValidateStress(main);
            ValidateStress(alt);

            _data = (byte)((int)main | ((int)alt << 4));
        }

        public RussianStressPattern(string stressPattern)
            => this = Parse(stressPattern);
        public RussianStressPattern(ReadOnlySpan<char> stressPattern)
            => this = Parse(stressPattern);

        private static void ValidateStress(RussianStress stress, [CallerArgumentExpression(nameof(stress))] string? paramName = null)
        {
            if ((uint)stress > (uint)RussianStress.Fpp)
                Throw(stress, paramName);

            static void Throw(RussianStress stress, string? paramName)
                => throw new InvalidEnumArgumentException(paramName, (int)stress, typeof(RussianStress));
        }

        public readonly void Deconstruct(out RussianStress main, out RussianStress alt)
        {
            main = Main;
            alt = Alt;
        }

        [Pure] public readonly bool Equals(RussianStressPattern other)
            => _data == other._data;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianStressPattern other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => _data;

        [Pure] public static bool operator ==(RussianStressPattern left, RussianStressPattern right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianStressPattern left, RussianStressPattern right)
            => !(left == right);

        internal void Normalize(RussianDeclensionType type, string paramName)
        {
            switch (type)
            {
                case RussianDeclensionType.Unknown:
                    break;
                case RussianDeclensionType.Noun:
                    NormalizeForNoun(paramName);
                    break;
                case RussianDeclensionType.Adjective:
                    NormalizeForAdjective(paramName);
                    break;
                case RussianDeclensionType.Pronoun:
                    NormalizeForPronoun(paramName);
                    break;
                default:
                    throw new InvalidEnumArgumentException(paramName, (int)type, typeof(RussianDeclensionType));
            }
        }
        private void NormalizeForNoun(string paramName)
        {
            var (main, alt) = this;

            // Nouns don't have alternative stress
            if (alt != RussianStress.Zero)
                throw new ArgumentException($"{this} is not a valid stress pattern for nouns.", paramName);

            // Noun stress can only be: 0, a through f, b′, d′, f′ and f″
            if ((uint)main > (uint)RussianStress.F && ((uint)main & 1) != 0)
                throw new ArgumentException($"{this} is not a valid stress pattern for nouns.", paramName);

            _data = (byte)((int)main | ((int)alt << 4));
        }
        private void NormalizeForAdjective(string paramName)
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

            _data = (byte)((int)main | ((int)alt << 4));
        }
        private void NormalizeForPronoun(string paramName)
        {
            var (main, alt) = this;

            // Pronouns don't have alternative stress
            if (alt != RussianStress.Zero)
                throw new ArgumentException($"{this} is not a valid stress pattern for pronouns.", paramName);
            // Pronoun stress can only be: 0, a, b or f
            if (main is > RussianStress.B and not RussianStress.F)
                throw new ArgumentException($"{this} is not a valid stress pattern for pronouns.", paramName);

            _data = (byte)((int)main | ((int)alt << 4));
        }

        internal void NormalizeForVerb(string paramName)
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

            _data = (byte)((int)main | ((int)alt << 4));
        }

    }
}
