using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianConjugationFlags
    {
        None = 0,

        Star          = 1 << 0,
        Circle        = 1 << 1,
        CircledOne    = 1 << 2,
        CircledTwo    = 1 << 3,
        CircledThree  = 1 << 4,
        CircledFour   = 1 << 5,
        CircledFive   = 1 << 6,
        CircledSix    = 1 << 7,
        CircledSeven  = 1 << 8,
        CircledEight  = 1 << 9,
        CircledNine   = 1 << 10,
        AlternatingYo = 1 << 11,
        AlternatingO  = 1 << 12,
    }
}
