namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianAdjective
    {
        public string Stem { get; }
        public RussianAdjectiveInfo Info { get; }

        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            Stem = info.Declension.IsZero ? word : info.Declension.ExtractStem(word, out info.IsReflexive);
            Info = info;
        }

    }
}
