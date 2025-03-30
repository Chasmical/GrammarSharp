using System;

namespace GrammarSharp.Russian
{
    [Flags]
    public enum RussianVerbFlags
    {
        Perfective    = 1 << 0,
        Imperfective  = 1 << 1,
        Intransitive  = 1 << 2,
        Reflexive     = 1 << 3, // TODO: is it really needed though? should it be processed automatically in ctor?
        Impersonal    = 1 << 4,
        Frequentative = 1 << 5,

    }
}
