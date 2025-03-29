using JetBrains.Annotations;

namespace GrammarSharp.English
{
    public sealed class EnglishNoun
    {
        public string Word { get; }
        public EnglishNounInfo Info { get; }
        private string? _wordPlural;

        public EnglishNoun(string word)
            : this(word, null, default) { }
        public EnglishNoun(string word, EnglishNounInfo info)
            : this(word, null, info) { }
        public EnglishNoun(string wordSingular, string? wordPlural)
            : this(wordSingular, wordPlural, default) { }
        public EnglishNoun(string wordSingular, string? wordPlural, EnglishNounInfo info)
        {
            Word = wordSingular;
            _wordPlural = wordPlural;
            Info = info;
        }

        [Pure] public string Decline(bool plural)
            => Decline(EnglishCase.Subjective, plural);
        [Pure] public string Decline(EnglishCase @case, bool plural)
        {
            string result = plural ? _wordPlural ??= DeclinePluralCore() : Word;

            if (@case is EnglishCase.PossessiveDeterminer or EnglishCase.PossessivePronoun)
                result = plural && result.EndsWith('s') ? result + "'" : result + "'s";

            return result;
        }

        [Pure] private string DeclinePluralCore()
        {
            string word = Word;
            EnglishNounFlags flags = Info.Flags;

            if ((flags & (EnglishNounFlags.IsSingulareTantum | EnglishNounFlags.IsPluraleTantum)) != 0)
                return word;
            if ((flags & EnglishNounFlags.IsProper) != 0)
                return word + "s";

            switch (word[^1])
            {
                // unity - unities
                // colloquy - colloquies
                case 'y':
                    if (word.Length > 1 && (EnglishLowerCase.IsConsonant(word[^2]) || word is [.., 'q', 'u', _]))
                        return word[..^1] + "ies";
                    break;

                // life - lives
                // knife - knives
                case 'e':
                    if (word is [.., 'i', 'f', _])
                        return word[..^2] + "ves";
                    break;

                // elf - elves
                // dwarf - dwarves
                // leaf - leaves
                // staff - staves
                // (but: hoof - hoofs, roof - roofs)
                case 'f':
                    if (word is [.., 'l' or 'r', _] or [.., 'e', 'a', _] or [.., not 'o', 'o', _])
                        return word[..^1] + "ves";
                    if (word is [.., 'a', 'f', _])
                        return word[..^2] + "ves";
                    break;

                // mass - masses
                // plus - pluses
                // fez - fezes
                case 's' or 'z':
                    return word + "es";

                // hero - heroes
                // potato - potatoes
                // volcano - volcanoes
                // echo - echoes
                // (exceptions like: photos, zeros, pianos, pros, cannot be determined automatically here)
                case 'o':
                    if (word.Length > 1 && EnglishLowerCase.IsConsonant(word[^2]))
                        return word + "es";
                    break;

                // dish - dishes
                // witch - witches
                // brooch - brooches
                case 'h':
                    if (word is [.., 'c' or 's', _])
                        return word + "es";
                    break;
            }
            // all the rest of the words, simply add an "s"
            return word + "s";
        }

    }
}
