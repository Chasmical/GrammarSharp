using System;

namespace Chasm.Grammar.Russian
{
    public sealed partial class RussianNoun
    {
        public string Stem { get; }
        public RussianNounInfo Info { get; }
        public RussianNounDeclension Declension { get; }

        public RussianNoun(string word, RussianNounInfo info, RussianNounDeclension declension)
        {
            Stem = declension.IsZero ? word : ExtractStem(word);
            Info = info;
            Declension = declension;
        }

        public static string ExtractStem(string word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;
        public static ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word)
            => RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;

        public string Decline(RussianCase @case, bool plural)
        {
            // Prepare the noun's information for declension:
            // - Account for tantums (override plural parameter)
            // - Store @case in the structure
            // - Use declension-specific gender/type
            RussianNounInfo info = Info.PrepareForDeclension(@case, plural);
            RussianNounDeclension declension = Declension;

            if (declension.IsZero) return Stem;
            return DeclineCore(Stem, info, declension);
        }

        private static string DeclineCore(ReadOnlySpan<char> stem, RussianNounInfo info, RussianNounDeclension declension)
        {
            if (declension.IsZero) return stem.ToString();

            // Allocate some memory for string manipulations
            const int extraCharCount = 8;
            Span<char> buffer = stackalloc char[stem.Length + extraCharCount];
            DeclensionResults results = new(buffer);

            // Find the appropriate noun ending
            ReadOnlySpan<char> ending = DetermineNounEnding(info, declension);
            // Write the stem and ending into the buffer
            results.WriteInitialParts(stem, ending);

            // If declension has a circle, apply the systematic alternation
            if ((declension.Flags & RussianDeclensionFlags.Circle) != 0)
                ProcessUniqueAlternation(info, declension, ref results);

            // Replace 'я' in endings with 'а', if it's after a hissing consonant
            if (declension.Digit == 8 && ending.Length > 0 && ending[0] == 'я' && RussianLowerCase.IsHissingConsonant(stem[^1]))
                results.Ending[0] = 'а';

            // If declension has a star, figure out the vowel and where to place it
            if ((declension.Flags & RussianDeclensionFlags.Star) != 0)
                ProcessVowelAlternation(info, declension, ref results);

            if ((declension.Flags & RussianDeclensionFlags.AlternatingYo) != 0)
                ProcessYoAlternation(info, declension, ref results);

            return results.Result.ToString();
        }

