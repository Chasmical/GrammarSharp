namespace GrammarSharp.Russian
{
    public enum RussianNounFlags
    {
        None = 0,

        IsSingulareTantum = 0b_10,
        IsPluraleTantum   = 0b_11,
    }
}
