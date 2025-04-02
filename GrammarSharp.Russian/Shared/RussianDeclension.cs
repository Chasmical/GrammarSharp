using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian declension, according to Zaliznyak's classification, consisting of: declension type, stem type, stress pattern, declension flags, and, optionally, special noun declension properties.</para>
    /// </summary>
    public partial struct RussianDeclension : IEquatable<RussianDeclension>
    {
        // Representation (_types field):
        //   xxxx_1111 - stem type       (0000 - 0, 1000 - 8)
        //   1111_xxxx - declension type (see RussianDeclensionType enum)
        //
        // Representation (_stress field):
        //   1111_1111 - see RussianStressPattern struct
        //
        // Representation (_flags field):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        // Representation (_specialProps field):
        //   111_11111 - see RussianNounProperties struct
        //
        //   xxx_11111 - special noun gender, animacy and tantums
        //   001_xxxxx - 'is noun and has special declension properties' flag
        //   010_xxxxx - 'is adjective and is reflexive' flag
        //
        private byte _types;
        private RussianStressPattern _stress;
        private byte _flags;
        private RussianNounProperties _specialProps;

        /// <summary>
        ///   <para>Gets the declension's type.</para>
        /// </summary>
        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_types >> 4);

            // This setter is only used in internal Normalize() method, so there's no validation
            private set => _types = (byte)((_types & 0x0F) | ((int)value << 4));
        }
        /// <summary>
        ///   <para>Gets the word's stem type.</para>
        ///   <para>
        ///     <br/>0 — the word is indeclinable;
        ///     <br/>1 — stem ending with a hard consonant;
        ///     <br/>2 — stem ending with a soft consonant;
        ///     <br/>3 — stem ending with a 'к', 'г' or 'х';
        ///     <br/>4 — stem ending with a hissing sibilant ('ш', 'ж', 'ч' or 'щ');
        ///     <br/>5 — stem ending with a 'ц';
        ///     <br/>6 — stem ending with a vowel;
        ///     <br/>7 — stem ending with 'и';
        ///     <br/>8 — traditional "3rd declension" for nouns.
        ///   </para>
        /// </summary>
        public int StemType
        {
            readonly get => _types & 0x0F;

            // This setter is only used in RussianEndings.GetAdjectiveEndingIndices(), so there's no validation
            internal set => _types = (byte)((_types & 0xF0) | value);
        }

        /// <summary>
        ///   <para>Gets the word's stress pattern.</para>
        /// </summary>
        public readonly RussianStressPattern StressPattern => _stress;
        /// <summary>
        ///   <para>Gets the declension's flags.</para>
        /// </summary>
        public readonly RussianDeclensionFlags Flags => (RussianDeclensionFlags)_flags;

        /// <summary>
        ///   <para>Gets or sets the noun's special declension properties (available only for noun type).</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">This declension is not of noun type.</exception>
        public RussianNounProperties? SpecialNounProperties
        {
            readonly get => _specialProps.ExtraData == 1 ? _specialProps.WithoutExtraData() : null;
            set
            {
                if (!value.HasValue && _specialProps.ExtraData != 1) return;

                if (Type is not RussianDeclensionType.Unknown and not RussianDeclensionType.Noun)
                    throw new InvalidOperationException("Cannot set adjective/pronoun declension's noun properties.");

                _specialProps = value.GetValueOrDefault();
                _specialProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }
        /// <summary>
        ///   <para>Gets or sets whether the adjective declension is reflexive (available only for adjective type).</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">This declension is not of adjective type.</exception>
        public bool IsReflexiveAdjective
        {
            readonly get => _specialProps.ExtraData == 2;
            set
            {
                if (value == IsReflexiveAdjective) return;

                if (Type is not RussianDeclensionType.Unknown and not RussianDeclensionType.Adjective)
                    throw new InvalidOperationException("Cannot set noun/pronoun declension's adjective properties.");

                _specialProps = default;
                _specialProps.ExtraData = value ? 2 : 0;
            }
        }

        /// <summary>
        ///   <para>Determines whether this declension is 0.</para>
        /// </summary>
        public readonly bool IsZero => StemType == 0;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianDeclension"/> structure with the specified declension <paramref name="type"/>, <paramref name="stemType"/>, <paramref name="stress"/> pattern and declension <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="type">The declension's type.</param>
        /// <param name="stemType">The word's stem type.</param>
        /// <param name="stress">The word's stress pattern.</param>
        /// <param name="flags">The declension's flags.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="type"/> is not a valid declension type.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> or <paramref name="flags"/> is not zero, while <paramref name="stemType"/> is zero.</exception>
        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            if ((uint)type > (uint)RussianDeclensionType.Pronoun)
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(RussianDeclensionType));

            if (stemType == 0)
            {
                if (!stress.IsZero) throw new ArgumentException("Declension 0 cannot have a non-zero stress pattern.", nameof(stress));
                if (flags != 0) throw new ArgumentException("Declension 0 cannot have declension flags.", nameof(stress));
                _types = (byte)((int)type << 4);
            }
            else
            {
                ValidateStemType(stemType, type);
                _types = (byte)(stemType | ((int)type << 4));
                _stress = stress;
                _flags = (byte)flags;
            }
        }

        private static void ValidateStemType(
            int stemType, RussianDeclensionType type,
            [CallerArgumentExpression(nameof(stemType))] string? paramName = null
        )
        {
            // Stem type 8 is exclusive to nouns
            if ((uint)stemType > (type == RussianDeclensionType.Noun ? 8 : 7))
                Throw(stemType, type, paramName);

            static void Throw(int stemType, RussianDeclensionType type, string? paramName)
            {
                string msg = $"{stemType} is not a valid stem type for {type}.";
                throw new ArgumentOutOfRangeException(paramName, stemType, msg);
            }
        }

        /// <summary>
        ///   <para>Extracts the stem of the specified <paramref name="word"/>, according to this declension's <see cref="Type"/>.</para>
        /// </summary>
        /// <param name="word">The word to extract the stem of.</param>
        /// <param name="isAdjReflexive">When this method returns, contains a value indicating whether the declension is of adjective type, and the word is a reflexive adjective (ending with 'ся').</param>
        /// <returns>The stem of the specified <paramref name="word"/>, according to this declension's <see cref="Type"/>.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Type"/> is <see cref="RussianDeclensionType.Unknown"/>.</exception>
        [Pure] public readonly string ExtractStem(string word, out bool isAdjReflexive)
        {
            if (IsZero)
            {
                isAdjReflexive = false;
                return word;
            }
            return ExtractStem(word.AsSpan(), out isAdjReflexive).ToString();
        }
        /// <summary>
        ///   <para>Extracts the stem of the specified <paramref name="word"/>, according to this declension's <see cref="Type"/>.</para>
        /// </summary>
        /// <param name="word">The word to extract the stem of.</param>
        /// <param name="isAdjReflexive">When this method returns, contains a value indicating whether the declension is of adjective type, and the word is a reflexive adjective (ending with 'ся').</param>
        /// <returns>The stem of the specified <paramref name="word"/>, according to this declension's <see cref="Type"/>.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Type"/> is <see cref="RussianDeclensionType.Unknown"/>.</exception>
        [Pure] public readonly ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word, out bool isAdjReflexive)
        {
            isAdjReflexive = false;
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Noun or RussianDeclensionType.Pronoun:
                    return RussianNoun.ExtractStem(word);

                case RussianDeclensionType.Adjective:
                    return RussianAdjective.ExtractStem(word, out isAdjReflexive);

                default:
                    throw new InvalidOperationException("Cannot extract stem using an unknown declension.");
            }
        }

        internal void RemovePluraleTantum()
            => _specialProps.IsPluraleTantum = false;

        internal void Normalize(RussianDeclensionType defaultType, string paramName)
        {
            if (IsZero)
            {
                Type = defaultType;
                return;
            }

            RussianDeclensionType type = Type;
            if (type == RussianDeclensionType.Unknown)
            {
                Type = type = defaultType;
                ValidateStemType(StemType, type, paramName);
            }
            _stress.NormalizeMut(type, paramName);
        }

        /// <summary>
        ///   <para>Determines whether this Russian declension is equal to another specified Russian declension.</para>
        /// </summary>
        /// <param name="other">The Russian declension to compare with this Russian declension.</param>
        /// <returns><see langword="true"/>, if this Russian declension is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianDeclension other)
        {
            // Nothing special is needed here, so just perform bitwise equality
            return Unsafe.As<RussianDeclension, int>(ref Unsafe.AsRef(in this)) == Unsafe.As<RussianDeclension, int>(ref other);
        }
        /// <summary>
        ///   <para>Determines whether this Russian declension is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian declension.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianDeclension"/> instance equal to this Russian declension; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianDeclension other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian declension.</para>
        /// </summary>
        /// <returns>A hash code for this Russian declension.</returns>
        [Pure] public readonly override int GetHashCode()
        {
            // Note: this hash code may not be consistent across machines
            return Unsafe.As<RussianDeclension, int>(ref Unsafe.AsRef(in this));
        }

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
