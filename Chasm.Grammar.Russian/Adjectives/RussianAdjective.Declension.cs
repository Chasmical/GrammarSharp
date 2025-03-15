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

            // TODO: transformations

            // Add 'ся' to the end, if it's a reflexive adjective
            if ((info.Flags & RussianAdjectiveFlags.IsReflexive) != 0)
                results.AppendToEnding('с', 'я');

            return results.Result.ToString();
        }

    }
}
