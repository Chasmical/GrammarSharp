using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian noun's properties: grammatical gender, animacy and tantums.</para>
    /// </summary>
    public partial struct RussianNounProperties : IEquatable<RussianNounProperties>
    {
        // Representation (_data field):
        //   xxx_xx_x11 - gender (see RussianGender enum)
        //   xxx_xx_1xx - animacy (0 - inanimate, 1 - animate)
        //   xxx_x1_xxx - is plural (0 - singular, 1 - plural)
        //   xxx_1x_xxx - tantum indicator (0 - nothing, 1 - current "is plural" value is the tantum)
        //   111_xx_xxx - reserved for storing extra data (such as case, or various internal flags)
        //
        private byte _data;

        /// <summary>
        ///   <para>Gets or sets the noun's grammatical gender.</para>
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="value"/> is not a valid <seealso cref="RussianGender"/> value.</exception>
        public RussianGender Gender
        {
            readonly get => (RussianGender)(_data & 0b_000_00_011);
            set
            {
                ValidateGender(value);
                _data = (byte)((_data & 0b_111_11_100) | (int)value);
            }
        }
        /// <summary>
        ///   <para>Gets or sets the noun's grammatical animacy.</para>
        /// </summary>
        public bool IsAnimate
        {
            readonly get => (_data & 0b_000_00_100) != 0;
            set => _data = (byte)((_data & 0b_111_11_011) | (value ? 0b_100 : 0));
        }

        /// <summary>
        ///   <para>Determines whether the noun is a tantum (only appears in either singular or plural form).</para>
        /// </summary>
        public readonly bool IsTantum => (_data & 0b_000_10_000) != 0;
        /// <summary>
        ///   <para>Gets or sets whether the noun is a singulare tantum (only appears in singular form).</para>
        /// </summary>
        public bool IsSingulareTantum
        {
            readonly get => (_data & 0b_000_11_000) == 0b_000_10_000;
            set
            {
                if (IsSingulareTantum == value) return;
                _data = (byte)((_data & 0b_111_00_111) | (value ? 0b_10_000 : 0));
            }
        }
        /// <summary>
        ///   <para>Gets or sets whether the noun is a plurale tantum (only appears in plural form).</para>
        /// </summary>
        public bool IsPluraleTantum
        {
            readonly get => (_data & 0b_000_11_000) == 0b_000_11_000;
            set
            {
                if (IsPluraleTantum == value) return;
                _data = (byte)((_data & 0b_111_00_111) | (value ? 0b_11_000 : 0));
            }
        }

        private RussianNounProperties(byte data)
            => _data = data;

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounProperties"/> structure with the specified <paramref name="gender"/> and animacy.</para>
        /// </summary>
        /// <param name="gender">The noun's grammatical gender.</param>
        /// <param name="isAnimate">The noun's grammatical animacy: <see langword="true"/> - animate, <see langword="false"/> - inanimate.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="gender"/> is not a valid <seealso cref="RussianGender"/> value.</exception>
        public RussianNounProperties(RussianGender gender, bool isAnimate)
        {
            ValidateGender(gender);
            _data = (byte)((int)gender | (isAnimate ? 0b_100 : 0));
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounProperties"/> structure with the specified <paramref name="gender"/>, animacy and <paramref name="tantums"/>.</para>
        /// </summary>
        /// <param name="gender">The noun's grammatical gender.</param>
        /// <param name="isAnimate">The noun's grammatical animacy: <see langword="true"/> - animate, <see langword="false"/> - inanimate.</param>
        /// <param name="tantums">The noun's flags.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="gender"/> is not a valid <seealso cref="RussianGender"/> value.</exception>
        public RussianNounProperties(RussianGender gender, bool isAnimate, RussianTantums tantums)
        {
            ValidateGender(gender);
            ValidateTantums(tantums);
            _data = (byte)((int)gender | (isAnimate ? 0b_100 : 0) | ((int)tantums << 3));
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianNounProperties"/> structure from the specified <paramref name="nounProperties"/> string.</para>
        /// </summary>
        /// <param name="nounProperties">The string containing Russian noun properties to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="nounProperties"/> are not valid Russian noun properties.</exception>
        public RussianNounProperties(string nounProperties)
            => this = Parse(nounProperties);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianNounProperties"/> structure from the specified <paramref name="nounProperties"/> span.</para>
        /// </summary>
        /// <param name="nounProperties">The read-only span of characters containing Russian noun properties to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="nounProperties"/> are not valid Russian noun properties.</exception>
        public RussianNounProperties(ReadOnlySpan<char> nounProperties)
            => this = Parse(nounProperties);

        internal RussianNounProperties(RussianGender gender, RussianCase @case)
            => _data = (byte)(((int)@case << 5) | (int)gender);

        private static void ValidateGender(RussianGender gender, [CAE(nameof(gender))] string? paramName = null)
        {
            if ((uint)gender > (uint)RussianGender.Common)
                Throw(gender, paramName);

            static void Throw(RussianGender gender, string? paramName)
                => throw new InvalidEnumArgumentException(paramName, (int)gender, typeof(RussianGender));
        }
        private static void ValidateTantums(RussianTantums tantums, [CAE(nameof(tantums))] string? paramName = null)
        {
            if ((uint)tantums is 1 or > (uint)RussianTantums.IsPluraleTantum)
                Throw(tantums, paramName);

            static void Throw(RussianTantums tantums, string? paramName)
                => throw new InvalidEnumArgumentException(paramName, (int)tantums, typeof(RussianTantums));
        }

        // Where ExtraData is used and what for:
        // - in RussianNounDeclension, to store 3 extra bits of data.
        // - during declension of all declensible words, to store case.
        internal int ExtraData
        {
            readonly get => _data >> 5;
            set => _data = (byte)((_data & 0b_000_11_111) | (value << 5));
        }
        internal readonly bool IsPlural => (_data & 0b_000_01_000) != 0;
        internal readonly RussianCase Case => (RussianCase)ExtraData;

        internal void PrepareForDeclensionCase(RussianCase @case, bool plural)
        {
            // If it doesn't have a tantum, apply specified count
            int pluralFlag = IsTantum ? _data & 0b_000_01_000 : plural ? 0b_000_01_000 : 0;
            // Convert Common to Feminine gender for endings
            int genderFlags = Math.Min(_data & 0b_011, (int)RussianGender.Feminine);
            // Preserve animacy, and add case as extra data
            _data = (byte)((_data & 0b_100) | pluralFlag | genderFlags | ((int)@case << 5));
        }
        internal void PrepareForDeclensionGenderCount(RussianCase @case, bool plural)
        {
            // If it doesn't have a tantum, apply specified count
            int pluralFlag = IsTantum ? _data & 0b_000_01_000 : plural ? 0b_000_01_000 : 0;
            // If plural, use Common gender (lookup optimization)
            int genderFlags = pluralFlag != 0 ? 0b_011 : Math.Min(_data & 0b_011, (int)RussianGender.Feminine);
            // Preserve animacy, and add case as extra data
            _data = (byte)((_data & 0b_100) | pluralFlag | genderFlags | ((int)@case << 5));
        }

        internal readonly RussianNounProperties WithoutExtraData()
            => new RussianNounProperties((byte)(_data & 0b_000_11_111));
        internal void StripTantumsAndSetBoolExtraData(bool extraData)
            => _data = (byte)((_data & 0b_000_00_111) | (extraData ? 0b_001_00_000 : 0));
        internal void CopyFromButKeepTantums(RussianNounProperties other)
            => _data = (byte)((_data & 0b_000_11_000) | (other._data & 0b_111_00_111));

        internal readonly bool IsNominativeNormalized
        {
            get
            {
                RussianCase @case = Case;
                return @case == RussianCase.Nominative || @case == RussianCase.Accusative && !IsAnimate;
            }
        }
        internal readonly bool IsGenitiveNormalized
        {
            get
            {
                RussianCase @case = Case;
                return @case == RussianCase.Genitive || @case == RussianCase.Accusative && IsAnimate;
            }
        }

        /// <summary>
        ///   <para>Determines whether this Russian noun property set is equal to another specified Russian noun property set.</para>
        /// </summary>
        /// <param name="other">The Russian noun property set to compare with this Russian noun property set.</param>
        /// <returns><see langword="true"/>, if this Russian noun property set is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianNounProperties other)
            => _data == other._data;
        /// <summary>
        ///   <para>Determines whether this Russian noun property set is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian noun property set.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianNounInfo"/> instance equal to this Russian noun property set; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounProperties other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian noun property set.</para>
        /// </summary>
        /// <returns>A hash code for this Russian noun property set.</returns>
        [Pure] public readonly override int GetHashCode()
            => _data;

        /// <summary>
        ///   <para>Determines whether the two specified Russian noun property sets are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun property set to compare.</param>
        /// <param name="right">The second Russian noun property set to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianNounProperties left, RussianNounProperties right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian noun property sets are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun property set to compare.</param>
        /// <param name="right">The second Russian noun property set to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianNounProperties left, RussianNounProperties right)
            => !(left == right);

    }
}
