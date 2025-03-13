using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
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
            set => _data = (byte)((_data & 0b_111_11_100) | (int)value);
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

        internal RussianNounProperties(byte data)
            => _data = data;

        public RussianNounProperties(char gender, bool isAnimate)
            : this(RussianGrammar.ParseGender(gender), isAnimate, 0) { }
        public RussianNounProperties(RussianGender gender, bool isAnimate)
            : this(gender, isAnimate, 0) { }
        public RussianNounProperties(RussianGender gender, bool isAnimate, RussianNounFlags flags)
        {
            if ((uint)gender > (uint)RussianGender.Common)
                throw new InvalidEnumArgumentException(nameof(gender), (int)gender, typeof(RussianGender));
            if (gender == RussianGender.Common && !isAnimate)
                throw new ArgumentException("isAnimate must be true, if the gender is Common.", nameof(isAnimate));

            _data = (byte)((int)gender | (isAnimate ? 0b_100 : 0) | ((int)flags << 3));
        }

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
            // Preserve animacy, convert Common to Feminine for correct endings, and add case as extra data
            _data = (byte)((_data & 0b_100) | Math.Min(_data & 0b_011, (int)RussianGender.Feminine) | pluralFlag | ((int)@case << 5));
        }
        internal void PrepareForAdjectiveDeclension(RussianCase @case, bool plural)
        {
            // If it doesn't have a tantum, apply specified count
            int pluralFlag = IsTantum ? _data & 0b_000_01_000 : plural ? 0b_000_01_000 : 0;
            // If plural, use Common gender (lookup optimization)
            int genderFlag = plural ? 0b_011 : Math.Min(_data & 0b_011, (int)RussianGender.Feminine);
            // Preserve animacy, and add case as extra data
            _data = (byte)((_data & 0b_100) | pluralFlag | genderFlag | ((int)@case << 5));
        }

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
