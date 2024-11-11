namespace Chasm.Grammar.Russian
{
    public readonly struct RussianNounInfo
    {
        // Representation:
        //   xxx_xx_x11 - gender (see RussianGender enum)
        //   xxx_xx_1xx - animacy (0 - inanimate, 1 - animate)
        //   xxx_x1_xxx - tantum indicator (0 - nothing, 1 - current "is plural" value is the tantum)
        //   xxx_1x_xxx - [[is plural]] (0 - singular, 1 - plural)
        //   111_xx_xxx - [[case]] (see RussianCase enum)
        //
        // Notes:
        //   Values enclosed in [[]] are only used internally during noun declension,
        //   and aren't supposed to represent any specific noun's characteristics.
        //
        //   TODO: maybe it'd be better to not pack RussianNounInfo's values
        //
        //   TODO: figure out where to store declension type (n/adj/pro) and declension gender
        //         (мужчина - мо <жо 1a>) (леший - мо <п 4a>)
        //
        private readonly byte _data;

        private RussianNounInfo(byte data)
            => _data = data;
        public RussianNounInfo(char gender, bool animate)
            => _data = (byte)((int)RussianGrammar.ParseGender(gender) | (animate ? 0b_100 : 0));

        public RussianGender Gender => (RussianGender)(_data & 0b_000_00_011);
        public bool IsAnimate => (_data & 0b_000_00_100) != 0;

        public bool IsTantum => (_data & 0b_000_01_000) != 0;
        public bool IsSingulareTantum => (_data & 0b_000_11_000) == 0b_000_01_000;
        public bool IsPluraleTantum => (_data & 0b_000_11_000) == 0b_000_11_000;

        internal bool IsPlural => (_data & 0b_000_10_000) != 0;
        internal RussianCase Case => (RussianCase)((_data & 0b_111_00_000) >> 5);

        internal RussianNounInfo PrepareForDeclension(RussianCase @case, bool plural)
        {
            int pluralFlag = IsTantum ? _data & 0b_000_10_000 : plural ? 0b_000_10_000 : 0;
            int data = (_data & 0b_000_00_111) | pluralFlag | ((int)@case << 5);
            return new RussianNounInfo((byte)data);
        }

    }
}
