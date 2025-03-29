using System;

namespace GrammarSharp
{
    [Flags]
    public enum GrammaticalAspect : ulong
    {
        None,

        Continuous     = 1 << 0,
        Progressive    = 1 << 1,
        Perfective     = 1 << 2,
        Imperfective   = 1 << 3,
        NearPerfective = 1 << 4,
        Discontinuous  = 1 << 5,
        Habitual       = 1 << 6,
        Prospective    = 1 << 7,
        Retrospective  = 1 << 8,
        Determinate    = 1 << 9,
        Indeterminate  = 1 << 10,
        Momentane      = 1 << 11,
        Causative      = 1 << 12,
        Semelfactive   = 1 << 13,
        Gnomic         = 1 << 14,
        Episodic       = 1 << 15,
        Continuative   = 1 << 16,
        Inceptive      = 1 << 17,
        Inchoative     = 1 << 18,
        Cessative      = 1 << 19,
        Defective      = 1 << 20,
        Pausative      = 1 << 21,
        Resumptive     = 1 << 22,
        Punctual       = 1 << 23,
        Delimitative   = 1 << 24,
        Protractive    = 1 << 25,
        Iterative      = 1 << 26,
        Frequentative  = 1 << 27,
        Experiental    = 1 << 28,
        Intentional    = 1 << 29,
        Accidental     = 1 << 30,
        Intensive      = 1UL << 31,
        Attenuative    = 1UL << 32,
        Segmentative   = 1UL << 33,
    }
}
