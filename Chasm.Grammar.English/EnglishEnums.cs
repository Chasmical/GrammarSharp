using System;

namespace Chasm.Grammar.English
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
