using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    using AdjInfo = RussianAdjectiveInfo;
    using SubjProps = RussianNounProperties;

    public sealed partial class RussianAdjective
    {
        [Pure] public string Decline(RussianCase @case, bool plural, SubjProps props)
        {
            RussianDeclension declension = Info.Declension;
            if (declension.IsZero) return Stem;

            if (declension.Type == RussianDeclensionType.Adjective)
            {
                // Prepare the props for adjective declension, store case in props
                props.PrepareForAdjectiveDeclension(@case, plural);

                return DeclineCore(Stem, Info, props);
            }
            else
            {
                throw new NotImplementedException("Declension for pronoun-like adjectives not implemented yet.");
            }
        }

        [Pure] public string? DeclineShort(bool plural, SubjProps properties, bool force = false)
        {
            if (Info.Declension.Type != RussianDeclensionType.Adjective) return null;

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

            return Decline((RussianCase)6, plural, properties);
        }

        [Pure] public string? DeclineComparative(bool force = false)
        {
            RussianDeclension declension = Info.Declension;
            if (declension.Type != RussianDeclensionType.Adjective) return null;
            if ((Info.Flags & RussianAdjectiveFlags.NoComparativeForm) != 0) return null;

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
                    if (yoIndex >= 0 && !declension.StressPattern.IsDoubleA())
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

        [Pure] internal static string DeclineCore(ReadOnlySpan<char> stem, AdjInfo info, SubjProps props)
        {
            if (info.Declension.IsZero) return stem.ToString();

            // Allocate some memory for string manipulations
            const int extraCharCount = 8;
            Span<char> buffer = stackalloc char[stem.Length + extraCharCount];
            InflectionBuffer results = new(buffer);

            // Find the appropriate adjective ending
            ReadOnlySpan<char> ending = DetermineAdjectiveEnding(info, props);
            // Write the stem and ending into the buffer
            results.WriteInitialParts(stem, ending);

            RussianDeclensionFlags flags = info.Declension.Flags;

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
                        ProcessShortFormVowelAlternation(ref results, info, props);
                }
            }

            // If declension has an alternating ё, figure out if it needs to be moved
            if ((flags & RussianDeclensionFlags.AlternatingYo) != 0)
                ProcessYoAlternation(ref results, info, props);

            // Add 'ся' to the end, if it's a reflexive adjective
            if ((info.Flags & RussianAdjectiveFlags.IsReflexive) != 0)
                results.AppendToEnding('с', 'я');

            return results.Result.ToString();
        }

        private static void ProcessShortFormVowelAlternation(ref InflectionBuffer buffer, AdjInfo info, SubjProps props)
        {
            if (props.Gender == RussianGender.Masculine)
            {
                // B) vowel alternation type (masculine short form)

                int lastConsonantIndex = RussianLowerCase.LastIndexOfConsonant(buffer.Stem);
                if (lastConsonantIndex < 1) throw new InvalidOperationException();

                char preLastChar = buffer.Buffer[lastConsonantIndex - 1];
                char lastChar = buffer.Buffer[lastConsonantIndex];

                // Special exception for adjectives ending with '-ний' - remove 'ь' ending
                if (lastChar is 'н' && info.Declension.StemType == 2 && info.Declension.StressPattern.Alt == RussianStress.A)
                {
                    buffer.RemoveEnding();
                }

                if (preLastChar is 'ь' or 'й')
                {
                    // 2) if 'ь' or 'й' precedes the stem's last consonant, replace with 'ё' or 'е'
                    buffer.Buffer[lastConsonantIndex - 1] = lastChar != 'ц' && IsEndingStressed(info, props) ? 'ё' : 'е';
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
                    lastChar != 'ц' && IsEndingStressed(info, props)
                        ? RussianLowerCase.IsHissingConsonant(preLastChar) ? 'о' : 'ё'
                        : 'е'
                );
            }
        }

        private static void ProcessYoAlternation(ref InflectionBuffer buffer, AdjInfo info, SubjProps props)
        {
            int letterIndex = buffer.Stem.IndexOf('ё');
            if (letterIndex >= 0)
            {
                if (IsEndingStressed(info, props) && RussianLowerCase.LastIndexOfVowel(buffer.Ending) != -1)
                {
                    buffer.Buffer[letterIndex] = 'е';
                }
            }
            else
            {
                ReadOnlySpan<char> stem = buffer.Stem;
                // For stems with vowel alternation, cut off the last two letters (where an extra 'е' could appear)
                if ((info.Declension.Flags & RussianDeclensionFlags.Star) != 0)
                    stem = stem[..^2];

                letterIndex = stem.LastIndexOf('е');

                if (
                    RussianLowerCase.LastIndexOfVowel(buffer.Ending) == -1 ||
                    !IsEndingStressed(info, props) && (
                        info.Declension.StressPattern.Main is not RussianStress.F and not RussianStress.Fp and not RussianStress.Fpp ||
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
