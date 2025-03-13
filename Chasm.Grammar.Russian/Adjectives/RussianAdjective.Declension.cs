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

            // Prepare information for declension, and use the appropriate declension method
            switch (Info.Declension.Type)
            {
                case RussianDeclensionType.Adjective:
                    props.PrepareForAdjectiveDeclension(@case, plural);
                    return DeclineCore(Stem, Info, props);

                default:
                    throw new NotImplementedException("Declension for other types not implemented yet.");
            }
        }
        [Pure] public string DeclineShort(bool plural, SubjProps properties)
            => Decline((RussianCase)6, plural, properties);

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

            // Move 'ся' to the end, if it's a reflexive adjective
            if (results.StemLength > 2 && results.Stem[^2] == 'с' && results.Stem[^1] == 'я')
            {
                results.ShrinkStemBy(2);
                results.AppendToEnding('с', 'я');
            }

            return results.Result.ToString();
        }

    }
}
