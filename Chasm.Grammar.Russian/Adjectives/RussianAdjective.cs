using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianAdjective
    {
        public string Stem { get; }
        public RussianAdjectiveInfo Info { get; }

        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            string stem = info.Declension.ExtractStem(word, out bool isAdjReflexive);
            if (isAdjReflexive) info.Flags |= RussianAdjectiveFlags.IsReflexive;

            Stem = stem;
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianAdjective(string stem, RussianAdjectiveInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        [Pure] public static RussianAdjective FromStem(string stem, RussianAdjectiveInfo info)
            => new RussianAdjective(stem, info, false);

    }
}
