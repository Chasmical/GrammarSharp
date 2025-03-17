namespace Chasm.Grammar.English
{
    public sealed class EnglishNoun
    {
        public string Stem { get; }
        public EnglishNounInfo Info { get; }

        public EnglishNoun(string stem)
            : this(stem, default) { }
        public EnglishNoun(string stem, EnglishNounInfo info)
        {
            Stem = stem;
            Info = info;
        }

        public string Decline(bool plural)
        {
            const EnglishNounFlags tantums = EnglishNounFlags.IsSingulareTantum | EnglishNounFlags.IsPluraleTantum;
            if (!plural || (Info.Flags & tantums) == EnglishNounFlags.IsSingulareTantum) return Stem;

            switch (Stem[^1])
            {
                default:
                    return Stem + "s";
            }
        }

    }
}
