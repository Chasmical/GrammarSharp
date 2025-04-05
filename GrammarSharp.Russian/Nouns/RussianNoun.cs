using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }

        public RussianNoun(ReadOnlySpan<char> word, RussianNounInfo info)
        {
            Stem = info._declension.ExtractStem(word).ToString();
            Info = info;
        }
        public RussianNoun(string word, RussianNounInfo info)
        {
            Guard.ThrowIfNull(word);
            Stem = info._declension.ExtractStem(word);
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianNoun(string stem, RussianNounInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        [Pure] public static RussianNoun FromStem(string stem, RussianNounInfo info)
        {
            Guard.ThrowIfNull(stem);
            return new RussianNoun(stem, info, false);
        }

    }
}
