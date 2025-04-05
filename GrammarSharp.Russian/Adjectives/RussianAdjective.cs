using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianAdjective
    {
        public string Stem { get; }
        public RussianAdjectiveInfo Info { get; }

        public RussianAdjective(ReadOnlySpan<char> word, RussianAdjectiveInfo info)
        {
            Stem = info._declension.ExtractStem(word).ToString();
            Info = info;
        }
        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            Guard.ThrowIfNull(word);
            Stem = info._declension.ExtractStem(word);
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianAdjective(string stem, RussianAdjectiveInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        [Pure] public static RussianAdjective FromStem(string stem, RussianAdjectiveInfo info)
        {
            Guard.ThrowIfNull(stem);
            return new RussianAdjective(stem, info, false);
        }

    }
}
