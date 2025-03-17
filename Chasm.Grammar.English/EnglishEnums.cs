using System;

namespace Chasm.Grammar.English
{
    public enum EnglishCase
    {
        Nominative,
        // TODO
    }
    [Flags]
    public enum EnglishNounFlags
    {
        IsSingulareTantum = 0b_10,
        IsPluraleTantum   = 0b_11,
    }
}
