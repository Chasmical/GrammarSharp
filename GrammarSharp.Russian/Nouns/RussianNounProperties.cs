using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
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
        public bool IsAnimate
        {
            readonly get => (_data & 0b_000_00_100) != 0;
            set => _data = (byte)((_data & 0b_111_11_011) | (value ? 0b_100 : 0));
        }

        public readonly bool IsTantum => (_data & 0b_000_10_000) != 0;
        public bool IsSingulareTantum
        {
            readonly get => (_data & 0b_000_11_000) == 0b_000_10_000;
            set
            {
                if (IsSingulareTantum == value) return;
                _data = (byte)((_data & 0b_111_00_111) | (value ? 0b_10_000 : 0));
            }
        }
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
        public RussianNounProperties(RussianGender gender, bool isAnimate)
            : this(gender, isAnimate, 0) { }
        public RussianNounProperties(RussianGender gender, bool isAnimate, RussianNounFlags flags)
        {
            if ((uint)gender > (uint)RussianGender.Common)
                ThrowInvalidGender((int)gender);

            _data = (byte)((int)gender | (isAnimate ? 0b_100 : 0) | ((int)flags << 3));
        }

        private static void ThrowInvalidGender(int gender)
            => throw new InvalidEnumArgumentException(nameof(gender), gender, typeof(RussianGender));

        // Where ExtraData is used and what for:
        // - in RussianDeclension, as "has special noun props" flag (value 1).
        // - in RussianDeclension, as "is reflexive adjective" flag (value 2).
        // - when declining nouns and adjectives, to store case.
        internal int ExtraData
        {
            readonly get => _data >> 5;
            set => _data = (byte)((_data & 0b_000_11_111) | (value << 5));
        }
        internal readonly bool IsPlural => (_data & 0b_000_01_000) != 0;
        internal readonly RussianCase Case => (RussianCase)ExtraData;

        internal readonly RussianNounProperties WithoutExtraData()
            => new RussianNounProperties((byte)(_data & 0b_000_11_111));

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
        internal void CopyTantumsFrom(RussianNounProperties other)
            => _data = (byte)((_data & 0b_111_00_111) | (other._data & 0b_000_11_000));

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

        [Pure] public readonly bool Equals(RussianNounProperties other)
            => (_data & 0b_000_11_111) == (other._data & 0b_000_11_111);
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounProperties other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => _data & 0b_000_11_111;

        [Pure] public static bool operator ==(RussianNounProperties left, RussianNounProperties right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianNounProperties left, RussianNounProperties right)
            => !(left == right);

    }
}
