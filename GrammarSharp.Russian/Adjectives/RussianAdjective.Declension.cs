using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    using AdjDecl = RussianAdjectiveDeclension;
    using SubjProps = RussianNounProperties;

    public sealed partial class RussianAdjective
    {
        [Pure] public string Decline(RussianCase @case, bool plural, SubjProps properties)
        {
            // Normalize "2nd" cases to the main 6 cases (anomalies can only be in main 6 cases)
            RussianGrammar.ValidateAndNormalizeCase(ref @case, ref plural);
            // Prepare the props for adjective/pronoun declension, store case in props, prepare gender-count
            properties.PrepareForDeclensionGenderCount(@case, plural);

            // See if the adjective has any anomalous forms for this case and gender
            if (_anomalies.GetForAdjective(@case, properties.Gender) is { } anomaly) return anomaly;

            // Start attempting to decline the adjective using its declension
            RussianDeclension declension = Info.Declension;
            if (declension.IsZero) return Stem;

            if (declension.Type == RussianDeclensionType.Adjective)
            {
                return DeclineCore(Stem, declension.AsAdjectiveUnsafeRef(), properties);
            }
            else // if (declension.Type == RussianDeclensionType.Pronoun)
            {
                return RussianPronoun.DeclineCore(Stem, declension.AsPronounUnsafeRef(), properties);
            }
        }

        [Pure] public string? DeclineShort(bool plural, SubjProps properties, bool force = false)
        {
            // Prepare the props for adjective declension, store "7th case" in props
            properties.PrepareForDeclensionGenderCount((RussianCase)6, plural);

            // See if the adjective has any anomalous forms for this count
            if (_anomalies.GetShortFormForAdjective(properties.Gender) is { } anomaly) return anomaly;

            if (Info.Declension.Type != RussianDeclensionType.Adjective) return null;
            if ((Info.Flags & (RussianAdjectiveFlags.IsNumeral | RussianAdjectiveFlags.IsPronoun)) != 0) return null;

            RussianAdjectiveFlags flags = Info.Flags & RussianAdjectiveFlags.BoxedCross;
            if (flags != 0)
            {
                if (
                    // If not forced, and there are difficulties forming short form for all types (cross and boxed cross);
                    !force && flags >= RussianAdjectiveFlags.Cross ||

                    // At this point, flags only concern the masculine short form (Minus or BoxedCross):
                    // - if force is false, return null always (Minus or BoxedCross) if masc (no short form, or difficulty)
                    // - if force is true, return null on BoxedCross if masc (no short form)
                    (!force || flags == RussianAdjectiveFlags.BoxedCross) &&
                    !plural && !properties.IsPluraleTantum && properties.Gender == RussianGender.Masculine
                )
                    return null; // (either no short form, or difficulty forming)
            }

            RussianDeclension declension = Info.Declension;
            if (declension.IsZero) return Stem;

            // Decline like an adjective with "7th case" (used in the ending lookup)
            return DeclineCore(Stem, declension.AsAdjectiveUnsafeRef(), properties);
        }

        [Pure] public string? DeclineComparative(bool force = false)
        {
            if (_anomalies.GetComparativeForAdjective() is { } anomaly) return anomaly;

            RussianDeclension declension = Info.Declension;
            if (declension.IsZero || declension.Type != RussianDeclensionType.Adjective) return null;
            if ((Info.Flags & RussianAdjectiveFlags.NoComparativeForm) != 0) return null;
            AdjDecl decl = declension.AsAdjectiveUnsafeRef();

            ReadOnlySpan<char> stem = Stem;
            int length = stem.Length;
            Span<char> buffer = stackalloc char[length + 2];
            stem.CopyTo(buffer);
            // Add a 'е' to the ending (ч|е, ж|е, ш|е, |ее)
            buffer[length] = 'е';

            int yoIndex;
            switch (stem[^1])
            {
                case 'к': // Replace 'к' with 'ч'
                    buffer[length - 1] = 'ч';
                    break;
                case 'г': // Replace 'г' with 'ж'
                    buffer[length - 1] = 'ж';
                    break;
                case 'х': // Replace 'х' with 'ш'
                    buffer[length - 1] = 'ш';
                    break;

                default:
                    // Replace 'ё' in the stem with 'е', since the stress is always on the 'ее' ending.
                    // (unless the stress pattern is exactly a/a, in which case it is on the stem)
                    yoIndex = stem.IndexOf('ё');
                    if (yoIndex >= 0 && !decl.StressPattern.IsDoubleA())
                        buffer[yoIndex] = 'е';

                    // Add the last 'е' to complete the 'ее' ending
                    buffer[length + 1] = 'е';
                    // Construct and return the string with double-char ending (|ее)
                    return buffer.ToString();
            }

            // Here, stress should be on the last stem syllable.
            // If the stem contains a 'ё', and there's a vowel after it, replace 'ё' with 'е'.
            yoIndex = stem.IndexOf('ё');
            if (yoIndex >= 0 && yoIndex != RussianLowerCase.LastIndexOfVowel(stem))
                buffer[yoIndex] = 'е';

            // Construct and return the string with single-char ending (ч|е, ж|е, ш|е)
            return buffer[..(length + 1)].ToString();
        }

        [Pure] internal static string DeclineCore(ReadOnlySpan<char> stem, AdjDecl decl, SubjProps props)
        {
            if (decl.IsZero) return stem.ToString();

            // Allocate some memory for string manipulations
            const int extraCharCount = 8;
            Span<char> buffer = stackalloc char[stem.Length + extraCharCount];
            InflectionBuffer results = new(buffer);

            // Find the appropriate adjective ending
            ReadOnlySpan<char> ending = DetermineEnding(decl, props);
            // Write the stem and ending into the buffer
            results.WriteInitialParts(stem, ending);

            RussianDeclensionFlags flags = decl.Flags;

            // Apply short form-specific transformations
            if (props.Case == (RussianCase)6)
            {
                // If declension has a ① or ② mark, remove the redundant 'н' in short form
                if (
                    (flags & RussianDeclensionFlags.CircledTwo) != 0 ||
                    (flags & RussianDeclensionFlags.CircledOne) != 0 && props.Gender == RussianGender.Masculine
                )
                {
                    results.ShrinkStemBy(1);
                }
                else
                {
                    // If declension has a star, figure out the vowel and where to place it in short form
                    if ((flags & RussianDeclensionFlags.Star) != 0)
                        ProcessShortFormVowelAlternation(ref results, decl, props);
                }
            }

            // If declension has an alternating ё, figure out if it needs to be moved
            if ((flags & RussianDeclensionFlags.AlternatingYo) != 0)
                ProcessYoAlternation(ref results, decl, props);

            // Add 'ся' to the end, if it's a reflexive adjective
            if (decl.IsReflexive)
                results.AppendToEnding('с', 'я');

            return results.Result.ToString();
        }

        private static void ProcessShortFormVowelAlternation(ref InflectionBuffer buffer, AdjDecl decl, SubjProps props)
        {
            if (props.Gender == RussianGender.Masculine)
            {
                // B) vowel alternation type (masculine short form)

                int lastConsonantIndex = RussianLowerCase.LastIndexOfConsonant(buffer.Stem);
                if (lastConsonantIndex < 1) throw new InvalidOperationException();

                char preLastChar = buffer.Buffer[lastConsonantIndex - 1];
                char lastChar = buffer.Buffer[lastConsonantIndex];

                // Special exception for adjectives ending with '-ний' - remove 'ь' ending
                if (lastChar is 'н' && decl.StemType == 2 && decl.StressPattern.Alt == RussianStress.A)
                {
                    buffer.RemoveEnding();
                }

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

        private static void ProcessYoAlternation(ref InflectionBuffer buffer, AdjDecl decl, SubjProps props)
        {
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
                    RussianLowerCase.IndexOfVowel(buffer.Ending) == -1 ||
                    !IsEndingStressed(decl, props) && letterIndex == RussianLowerCase.IndexOfVowel(buffer.Stem)
                )
                {
                    buffer.Buffer[letterIndex] = 'ё';
                }
            }
        }

    }
}