        private static void ProcessUniqueAlternation(RussianNounInfo info, RussianNounDeclension declension, ref DeclensionResults res)
        {
            Span<char> stem = res.Stem;

            switch (stem)
            {
                // -ин (-анин/-янин) unique stem alternation
                case [.., 'и', 'н']:
                    if (info.IsPlural)
                    {
                        res.ShrinkStemBy(2);

                        if (info.IsNominativeNormalized)
                        {
                            if ((declension.Flags & RussianDeclensionFlags.CircledOne) == 0)
                                res.ReplaceEnding('е');
                        }
                        else if (info.IsGenitiveNormalized) res.RemoveEnding();
                    }
                    break;
                // -ёнок/-онок unique stem alternation
                case [.., 'ё' or 'о', 'н', 'о', 'к']:
                    if (info.IsPlural)
                    {
                        stem[^4] = stem[^4] == 'ё' ? 'я' : 'а';
                        stem[^3] = 'т';

                        res.ShrinkStemBy(2);
                        if (info.IsNominativeNormalized) res.ReplaceEnding('а');
                        else if (info.IsGenitiveNormalized) res.RemoveEnding();
                    }
                    else
                    {
                        if (!info.IsNominativeNormalized)
                        {
                            int lastVowelIndex = RussianLowerCase.LastIndexOfVowel(res.Stem);
                            if (lastVowelIndex < 0) throw new InvalidOperationException();
                            res.RemoveStemCharAt(lastVowelIndex);
                        }
                    }
                    break;
                // -ок unique stem alternation
                case [.., _, 'о', 'к']:
                    if (info.IsPlural)
                    {
                        stem[^2] = RussianLowerCase.IsSibilantConsonant(stem[^3]) ? 'а' : 'я';
                        stem[^1] = 'т';

                        if (info.IsNominativeNormalized) res.ReplaceEnding('а');
                        else if (info.IsGenitiveNormalized) res.RemoveEnding();
                    }
                    else
                    {
                        // Remove 'о' in the stem (vowel alternation)
                        if (!info.IsNominativeNormalized)
                            res.RemoveStemCharAt(res.StemLength - 2);
                    }
                    break;
                // -ёночек/-оночек unique stem alternation
                case [.., 'ё' or 'о', 'н', 'о', 'ч', 'е', 'к']:
                    if (info.IsPlural)
                    {
                        stem[^6] = stem[^6] == 'ё' ? 'я' : 'а';
                        stem[^5] = 'т';
                        stem[^4] = 'к';

                        res.ShrinkStemBy(3);
                        if (info.IsNominativeNormalized)
                            res.ReplaceEnding('и');
                        else if (info.IsGenitiveNormalized)
                        {
                            res.RemoveEnding();
                            res.InsertBetweenTwoLastStemChars('о');
                        }
                    }
                    else
                    {
                        // Remove 'о' in the stem (vowel alternation)
                        if (!info.IsNominativeNormalized)
                            res.RemoveStemCharAt(res.StemLength - 2);
                    }
                    break;
                // -мя unique stem alternation
                case [.., 'м'] when info.Gender == RussianGender.Neuter:
                    if (!info.IsNominativeNormalized || info.IsPlural)
                    {
                        bool useYo = (declension.Flags & RussianDeclensionFlags.AlternatingYo) != 0
                                  && info.IsPlural && info.IsGenitiveNormalized;
                        res.AppendToStem(useYo ? 'ё' : 'е', 'н');
                    }

                    if (!info.IsPlural)
                    {
                        switch (info.Case)
                        {
                            case RussianCase.Nominative:
                                res.ReplaceEnding('я');
                                break;
                            case RussianCase.Genitive:
                            case RussianCase.Dative:
                            case RussianCase.Prepositional:
                                res.ReplaceEnding('и');
                                break;
                            case RussianCase.Accusative:
                                if (info.IsAnimate) goto case RussianCase.Genitive;
                                goto case RussianCase.Nominative;
                            case RussianCase.Instrumental:
                                res.ReplaceEnding('е', 'м');
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void ProcessVowelAlternation(RussianNounInfo info, RussianNounDeclension declension, ref DeclensionResults res)
        {
            if (info.Gender == RussianGender.Masculine || info.Gender == RussianGender.Feminine && declension.Digit == 8)
            {
                // A) vowel alternation type (masc any / fem 8*)

                int lastVowelIndex = RussianLowerCase.LastIndexOfVowel(res.Stem);
                if (lastVowelIndex == -1) throw new InvalidOperationException();

                if (!info.IsPlural)
                {
                    // Singular count nominative case is unchanged (and accusative for inanimate nouns too)
                    if (info.IsNominativeNormalized)
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

                        if (RussianLowerCase.IsVowel(preceding))
                        {
                            // 1) is replaced with 'й', when after a vowel
                            res.Buffer[lastVowelIndex] = 'й';
                        }
                        else if (
                            // 2)a) is *always* replaced with 'ь', when noun is masc 6*
                            declension.Digit == 6 ||
                            // 2)b) is replaced with 'ь', when noun is masc 3* and it's after a non-sibilant consonant
                            declension.Digit == 3 && RussianLowerCase.IsNonSibilantConsonant(preceding) ||
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
                // B) vowel alternation type (neuter any / fem any, except for fem 8* which was processed earlier)

                // B) type only affects plural count and genitive case (and accusative for animate nouns too)
                if (info.IsPlural && info.IsGenitiveNormalized)
                {
                    // Undocumented exceptions to the rule (2*b, 2*f, 2*②, and neuter ②)
                    if (declension.Digit == 2 && declension.Letter is RussianDeclensionAccent.B or RussianDeclensionAccent.F)
                        return;

                    // If the noun is marked with ②, then it uses a different gender's endings,
                    // and for some reason that also means that vowel alternation doesn't apply.
                    if ((declension.Flags & RussianDeclensionFlags.CircledTwo) != 0) return;

                    if (declension.Digit == 6 && res.Stem[^1] == 'ь')
                    {
                        // 1) stem's ending 'ь' is replaced with 'е' or 'и'
                        res.Stem[^1] = IsAccentOnEnding(info, declension) ? 'е' : 'и';
                        return;
                    }
                    // Special exception for feminine 2*a nouns ending with 'ня' - remove ending 'ь'.
                    // Note: for nouns matching B) type, only 2*a nouns can have 'ь' as an ending.
                    if (info.Gender == RussianGender.Feminine && res.Ending is ['ь'])
                    {
                        // Remove 'ь' from the result
                        res.RemoveEnding();
                    }

                    int lastConsonantIndex = RussianLowerCase.LastIndexOfConsonant(res.Stem);
                    if (lastConsonantIndex < 1) throw new InvalidOperationException();

                    char preLastChar = res.Buffer[lastConsonantIndex - 1];
                    char lastChar = res.Buffer[lastConsonantIndex];

                    if (preLastChar is 'ь' or 'й')
                    {
                        // 2) if 'ь' or 'й' precedes the stem's last consonant, replace with 'ё' or 'е'
                        res.Buffer[lastConsonantIndex - 1] = lastChar != 'ц' && IsAccentOnEnding(info, declension) ? 'ё' : 'е';
                        return;
                    }

                    // 3) in all other cases, insert 'о', 'е' or 'ё'
                    if (
                        preLastChar is 'к' or 'г' or 'х' ||
                        lastChar is 'к' or 'г' or 'х' /* OR declension.Digit == 3 */ && !RussianLowerCase.IsSibilantConsonant(preLastChar)
                    )
                    {
                        // 3)a) after 'к', 'г' or 'х' insert 'о'
                        // 3)b) before 'к', 'г' or 'х', but not after a sibilant, insert 'о'
                        res.InsertBetweenTwoLastStemChars('о');
                        return;
                    }

                    // 3)c) unaccented or before 'ц' - 'е', after hissing consonants - 'о', otherwise accented 'ё'
                    res.InsertBetweenTwoLastStemChars(
                        lastChar == 'ц' || !IsAccentOnEnding(info, declension) ? 'е'
                        : RussianLowerCase.IsHissingConsonant(preLastChar) ? 'о'
                        : 'ё'
                    );
                }
            }
        }

        private static void ProcessYoAlternation(RussianNounInfo info, RussianNounDeclension declension, ref DeclensionResults res)
        {
            // Alternating ё is handled in the ° method
            if ((declension.Flags & RussianDeclensionFlags.Circle) != 0) return;

            int letterIndex = res.Stem.IndexOf('ё');
            if (letterIndex >= 0)
            {
                if (IsAccentOnEnding(info, declension) && RussianLowerCase.LastIndexOfVowel(res.Ending) != -1)
                {
                    res.Buffer[letterIndex] = 'е';
                }
            }
            else
            {
                ReadOnlySpan<char> stem = res.Stem;
                // For stems with vowel alternation, cut off the last two letters (where an extra 'е' could appear)
                if ((declension.Flags & RussianDeclensionFlags.Star) != 0)
                    stem = stem[..^2];

                letterIndex = stem.LastIndexOf('е');

                if (
                    RussianLowerCase.LastIndexOfVowel(res.Ending) == -1 ||
                    !IsAccentOnEnding(info, declension) && (
                        declension.Letter is not RussianDeclensionAccent.F and not RussianDeclensionAccent.Fp and not RussianDeclensionAccent.Fpp ||
                        letterIndex == RussianLowerCase.IndexOfVowel(res.Stem)
                    )
                )
                {
                    res.Buffer[letterIndex] = 'ё';
                }
            }
        }

        private ref struct DeclensionResults
        {
            public readonly Span<char> Buffer;
            public int StemLength;
            private int ResultLength;

            public DeclensionResults(Span<char> buffer)
                => Buffer = buffer;

            public readonly Span<char> Stem => Buffer[..StemLength];
            public readonly Span<char> Ending => Buffer[StemLength..ResultLength];
            public readonly Span<char> Result => Buffer[..ResultLength];

            public void WriteInitialParts(ReadOnlySpan<char> stem, ReadOnlySpan<char> ending)
            {
                StemLength = stem.Length;
                ResultLength = StemLength + ending.Length;

                stem.CopyTo(Buffer);
                ending.CopyTo(Buffer[stem.Length..]);
            }

            public void RemoveStemCharAt(int index)
            {
                for (int i = index + 1; i < ResultLength; i++)
                    Buffer[i - 1] = Buffer[i];
                StemLength--;
                ResultLength--;
            }
            public void ShrinkStemBy(int offset)
            {
                StemLength -= offset;
                ResultLength -= offset;
                for (int i = StemLength; i < ResultLength; i++)
                    Buffer[i] = Buffer[i + offset];
            }
            public void AppendToStem(char a, char b)
            {
                for (int i = ResultLength - 1; i >= StemLength; i--)
                    Buffer[i + 2] = Buffer[i];
                Buffer[StemLength] = a;
                Buffer[StemLength + 1] = b;
                StemLength += 2;
                ResultLength += 2;
            }

            public void RemoveEnding()
                => ResultLength = StemLength;
            public void ReplaceEnding(char a)
            {
                Buffer[StemLength] = a;
                ResultLength = StemLength + 1;
            }
            public void ReplaceEnding(char a, char b)
            {
                Buffer[StemLength] = a;
                Buffer[StemLength + 1] = b;
                ResultLength = StemLength + 2;
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
