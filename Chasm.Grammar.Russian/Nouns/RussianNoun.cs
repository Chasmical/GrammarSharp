namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }

        public RussianNoun(string word, RussianNounInfo info)
        {
            string stem;

            if (info.Declension.IsZero)
                stem = word;
            else
            {
                stem = info.Declension.ExtractStem(word, out bool isAdjReflexive);
                // Store "is reflexive adjective" flag in ExtraData (it's then retrieved during declension)
                if (isAdjReflexive) info.Declension.IsReflexiveAdjective = isAdjReflexive;
            }

            Stem = stem;
            Info = info;
        }

    }
}
