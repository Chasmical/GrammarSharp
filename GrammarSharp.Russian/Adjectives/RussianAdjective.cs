using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianAdjective
    {
        public string Stem { get; }
        public RussianAdjectiveInfo Info { get; }

        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            Stem = info._declension.ExtractStem(word);
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianAdjective(string stem, RussianAdjectiveInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        [Pure] public static string ExtractStem(string word, out bool isAdjReflexive)
            => ExtractStem(word.AsSpan(), out isAdjReflexive).ToString();
        [Pure] public static ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word, out bool isAdjReflexive)
        {
            // If adjective ends with 'ся', remove the last four letters
            if (word.Length > 4 && word[^2] == 'с' && word[^1] == 'я')
            {
                isAdjReflexive = true;
                return word[..^4];
            }
            // Otherwise, remove just the last two letters
            isAdjReflexive = false;
            return word.Length > 2 ? word[..^2] : word;
        }

        [Pure] public static RussianAdjective FromStem(string stem, RussianAdjectiveInfo info)
            => new RussianAdjective(stem, info, false);

    }
}
