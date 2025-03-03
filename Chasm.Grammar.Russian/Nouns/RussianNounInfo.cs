namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianNounInfo
    {
        // Representation (_data field):
        //   xxx_xx_x11 - gender (see RussianGender enum)
        //   xxx_xx_1xx - animacy (0 - inanimate, 1 - animate)
        //   xxx_x1_xxx - is plural (0 - singular, 1 - plural)
        //   xxx_1x_xxx - tantum indicator (0 - nothing, 1 - current "is plural" value is the tantum)
        //   111_xx_xxx - reserved
        //
        // Notes:
        //   Values enclosed in [[]] are only used internally during noun declension,
        //   and aren't supposed to represent any specific noun's characteristics.
        //
        private readonly byte _data;
        private readonly RussianNounInfoForDeclension _declensionInfo;

        private RussianNounInfo(byte data)
        {
            _data = data;
            _declensionInfo = new(data);
        }

        public RussianNounInfo(char gender, bool animate)
            : this(RussianGrammar.ParseGender(gender), animate) { }
        public RussianNounInfo(RussianGender gender, bool animate)
        {
            byte data = (byte)((int)gender | (animate ? 0b_100 : 0));
            _data = data;
            _declensionInfo = new(data);
        }

        public RussianNounInfo(
            RussianGender gender, bool animate, RussianNounFlags flags,
            RussianGender declGender, bool declAnimate,
            RussianDeclensionType declType
        )
        {
            _data = (byte)((int)gender | (animate ? 0b_100 : 0) | ((int)flags << 3));
            _declensionInfo = new(declGender, declAnimate, flags, declType);
        }

        public RussianGender Gender => (RussianGender)(_data & 0b_000_00_011);
        public bool IsAnimate => (_data & 0b_000_00_100) != 0;

        public bool IsTantum => (_data & 0b_000_10_000) != 0;
        public bool IsSingulareTantum => (_data & 0b_000_11_000) == 0b_000_10_000;
        public bool IsPluraleTantum => (_data & 0b_000_11_000) == 0b_000_11_000;

        public RussianGender DeclensionGender => _declensionInfo.Gender;
        public bool DeclensionIsAnimate => _declensionInfo.IsAnimate;
        public RussianDeclensionType DeclensionType => (RussianDeclensionType)_declensionInfo.Case;

        internal bool IsPlural => (_data & 0b_000_01_000) != 0;
        internal RussianCase Case => (RussianCase)(_data >> 5);

        internal RussianNounInfoForDeclension PrepareForDeclension(RussianCase @case, bool plural)
            => _declensionInfo.PrepareForDeclension(@case, plural);

    }
    internal readonly struct RussianNounInfoForDeclension
    {
        // Representation:
        //   xxx_11_111 - mirrors RussianNounInfo's structure
        //
        // - During declension, the three most significant bits represent the case:
        //   111_xx_xxx - case (see RussianCase enum)
        // - And when a part of RussianNounInfo, they represent the declension type:
        //   xxx_11_xxx - declension type (see RussianDeclensionType enum)
        //
        private readonly byte _data;

        public RussianNounInfoForDeclension(byte data)
            => _data = data;
        public RussianNounInfoForDeclension(RussianGender gender, bool animate, RussianNounFlags flags, RussianDeclensionType type)
            => _data = (byte)((int)gender | (animate ? 0b_100 : 0) | ((int)flags << 3) | ((int)type << 5));

        public RussianGender Gender => (RussianGender)(_data & 0b_000_00_011);
        public bool IsAnimate => (_data & 0b_000_00_100) != 0;
        public bool IsPlural => (_data & 0b_000_01_000) != 0;
        public bool IsTantum => (_data & 0b_000_10_000) != 0;
        public RussianCase Case => (RussianCase)(_data >> 5);

        internal bool IsNominativeNormalized
        {
            get
            {
                RussianCase @case = Case;
                return @case == RussianCase.Nominative || @case == RussianCase.Accusative && !IsAnimate;
            }
        }
        internal bool IsGenitiveNormalized
        {
            get
            {
                RussianCase @case = Case;
                return @case == RussianCase.Genitive || @case == RussianCase.Accusative && IsAnimate;
            }
        }

        public RussianNounInfoForDeclension PrepareForDeclension(RussianCase @case, bool plural)
        {
            // If it doesn't have a tantum, apply specified count
            int pluralFlag = IsTantum ? _data & 0b_000_01_000 : plural ? 0b_000_01_000 : 0;
            int data = (_data & 0b_000_00_111) | pluralFlag | ((int)@case << 5);
            return new((byte)data);
        }

    }
}
