namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianAdjective
    {
        public string Stem { get; }
        public RussianAdjectiveInfo Info { get; }

        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            string stem;

            if (info.Declension.IsZero)
                stem = word;
            else
            {
                stem = info.Declension.ExtractStem(word, out bool isAdjReflexive);
                if (isAdjReflexive) info.Flags |= RussianAdjectiveFlags.IsReflexive;
            }

            Stem = stem;
            Info = info;
        }

    }
}
