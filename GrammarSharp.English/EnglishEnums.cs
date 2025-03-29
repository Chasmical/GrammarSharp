using System;

namespace GrammarSharp.English
{
    public enum EnglishCase
    {
        Subjective,
        Objective,
        PossessiveDeterminer,
        PossessivePronoun,
    }
    public enum EnglishTense
    {
        Present,
        Past,
        Future,
    }
    [Flags]
    public enum EnglishAspect
    {
        Continuous        = 0b_01,
        Perfect           = 0b_10,
        PerfectContinuous = 0b_11,
    }
    public enum EnglishVoice
    {
        Active,
        Passive,
    }
    public enum EnglishPerson
    {
        First,
        Second,
        Third,
    }
    public enum EnglishConjugationType
    {
        SingularIs,
        SingularAre,
        SingularAm,
        PluralAre,
    }
    [Flags]
    public enum EnglishNounFlags
    {
        IsProper          = 0b_001,
        IsSingulareTantum = 0b_100,
        IsPluraleTantum   = 0b_110,
    }
}
