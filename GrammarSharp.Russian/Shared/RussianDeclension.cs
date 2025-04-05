using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a discriminated union of Russian declension structures: <see cref="RussianNounDeclension"/>, <see cref="RussianAdjectiveDeclension"/> and <see cref="RussianPronounDeclension"/>.</para>
    /// </summary>
    public partial struct RussianDeclension : IEquatable<RussianDeclension>
    {
        // Representation (_field1):
        //   11_xxxxxx - declension type (see all Russian[…]Declension types)
        //   xx_111111 - < type-specific data >
        // Representation (_field2):
        //   1111_1111 - < type-specific data >
        // Representation (_field3):
        //   1111_1111 - < type-specific data >
        //
        private byte _field1;
        [UsedImplicitly] private readonly byte _field2;
        [UsedImplicitly] private readonly byte _field3;

        /// <summary>
        ///   <para>Gets the declension's type.</para>
        /// </summary>
        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_field1 >> 6);
            private set => _field1 = (byte)((_field1 & 0x3F) | ((int)value << 6));
        }

        /// <summary>
        ///   <para>Gets or sets the declension's stem type.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not a valid stem type for this type of declension.</exception>
        public int StemType
        {
            readonly get => Type switch
            {
                RussianDeclensionType.Noun => this.AsNounUnsafeRef().StemType,
                RussianDeclensionType.Adjective => this.AsAdjectiveUnsafeRef().StemType,
                RussianDeclensionType.Pronoun => this.AsPronounUnsafeRef().StemType,
                _ => throw new InvalidOperationException(),
            };
            set
            {
                switch (Type)
                {
                    case RussianDeclensionType.Noun:
                        this.AsNounUnsafeRefMutable().StemType = value;
                        break;
                    case RussianDeclensionType.Adjective:
                        this.AsAdjectiveUnsafeRefMutable().StemType = value;
                        break;
                    case RussianDeclensionType.Pronoun:
                        this.AsPronounUnsafeRefMutable().StemType = value;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        /// <summary>
        ///   <para>Gets or sets the declension's stress pattern.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid stress pattern for this type of declension.</exception>
        public RussianStressPattern StressPattern
        {
            readonly get => Type switch
            {
                RussianDeclensionType.Noun => new((byte)this.AsNounUnsafeRef().Stress),
                RussianDeclensionType.Adjective => this.AsAdjectiveUnsafeRef().StressPattern,
                RussianDeclensionType.Pronoun => new((byte)this.AsPronounUnsafeRef().Stress),
                _ => throw new InvalidOperationException(),
            };
            set
            {
                switch (Type)
                {
                    case RussianDeclensionType.Noun:
                        this.AsNounUnsafeRefMutable().Stress = value.Main;
                        break;
                    case RussianDeclensionType.Adjective:
                        this.AsAdjectiveUnsafeRefMutable().StressPattern = value;
                        break;
                    case RussianDeclensionType.Pronoun:
                        this.AsPronounUnsafeRefMutable().Stress = value.Main;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        /// <summary>
        ///   <para>Gets or sets the declension's flags.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid flag for this type of declension.</exception>
        public RussianDeclensionFlags Flags
        {
            readonly get => Type switch
            {
                RussianDeclensionType.Noun => this.AsNounUnsafeRef().Flags,
                RussianDeclensionType.Adjective => this.AsAdjectiveUnsafeRef().Flags,
                RussianDeclensionType.Pronoun => this.AsPronounUnsafeRef().Flags,
                _ => throw new InvalidOperationException(),
            };
            set
            {
                switch (Type)
                {
                    case RussianDeclensionType.Noun:
                        this.AsNounUnsafeRefMutable().Flags = value;
                        break;
                    case RussianDeclensionType.Adjective:
                        this.AsAdjectiveUnsafeRefMutable().Flags = value;
                        break;
                    case RussianDeclensionType.Pronoun:
                        this.AsPronounUnsafeRefMutable().Flags = value;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        ///   <para>Determines whether this declension is zero.</para>
        /// </summary>
        public readonly bool IsZero => StemType == 0;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianDeclension"/> structure from the specified <paramref name="declension"/> string.</para>
        /// </summary>
        /// <param name="declension">The string containing a Russian declension to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="declension"/> is not a valid Russian declension.</exception>
        public RussianDeclension(string declension)
            => this = Parse(declension);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianDeclension"/> structure from the specified <paramref name="declension"/> span.</para>
        /// </summary>
        /// <param name="declension">The read-only span of characters containing a Russian declension to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="declension"/> is not a valid Russian declension.</exception>
        public RussianDeclension(ReadOnlySpan<char> declension)
            => this = Parse(declension);

        /// <summary>
        ///   <para>Defines an implicit conversion of a <seealso cref="RussianNounDeclension"/> to a <seealso cref="RussianDeclension"/>.</para>
        /// </summary>
        /// <param name="declension">The Russian noun declension to convert.</param>
        [Pure] public static implicit operator RussianDeclension(RussianNounDeclension declension)
            => Unsafe.As<RussianNounDeclension, RussianDeclension>(ref declension);
        /// <summary>
        ///   <para>Defines an implicit conversion of a <seealso cref="RussianAdjectiveDeclension"/> to a <seealso cref="RussianDeclension"/>.</para>
        /// </summary>
        /// <param name="declension">The Russian adjective declension to convert.</param>
        [Pure] public static implicit operator RussianDeclension(RussianAdjectiveDeclension declension)
        {
            var decl = Unsafe.As<RussianAdjectiveDeclension, RussianDeclension>(ref declension);
            // Ensure that the type marker is not lost (argument could have a default value)
            decl._field1 |= (int)RussianDeclensionType.Adjective << 6;
            return decl;
        }
        /// <summary>
        ///   <para>Defines an implicit conversion of a <seealso cref="RussianPronounDeclension"/> to a <seealso cref="RussianDeclension"/>.</para>
        /// </summary>
        /// <param name="declension">The Russian pronoun declension to convert.</param>
        [Pure] public static implicit operator RussianDeclension(RussianPronounDeclension declension)
        {
            var decl = Unsafe.As<RussianPronounDeclension, RussianDeclension>(ref declension);
            // Ensure that the type marker is not lost (argument could have a default value)
            decl._field1 |= (int)RussianDeclensionType.Pronoun << 6;
            return decl;
        }

        /// <summary>
        ///   <para>Tries to reinterpret this Russian declension as a declension of noun type.</para>
        /// </summary>
        /// <param name="nounDeclension">When this method returns, contains the reinterpreted declension of noun type.</param>
        /// <returns><see langword="true"/>, if this Russian declension is of noun type; otherwise, <see langword="false"/>.</returns>
        public readonly bool TryAsNoun(out RussianNounDeclension nounDeclension)
        {
            nounDeclension = this.AsNounUnsafeRef();
            return Type == RussianDeclensionType.Noun;
        }
        /// <summary>
        ///   <para>Tries to reinterpret this Russian declension as a declension of adjective type.</para>
        /// </summary>
        /// <param name="adjectiveDeclension">When this method returns, contains the reinterpreted declension of adjective type.</param>
        /// <returns><see langword="true"/>, if this Russian declension is of adjective type; otherwise, <see langword="false"/>.</returns>
        public readonly bool TryAsAdjective(out RussianAdjectiveDeclension adjectiveDeclension)
        {
            adjectiveDeclension = this.AsAdjectiveUnsafeRef();
            return Type == RussianDeclensionType.Adjective;
        }
        /// <summary>
        ///   <para>Tries to reinterpret this Russian declension as a declension of pronoun type.</para>
        /// </summary>
        /// <param name="pronounDeclension">When this method returns, contains the reinterpreted declension of pronoun type.</param>
        /// <returns><see langword="true"/>, if this Russian declension is of pronoun type; otherwise, <see langword="false"/>.</returns>
        public readonly bool TryAsPronoun(out RussianPronounDeclension pronounDeclension)
        {
            pronounDeclension = this.AsPronounUnsafeRef();
            return Type == RussianDeclensionType.Pronoun;
        }

        /// <summary>
        ///   <para>Extracts the stem of the specified <paramref name="word"/>, according to this declension's type.</para>
        /// </summary>
        /// <param name="word">The word to extract the stem of.</param>
        /// <returns>The stem of the specified <paramref name="word"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="word"/> is <see langword="null"/>.</exception>
        public string ExtractStem(string word)
        {
            Guard.ThrowIfNull(word);
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Adjective:
                    string stem = RussianAdjective.ExtractStem(word, out bool isAdjReflexive);
                    if (isAdjReflexive) this.AsAdjectiveUnsafeRefMutable().IsReflexive = true;
                    return stem;

                default: // case RussianDeclensionType.Noun or RussianDeclensionType.Pronoun:
                    return RussianNoun.ExtractStem(word);
            }
        }

        /// <summary>
        ///   <para>Determines whether this Russian declension is equal to another specified Russian declension.</para>
        /// </summary>
        /// <param name="other">The Russian declension to compare with this Russian declension.</param>
        /// <returns><see langword="true"/>, if this Russian declension is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        public readonly bool Equals(RussianDeclension other)
            => _field1 == other._field1 && _field2 == other._field2 && _field3 == other._field3;
        /// <summary>
        ///   <para>Determines whether this Russian declension is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian declension.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianDeclension"/> instance equal to this Russian declension; otherwise, <see langword="false"/>.</returns>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianDeclension other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian declension.</para>
        /// </summary>
        /// <returns>A hash code for this Russian declension.</returns>
        public readonly override int GetHashCode()
            => HashCode.Combine(_field1, _field2, _field3);

        /// <summary>
        ///   <para>Determines whether the two specified Russian declensions are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian declension to compare.</param>
        /// <param name="right">The second Russian declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianDeclension left, RussianDeclension right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian declensions are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian declension to compare.</param>
        /// <param name="right">The second Russian declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianDeclension left, RussianDeclension right)
            => !(left == right);

    }
}
