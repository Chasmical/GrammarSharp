using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianAdjectiveFlags
    {
        None = 0,

        NoComparativeForm = 0b_0001,
        Minus             = 0b_0010,
        Cross             = 0b_0100,
        BoxedCross        = 0b_0110,
    }
}
