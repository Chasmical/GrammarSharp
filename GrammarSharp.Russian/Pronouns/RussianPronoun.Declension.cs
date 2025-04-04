using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    using ProDecl = RussianPronounDeclension;
    using SubjProps = RussianNounProperties;

    public sealed partial class RussianPronoun
    {
        // TODO: RussianPronoun.Decline(RussianCase)

        [Pure] internal static string DeclineCore(ReadOnlySpan<char> stem, ProDecl decl, SubjProps props)
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

            // Process vowel alternation
            if ((decl.Flags & RussianDeclensionFlags.Star) != 0)
                ProcessPronounVowelAlternation(ref results, decl, props);

            return results.Result.ToString();
        }

        private static void ProcessPronounVowelAlternation(ref InflectionBuffer buffer, ProDecl decl, SubjProps props)
        {
            int lastVowelIndex = RussianLowerCase.LastIndexOfVowel(buffer.Stem);
            if (lastVowelIndex == -1) throw new InvalidOperationException();

            // Masculine nominative is unchanged (and accusative for inanimate nouns too)
            if (props.Gender == RussianGender.Masculine && props.IsNominativeNormalized)
                return;

            switch (buffer.Buffer[lastVowelIndex])
            {
                case 'о': // 'о' is removed
                    buffer.RemoveStemCharAt(lastVowelIndex);
                    break;
                case 'и': // 'и' is replaced with 'ь'
                    buffer.Buffer[lastVowelIndex] = 'ь';
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

        [Pure] private static ReadOnlySpan<char> DetermineEnding(ProDecl decl, SubjProps props)
        {
            var (unStressedIndex, stressedIndex) = RussianEndings.GetPronounEndingIndices(decl, props);

            // If the ending depends on the stress, determine the one needed here.
            // If the endings are the same, it doesn't matter which one is used.
            bool stressed = unStressedIndex != stressedIndex && IsEndingStressed(decl, props);

            return RussianEndings.Get(stressed ? stressedIndex : unStressedIndex);
        }

        [Pure] private static bool IsEndingStressed(ProDecl decl, SubjProps props)
        {
            return decl.Stress switch
            {
                RussianStress.A => false,
                RussianStress.B => true,
                RussianStress.F => !props.IsPlural || !props.IsNominativeNormalized,
                _ => throw new InvalidOperationException($"{decl.Stress} is not a valid stress pattern for adjectives."),
            };
        }

    }
}
