using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianAdjectiveFlags
    {
        None = 0,

        IsReflexive       = 0b_00001,
        NoComparativeForm = 0b_00010,
        Minus             = 0b_00100,
        Cross             = 0b_01000,
        BoxedCross        = 0b_01100,
    }
}
