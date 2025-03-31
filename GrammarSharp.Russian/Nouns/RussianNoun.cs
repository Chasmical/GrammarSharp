using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
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

        [Pure] public static string ExtractStem(string word)
        {
            // Remove the last vowel/'й'/'ь' to get the stem
            return word.Length > 1 && RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;
        }
        [Pure] public static ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word)
        {
            // Remove the last vowel/'й'/'ь' to get the stem
            return word.Length > 1 && RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;
        }

        [Pure] public static RussianNoun FromStem(string stem, RussianNounInfo info)
            => new RussianNoun(stem, info, false);

    }
}
