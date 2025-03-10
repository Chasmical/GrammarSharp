using System;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }
        public RussianDeclension Declension { get; }

        public RussianNoun(string word, RussianNounInfo info, RussianDeclension declension)
        {
            Stem = declension.IsZero ? word : ExtractStem(word);
            Info = info;
            Declension = declension;
        }

        public static string ExtractStem(string word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;
        public static ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;

    }
}
