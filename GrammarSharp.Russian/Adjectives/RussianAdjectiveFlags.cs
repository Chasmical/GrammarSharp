using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianAdjectiveFlags
    {
        None = 0,

        NoComparativeForm = 0b_00_0001,
        Minus             = 0b_00_0010,
        Cross             = 0b_00_0100,
        BoxedCross        = 0b_00_0110,
        IsNumeral         = 0b_10_0000,
        IsPronoun         = 0b_11_0000,
    }
}
