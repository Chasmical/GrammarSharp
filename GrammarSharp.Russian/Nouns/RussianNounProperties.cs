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
                if ((uint)value > (uint)RussianGender.Common)
                    ThrowInvalidGender((int)value);
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

        public RussianNounProperties(char gender, bool isAnimate)
            : this(RussianGrammar.ParseGender(gender), isAnimate, 0) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounProperties"/> structure with the specified <paramref name="gender"/> and animacy.</para>
        /// </summary>
        /// <param name="gender">The noun's grammatical gender.</param>
        /// <param name="isAnimate">The noun's grammatical animacy: <see langword="true"/> - animate, <see langword="false"/> - inanimate.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="gender"/> is not a valid <seealso cref="RussianGender"/> value.</exception>
        public RussianNounProperties(RussianGender gender, bool isAnimate)
            : this(gender, isAnimate, 0) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounProperties"/> structure with the specified <paramref name="gender"/>, animacy and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="gender">The noun's grammatical gender.</param>
        /// <param name="isAnimate">The noun's grammatical animacy: <see langword="true"/> - animate, <see langword="false"/> - inanimate.</param>
        /// <param name="flags">The noun's flags.</param>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="gender"/> is not a valid <seealso cref="RussianGender"/> value.</exception>
        public RussianNounProperties(RussianGender gender, bool isAnimate, RussianNounFlags flags)
        {
            if ((uint)gender > (uint)RussianGender.Common)
                ThrowInvalidGender((int)gender);

            _data = (byte)((int)gender | (isAnimate ? 0b_100 : 0) | ((int)flags << 3));
        }

        private static void ThrowInvalidGender(int gender)
            => throw new InvalidEnumArgumentException(nameof(gender), gender, typeof(RussianGender));

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

        internal void PrepareForNounDeclension(RussianCase @case, bool plural)
        {
            // If it doesn't have a tantum, apply specified count
            int pluralFlag = IsTantum ? _data & 0b_000_01_000 : plural ? 0b_000_01_000 : 0;
            // Convert Common to Feminine gender for endings
            int genderFlags = Math.Min(_data & 0b_011, (int)RussianGender.Feminine);
            // Preserve animacy, and add case as extra data
            _data = (byte)((_data & 0b_100) | pluralFlag | genderFlags | ((int)@case << 5));
        }
        internal void PrepareForAdjectiveDeclension(RussianCase @case, bool plural)
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
