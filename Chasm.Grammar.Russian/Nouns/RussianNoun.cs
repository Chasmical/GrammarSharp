using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }

        public RussianNoun(string word, RussianNounInfo info)
        {
            Stem = info.Declension.IsZero ? word : ExtractStem(word);
            Info = info;
        }

        [Pure] public static string ExtractStem(string word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;
        [Pure] public static ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;

    }
}
