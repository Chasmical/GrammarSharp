using System;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }
        public RussianNounDeclension Declension { get; }

        public RussianNoun(string stem, RussianNounInfo info, RussianNounDeclension declension)
        {
            Guard.ThrowIfNull(stem);

            Stem = stem;
            Info = info;
            Declension = declension;
        }

        public string Decline(RussianCase @case, bool plural)
        {
            // Prepare the noun's information for declension:
            // - Account for tantums (override plural parameter)
            // - Store @case in the structure
            // - Use declension-specific gender/type
            RussianNounInfo info = Info.PrepareForDeclension(@case, plural);

            return DeclineCore(Stem, Declension, info);
        }

        private static string DeclineCore(string stem, RussianNounDeclension declension, RussianNounInfo info)
        {
            if (declension.IsZero) return stem;

            // Find the appropriate noun ending
            string ending = FindNounEnding(declension, info);

            // Allocate some memory for string manipulations
            const int extraCharCount = 8;
            Span<char> buffer = stackalloc char[stem.Length + extraCharCount];

            // Write the stem and ending into the buffer
            stem.AsSpan().CopyTo(buffer);
            ending.AsSpan().CopyTo(buffer[stem.Length..]);

            DeclensionResults res = new(buffer, stem.Length, stem.Length + ending.Length);

            // If declension has a star, figure out the vowel and where to place it
            if ((declension.Flags & RussianDeclensionFlags.Star) != 0)
                ProcessVowelAlternation(declension, info, ref res);

            // TODO: if declension has a circle, figure out the systematic alteration here

            // TODO: [needs some research] Figure out what to do with ①/②/③ for nouns

            // TODO: [needs some research] figure out what to do with alternating ё

            return res.Result.ToString();
        }

        private const string Vowels = "аеёиоуыэюя";
        private const string Consonants = "бвгджзйклмнпрстфхцчшщ";
        private const string NonSibilantConsonants = "бвгдзйклмнпрстфх";
        private const string HissingConsonants = "шжчщ";
        private const string Sibilants = "шжчщц";

        private static void ProcessVowelAlternation(RussianNounDeclension declension, RussianNounInfo info, ref DeclensionResults res)
        {
            if (info.Gender == RussianGender.Masculine || info.Gender == RussianGender.Feminine && declension.Digit == 8)
            {
                // A) vowel alteration type (masc any / fem 8*)

                int lastVowelIndex = res.Stem.LastIndexOfAny(Vowels);
                if (lastVowelIndex == -1) throw new InvalidOperationException();

                if (!info.IsPlural)
                {
                    // Singular count nominative case is unchanged (and accusative for inanimate nouns too)
                    if (info.Case == RussianCase.Nominative || info.Case == RussianCase.Accusative && !info.IsAnimate)
                        return;
                    // Special exception: fem 8* nouns in instrumental case are unchanged (ending with 'ью')
                    if (info.Gender == RussianGender.Feminine && info.Case == RussianCase.Instrumental)
                        return;
                }

                switch (res.Buffer[lastVowelIndex])
                {
                    case 'о': // 'о' is removed
                        res.RemoveStemCharAt(lastVowelIndex);
                        break;
                    case 'е' or 'ё':
                        char preceding = lastVowelIndex > 0 ? res.Buffer[lastVowelIndex - 1] : '\0';

                        if (Vowels.Contains(preceding))
                        {
                            // 1) is replaced with 'й', when after a vowel
                            res.Buffer[lastVowelIndex] = 'й';
                        }
                        else if (
                            // 2)a) is *always* replaced with 'ь', when noun is masc 6*
                            declension.Digit == 6 ||
                            // 2)b) is replaced with 'ь', when noun is masc 3* and it's after a non-sibilant consonant
                            declension.Digit == 3 && NonSibilantConsonants.Contains(preceding) ||
                            // 2)c) is replaced with 'ь', when after 'л'
                            preceding == 'л'
                        )
                        {
                            res.Buffer[lastVowelIndex] = 'ь';
                        }
                        else
                        {
                            // 3) in all other cases, is removed
                            res.RemoveStemCharAt(lastVowelIndex);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            // TODO: Wiktionary: if ②, force B) type, while setting gender to neuter and forcing genitive-plural stem???
            else if (info.Gender is RussianGender.Neuter or RussianGender.Feminine)
            {
                // B) vowel alteration type (neuter any / fem any, except for fem 8* which was processed earlier)

                // B) type only affects plural count and genitive case (and accusative for animate nouns too)
                if (
                    info.IsPlural &&
                    (info.Case == RussianCase.Genitive || info.Case == RussianCase.Accusative && info.IsAnimate)
                )
                {
                    // TODO: Wiktionary: skip b)1) for 2*b and 2*f?
                    // TODO: Wiktionary: skip b)2) for 2*②? (old was for 3*②, 5*② and 6*②)
                    // TODO: Wiktionary: skip b)3) for neuter ②?

                    if (declension.Digit == 6 && res.Stem[^1] == 'ь')
                    {
                        // 1) stem's ending 'ь' is replaced with 'е' or 'и'
                        res.Stem[^1] = IsAccentOnEnding(declension, info) ? 'е' : 'и';
                        return;
                    }
                    // Special exception for feminine 2*a nouns ending with 'ня' - remove ending 'ь'.
                    // Note: for nouns matching B) type, only 2*a nouns can have 'ь' as an ending.
                    if (info.Gender == RussianGender.Feminine && res.Ending is ['ь'])
                    {
                        // Remove 'ь' from the result
                        res.ResultLength--;
                    }

                    int lastConsonantIndex = res.Stem.LastIndexOfAny(Consonants);
                    if (lastConsonantIndex < 1) throw new InvalidOperationException();

                    char preLastChar = res.Buffer[lastConsonantIndex - 1];
                    char lastChar = res.Buffer[lastConsonantIndex];

                    if (preLastChar is 'ь' or 'й')
                    {
                        // 2) if 'ь' or 'й' precedes the stem's last consonant, replace with 'ё' or 'е'
                        res.Buffer[lastConsonantIndex - 1] = lastChar != 'ц' && IsAccentOnEnding(declension, info) ? 'ё' : 'е';
                        return;
                    }

                    // 3) in all other cases, insert 'о', 'е' or 'ё'
                    if (
                        preLastChar is 'к' or 'г' or 'х' ||
                        lastChar is 'к' or 'г' or 'х' /* OR declension.Digit == 3 */ && !Sibilants.Contains(preLastChar)
                    )
                    {
                        // 3)a) after 'к', 'г' or 'х' insert 'о'
                        // 3)b) before 'к', 'г' or 'х', but not after a sibilant, insert 'о'
                        res.InsertBetweenTwoLastStemChars('о');
                        return;
                    }

                    // 3)c) unaccented or before 'ц' - 'е', after hissing consonants - 'о', otherwise accented 'ё'
                    res.InsertBetweenTwoLastStemChars(
                        lastChar == 'ц' || !IsAccentOnEnding(declension, info) ? 'е'
                        : HissingConsonants.Contains(preLastChar) ? 'о'
                        : 'ё'
                    );
                }
            }
        }

        private ref struct DeclensionResults
        {
            public Span<char> Buffer;
            public int StemLength;
            public int ResultLength;

            public DeclensionResults(Span<char> buffer, int stemLength, int resultLength)
            {
                Buffer = buffer;
                StemLength = stemLength;
                ResultLength = resultLength;
            }

            public Span<char> Stem => Buffer[..StemLength];
            public Span<char> Ending => Buffer[StemLength..ResultLength];
            public Span<char> Result => Buffer[..ResultLength];

            public void RemoveStemCharAt(int index)
            {
                for (int i = index + 1; i < ResultLength; i++)
                    Buffer[i - 1] = Buffer[i];
                StemLength--;
                ResultLength--;
            }
            public void InsertBetweenTwoLastStemChars(char ch)
            {
                for (int i = ResultLength; i >= StemLength; i--)
                    Buffer[i] = Buffer[i - 1];

                Buffer[StemLength - 1] = ch;

                StemLength++;
                ResultLength++;
            }

        }

    }
}
