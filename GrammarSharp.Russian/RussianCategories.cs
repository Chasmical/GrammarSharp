namespace GrammarSharp.Russian
{
    public enum RussianCase
    {
        Nominative,
        Genitive,
        Dative,
        Accusative,
        Instrumental,
        Prepositional,
    }
    public enum RussianTense
    {
        Past,
        Present,
        Future,
    }
    public enum RussianGender
    {
        Neuter    = 0b_00,
        Masculine = 0b_01,
        Feminine  = 0b_10,
        Common    = 0b_11,
    }
}
