using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }

        public RussianNoun(string word, RussianNounInfo info)
        {
            string stem = info.Declension.ExtractStem(word, out bool isAdjReflexive);
            // Store "is reflexive adjective" flag in ExtraData (it's then retrieved during declension)
            if (isAdjReflexive) info.Declension.IsReflexiveAdjective = isAdjReflexive;

            Stem = stem;
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianNoun(string stem, RussianNounInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        [Pure] public static RussianNoun FromStem(string stem, RussianNounInfo info)
            => new RussianNoun(stem, info, false);

    }
}
