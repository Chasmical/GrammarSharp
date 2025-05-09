﻿using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    using NounDecl = RussianNounDeclension;
    using NounProps = RussianNounProperties;

    public sealed partial class RussianNoun
    {
        [Pure] public string Decline(RussianCase @case, bool plural)
        {
            // See if the noun has any anomalous forms for this case and count
            if (_anomalies.GetForNoun(@case, plural) is { } anomaly) return anomaly;
            // Normalize "2nd" cases to the main 6 cases
            RussianGrammar.ValidateAndNormalizeCase(ref @case, ref plural);

            // Start attempting to decline the noun using its declension
            RussianDeclension declension = Info.Declension;
            if (declension.IsZero) return Stem;

            NounProps props = Info.Properties;
            if (declension.Type == RussianDeclensionType.Noun)
            {
                NounDecl decl = declension.AsNounUnsafeRef();
                // Use special declension properties, if they're specified
                if (decl.SpecialProperties is { } specialProps)
                    props.CopyFromButKeepTantums(specialProps);

                // Prepare the props for noun declension, store case in props
                props.PrepareForDeclensionTwoNumbers(@case, plural);

                return DeclineCore(Stem, decl, props);
            }
            else // if (declension.Type == RussianDeclensionType.Adjective)
            {
                // Prepare the props for adjective declension, store case in props
                props.PrepareForDeclensionGendersAndPlural(@case, plural);

                return RussianAdjective.DeclineCore(Stem, declension.AsAdjectiveUnsafeRef(), props);
            }
        }
        [Pure] public string Decline(RussianCase @case, RussianNumeralAgreement agreement)
        {
            bool plural;
            if (agreement >= RussianNumeralAgreement.PaucalCountForm)
            {
                // See if the noun has an anomalous count form for this count
                plural = agreement == RussianNumeralAgreement.PluralCountForm;
                if (_anomalies.GetCountFormForNoun(plural) is { } anomaly) return anomaly;
                // Otherwise, use the noun's genitive form
                @case = RussianCase.Genitive;
            }
            // Decline regularly
            else
                plural = agreement == RussianNumeralAgreement.DeclinePlural;

            return Decline(@case, plural);
        }

        [Pure] public string DeclineCountForm(bool plural)
        {
            // See if the noun has an anomalous count form for this count
            if (_anomalies.GetCountFormForNoun(plural) is { } anomaly) return anomaly;
            // Otherwise, decline as if it were a genitive case
            return Decline(RussianCase.Genitive, plural);
        }

        [Pure] internal static string DeclineCore(ReadOnlySpan<char> stem, NounDecl decl, NounProps props)
        {
            if (decl.IsZero) return stem.ToString();

            // Allocate some memory for string manipulations
            const int extraCharCount = 8;
            Span<char> buffer = stackalloc char[stem.Length + extraCharCount];
            InflectionBuffer results = new(buffer);

            // Find the appropriate noun ending
            ReadOnlySpan<char> ending = DetermineEnding(decl, props);
            // Write the stem and ending into the buffer
            results.WriteInitialParts(stem, ending);

            // If declension has a circle, apply the systematic alternation
            if ((decl.Flags & RussianDeclensionFlags.Circle) != 0)
                ProcessUniqueAlternation(ref results, decl, props);

            // Replace 'я' in endings with 'а', if it's after a hissing consonant
            if (decl.StemType == 8 && ending.Length > 0 && ending[0] == 'я' && RussianLowerCase.IsHissingConsonant(stem[^1]))
                results.Ending[0] = 'а';

            // If declension has a star, figure out the vowel and where to place it
            if ((decl.Flags & RussianDeclensionFlags.Star) != 0)
                ProcessVowelAlternation(ref results, decl, props);

            if ((decl.Flags & RussianDeclensionFlags.AlternatingYo) != 0)
                ProcessYoAlternation(ref results, decl, props);

            return results.Result.ToString();
        }

        private static void ProcessUniqueAlternation(ref InflectionBuffer buffer, NounDecl decl, NounProps props)
        {
            Span<char> stem = buffer.Stem;

            switch (stem)
            {
                // -ин (-анин/-янин) unique stem alternation
                case [.., 'и', 'н']:
                    if (props.IsPlural)
                    {
                        buffer.ShrinkStemBy(2);

                        if (props.IsNominativeNormalized)
                        {
                            if ((decl.Flags & RussianDeclensionFlags.CircledOne) == 0)
                                buffer.ReplaceEnding('е');
                        }
                        else if (props.IsGenitiveNormalized) buffer.RemoveEnding();
                    }
                    break;
                // -ёнок/-онок unique stem alternation
                case [.., 'ё' or 'о', 'н', 'о', 'к']:
                    if (props.IsPlural)
                    {
                        stem[^4] = stem[^4] == 'ё' ? 'я' : 'а';
                        stem[^3] = 'т';

                        buffer.ShrinkStemBy(2);
                        if (props.IsNominativeNormalized) buffer.ReplaceEnding('а');
                        else if (props.IsGenitiveNormalized) buffer.RemoveEnding();
                    }
                    else
                    {
                        if (!props.IsNominativeNormalized)
                        {
                            int lastVowelIndex = RussianLowerCase.LastIndexOfVowel(buffer.Stem);
                            if (lastVowelIndex < 0) throw new InvalidOperationException();
                            buffer.RemoveStemCharAt(lastVowelIndex);
                        }
                    }
                    break;
                // -ок unique stem alternation
                case [.., _, 'о', 'к']:
                    if (props.IsPlural)
                    {
                        stem[^2] = RussianLowerCase.IsSibilantConsonant(stem[^3]) ? 'а' : 'я';
                        stem[^1] = 'т';

                        if (props.IsNominativeNormalized) buffer.ReplaceEnding('а');
                        else if (props.IsGenitiveNormalized) buffer.RemoveEnding();
                    }
                    else
                    {
                        // Remove 'о' in the stem (vowel alternation)
                        if (!props.IsNominativeNormalized)
                            buffer.RemoveStemCharAt(buffer.StemLength - 2);
                    }
                    break;
                // -ёночек/-оночек unique stem alternation
                case [.., 'ё' or 'о', 'н', 'о', 'ч', 'е', 'к']:
                    if (props.IsPlural)
                    {
                        stem[^6] = stem[^6] == 'ё' ? 'я' : 'а';
                        stem[^5] = 'т';
                        stem[^4] = 'к';

                        buffer.ShrinkStemBy(3);
                        if (props.IsNominativeNormalized)
                            buffer.ReplaceEnding('и');
                        else if (props.IsGenitiveNormalized)
                        {
                            buffer.RemoveEnding();
                            buffer.InsertBetweenTwoLastStemChars('о');
                        }
                    }
                    else
                    {
                        // Remove 'о' in the stem (vowel alternation)
                        if (!props.IsNominativeNormalized)
                            buffer.RemoveStemCharAt(buffer.StemLength - 2);
                    }
                    break;
                // -мя unique stem alternation
                case [.., 'м'] when props.Gender == RussianGender.Neuter:
                    if (!props.IsNominativeNormalized || props.IsPlural)
                    {
                        bool useYo = (decl.Flags & RussianDeclensionFlags.AlternatingYo) != 0
                                  && props.IsPlural && props.IsGenitiveNormalized;
                        buffer.AppendToStem(useYo ? 'ё' : 'е', 'н');
                    }

                    if (!props.IsPlural)
                    {
                        switch (props.Case)
                        {
                            case RussianCase.Nominative:
                                buffer.ReplaceEnding('я');
                                break;
                            case RussianCase.Genitive:
                            case RussianCase.Dative:
                            case RussianCase.Prepositional:
                                buffer.ReplaceEnding('и');
                                break;
                            case RussianCase.Accusative:
                                if (props.IsAnimate) goto case RussianCase.Genitive;
                                goto case RussianCase.Nominative;
                            case RussianCase.Instrumental:
                                buffer.ReplaceEnding('е', 'м');
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

        private static void ProcessVowelAlternation(ref InflectionBuffer buffer, NounDecl decl, NounProps props)
        {
            if (props.Gender == RussianGender.Masculine || props.Gender == RussianGender.Feminine && decl.StemType == 8)
            {
                // A) vowel alternation type (masc any / fem 8*)

                int lastVowelIndex = RussianLowerCase.LastIndexOfVowel(buffer.Stem);
                if (lastVowelIndex == -1) throw new InvalidOperationException();

                if (!props.IsPlural)
                {
                    // Singular count nominative case is unchanged (and accusative for inanimate nouns too)
                    if (props.IsNominativeNormalized)
                        return;
                    // Special exception: fem 8* nouns in instrumental case are unchanged (ending with 'ью')
                    if (props.Gender == RussianGender.Feminine && props.Case == RussianCase.Instrumental)
                        return;
                }

                switch (buffer.Buffer[lastVowelIndex])
                {
                    case 'о': // 'о' is removed
                        buffer.RemoveStemCharAt(lastVowelIndex);
                        break;
                    case 'е' or 'ё':
                        char preceding = lastVowelIndex > 0 ? buffer.Buffer[lastVowelIndex - 1] : '\0';

                        if (RussianLowerCase.IsVowel(preceding))
                        {
                            // 1) is replaced with 'й', when after a vowel
                            buffer.Buffer[lastVowelIndex] = 'й';
                        }
                        else if (
                            // 2)a) is *always* replaced with 'ь', when noun is masc 6*
                            decl.StemType == 6 ||
                            // 2)b) is replaced with 'ь', when noun is masc 3* and it's after a non-sibilant consonant
                            decl.StemType == 3 && RussianLowerCase.IsNonSibilantConsonant(preceding) ||
                            // 2)c) is replaced with 'ь', when after 'л'
                            preceding == 'л'
                        )
                        {
                            buffer.Buffer[lastVowelIndex] = 'ь';
                        }
                        else
                        {
                            // 3) in all other cases, is removed
                            buffer.RemoveStemCharAt(lastVowelIndex);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            // TODO: Wiktionary: if ②, force B) type, while setting gender to neuter and forcing genitive-plural stem???
            else if (props.Gender is RussianGender.Neuter or RussianGender.Feminine)
            {
                // B) vowel alternation type (neuter any / fem any, except for fem 8* which was processed earlier)

                // B) type only affects plural count and genitive case (and accusative for animate nouns too)
                if (props.IsPlural && props.IsGenitiveNormalized)
                {
                    // Undocumented exceptions to the rule (2*b, 2*f, 2*②, and neuter ②)
                    if (decl.StemType == 2 && decl.Stress is RussianStress.B or RussianStress.F)
                        return;

                    // If the noun is marked with ②, then it uses a different gender's endings,
                    // and for some reason that also means that vowel alternation doesn't apply.
                    if ((decl.Flags & RussianDeclensionFlags.CircledTwo) != 0) return;

                    if (decl.StemType == 6 && buffer.Stem[^1] == 'ь')
                    {
                        // 1) stem's ending 'ь' is replaced with 'е' or 'и'
                        buffer.Stem[^1] = IsEndingStressed(decl, props) ? 'е' : 'и';
                        return;
                    }
                    // Special exception for feminine 2*a nouns ending with 'ня' - remove ending 'ь'.
                    // Note: for nouns matching B) type, only 2*a nouns can have 'ь' as an ending.
                    if (props.Gender == RussianGender.Feminine && buffer.Ending is ['ь'])
                    {
                        // Remove 'ь' from the result
                        buffer.RemoveEnding();
                    }

                    int lastConsonantIndex = RussianLowerCase.LastIndexOfConsonant(buffer.Stem);
                    if (lastConsonantIndex < 1) throw new InvalidOperationException();

                    char preLastChar = buffer.Buffer[lastConsonantIndex - 1];
                    char lastChar = buffer.Buffer[lastConsonantIndex];

                    if (preLastChar is 'ь' or 'й')
                    {
                        // 2) if 'ь' or 'й' precedes the stem's last consonant, replace with 'ё' or 'е'
                        buffer.Buffer[lastConsonantIndex - 1] = lastChar != 'ц' && IsEndingStressed(decl, props) ? 'ё' : 'е';
                        return;
                    }

                    // 3) in all other cases, insert 'о', 'е' or 'ё'
                    if (
                        preLastChar is 'к' or 'г' or 'х' ||
                        lastChar is 'к' or 'г' or 'х' /* OR declension.StemType == 3 */ &&
                        !RussianLowerCase.IsSibilantConsonant(preLastChar)
                    )
                    {
                        // 3)a) after 'к', 'г' or 'х' insert 'о'
                        // 3)b) before 'к', 'г' or 'х', but not after a sibilant, insert 'о'
                        buffer.InsertBetweenTwoLastStemChars('о');
                        return;
                    }

                    // 3)c) if stressed, 'ё' (but after hissing consonants - 'о'); otherwise (or after 'ц'), 'е'
                    buffer.InsertBetweenTwoLastStemChars(
                        lastChar != 'ц' && IsEndingStressed(decl, props)
                            ? RussianLowerCase.IsHissingConsonant(preLastChar) ? 'о' : 'ё'
                            : 'е'
                    );
                }
            }
        }

        private static void ProcessYoAlternation(ref InflectionBuffer buffer, NounDecl decl, NounProps props)
        {
            // Alternating ё is handled in the ° method
            if ((decl.Flags & RussianDeclensionFlags.Circle) != 0) return;

            int letterIndex = buffer.Stem.IndexOf('ё');
            if (letterIndex >= 0)
            {
                if (IsEndingStressed(decl, props) && RussianLowerCase.LastIndexOfVowel(buffer.Ending) != -1)
                {
                    buffer.Buffer[letterIndex] = 'е';
                }
            }
            else
            {
                ReadOnlySpan<char> stem = buffer.Stem;
                // For stems with vowel alternation, cut off the last two letters (where an extra 'е' could appear)
                if ((decl.Flags & RussianDeclensionFlags.Star) != 0)
                    stem = stem[..^2];

                letterIndex = stem.LastIndexOf('е');

                if (
                    RussianLowerCase.LastIndexOfVowel(buffer.Ending) == -1 ||
                    !IsEndingStressed(decl, props) && (
                        decl.Stress is not RussianStress.F and not RussianStress.Fp and not RussianStress.Fpp ||
                        letterIndex == RussianLowerCase.IndexOfVowel(buffer.Stem)
                    )
                )
                {
                    buffer.Buffer[letterIndex] = 'ё';
                }
            }
        }

    }
}
