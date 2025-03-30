using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianDeclensionFlags
    {
        None = 0,

        Star          = 1 << 0,
        Circle        = 1 << 1,
        CircledOne    = 1 << 2,
        CircledTwo    = 1 << 3,
        CircledThree  = 1 << 4,
        AlternatingYo = 1 << 5,
    }
}
