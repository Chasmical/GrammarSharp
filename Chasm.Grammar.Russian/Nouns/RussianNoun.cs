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
            Stem = info.Declension.IsZero ? word : info.Declension.ExtractStem(word);
            Info = info;
        }

    }
}
