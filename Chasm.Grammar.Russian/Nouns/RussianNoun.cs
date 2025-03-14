namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }

        public RussianNoun(string word, RussianNounInfo info)
        {
            bool isAdjReflexive = false;
            Stem = info.Declension.IsZero ? word : info.Declension.ExtractStem(word, out isAdjReflexive);
            // Store "is reflexive adjective" flag in ExtraData (it's then retrieved during declension)
            if (isAdjReflexive) info.Properties.ExtraData = 1;
            Info = info;
        }

        public bool IsAdjectiveDeclensionAndReflexive => Info.Properties.ExtraData == 1;

    }
}
