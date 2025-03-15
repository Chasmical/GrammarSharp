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

            if ((info.Declension.Flags & RussianDeclensionFlags.Star) != 0)
                ProcessVowelAlternation(ref results, info, props);

            // TODO: transformations

            // Add 'ся' to the end, if it's a reflexive adjective
            if ((info.Flags & RussianAdjectiveFlags.IsReflexive) != 0)
                results.AppendToEnding('с', 'я');

            return results.Result.ToString();
        }

        private static void ProcessVowelAlternation(ref InflectionBuffer buffer, AdjInfo info, SubjProps props)
        {
            if (props.Gender == RussianGender.Masculine && props.Case == (RussianCase)6)
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

    }
}
