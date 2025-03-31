using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a stress pattern in Russian words, according to Zaliznyak's classification, consisting of main and alternative <see cref="RussianStress"/> schemas.</para>
    /// </summary>
    public partial struct RussianStressPattern : IEquatable<RussianStressPattern>
    {
        // Representation (_data field):
        //   xxxx_1111 - main stress type
        //   1111_xxxx - alternative stress type
        //
        private byte _data;

        /// <summary>
        ///   <para>Gets or sets the stress schema in the word's main form.</para>
        /// </summary>
        public RussianStress Main
        {
            readonly get => (RussianStress)(_data & 0x0F);
            set
            {
                ValidateStress(value);
                _data = (byte)((_data & 0xF0) | (int)value);
            }
        }
        /// <summary>
        ///   <para>Gets or sets the stress schema in the word's alternative form.</para>
        /// </summary>
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

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianStressPattern"/> structure with the specified <paramref name="main"/> and <paramref name="alt"/> form stress schemas.</para>
        /// </summary>
        /// <param name="main">The stress schema in the word's main form.</param>
        /// <param name="alt">The stress schema in the word's alternative form.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="main"/> or <paramref name="alt"/> is not a valid stress schema.</exception>
        public RussianStressPattern(RussianStress main, RussianStress alt)
        {
            ValidateStress(main);
            ValidateStress(alt);

            _data = (byte)((int)main | ((int)alt << 4));
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianStressPattern"/> structure from the specified <paramref name="stressPattern"/> string.</para>
        /// </summary>
        /// <param name="stressPattern">The string containing a Russian stress pattern to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="stressPattern"/> is not a valid Russian stress pattern.</exception>
        public RussianStressPattern(string stressPattern)
            => this = Parse(stressPattern);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianStressPattern"/> structure from the specified <paramref name="stressPattern"/> span.</para>
        /// </summary>
        /// <param name="stressPattern">The read-only span of characters containing a Russian stress pattern to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="stressPattern"/> is not a valid Russian stress pattern.</exception>
        public RussianStressPattern(ReadOnlySpan<char> stressPattern)
            => this = Parse(stressPattern);

        private static void ValidateStress(RussianStress stress, [CallerArgumentExpression(nameof(stress))] string? paramName = null)
        {
            if ((uint)stress > (uint)RussianStress.Fpp)
                Throw(stress, paramName);

            static void Throw(RussianStress stress, string? paramName)
                => throw new InvalidEnumArgumentException(paramName, (int)stress, typeof(RussianStress));
        }

        /// <summary>
        ///   <para>Deconstructs this Russian stress pattern by <see cref="Main"/> and <see cref="Alt"/>.</para>
        /// </summary>
        /// <param name="main">When this method returns, represents the <see cref="Main"/> value of this Russian stress pattern.</param>
        /// <param name="alt">When this method returns, represents the <see cref="Alt"/> value of this Russian stress pattern.</param>
        public readonly void Deconstruct(out RussianStress main, out RussianStress alt)
        {
            main = Main;
            alt = Alt;
        }

        /// <summary>
        ///   <para>Determines whether this Russian stress pattern is equal to another specified Russian stress pattern.</para>
        /// </summary>
        /// <param name="other">The Russian stress pattern to compare with this Russian stress pattern.</param>
        /// <returns><see langword="true"/>, if this Russian stress pattern is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianStressPattern other)
            => _data == other._data;
        /// <summary>
        ///   <para>Determines whether this Russian stress pattern is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian stress pattern.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianStressPattern"/> instance equal to this Russian stress pattern; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianStressPattern other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian stress pattern.</para>
        /// </summary>
        /// <returns>A hash code for this Russian stress pattern.</returns>
        [Pure] public readonly override int GetHashCode()
            => _data;

        /// <summary>
        ///   <para>Determines whether the two specified Russian stress patterns are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian stress pattern to compare.</param>
        /// <param name="right">The second Russian stress pattern to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianStressPattern left, RussianStressPattern right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian stress patterns are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian stress pattern to compare.</param>
        /// <param name="right">The second Russian stress pattern to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianStressPattern left, RussianStressPattern right)
            => !(left == right);

        [Pure] internal readonly bool IsDoubleA()
            => _data == ((int)RussianStress.A | ((int)RussianStress.A << 4));

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
