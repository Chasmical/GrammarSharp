using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    using AdjDecl = RussianDeclension;
    using SubjProps = RussianNounProperties;

    public sealed partial class RussianAdjective
    {
        [Pure] private static ReadOnlySpan<char> DetermineEnding(AdjDecl decl, SubjProps props)
        {
            var (unStressedIndex, stressedIndex) = RussianEndings.GetAdjectiveEndingIndices(decl, props);

            // If the ending depends on the stress, determine the one needed here.
            // If the endings are the same, it doesn't matter which one is used.
            bool stressed = unStressedIndex != stressedIndex && IsEndingStressed(decl, props);

            return RussianEndings.Get(stressed ? stressedIndex : unStressedIndex);
        }

        [Pure] private static bool IsEndingStressed(AdjDecl decl, SubjProps props)
        {
            RussianStress stress = props.Case == (RussianCase)6
                ? decl.StressPattern.Alt
                : decl.StressPattern.Main;

            return stress switch
            {
                // Full form and short form:
                RussianStress.A => false,
                RussianStress.B => true,
                // Short form only:
                RussianStress.C => props.Gender is RussianGender.Feminine,

                // Stresses with primes are kind of ambiguous...
                //   Ap:  F - ???,    N - stem,   Pl - stem
                //   Bp:  F - ending, N - ending, Pl - ???
                //   Cp:  F - ending, N - stem,   Pl - ???
                //   Cpp: F - ending, N - ???,    Pl - ???
                //
                // I decided to set these values for now:
                //   Ap:  F - stem,   N - stem,   Pl - stem
                //   Bp:  F - ending, N - ending, Pl - ending
                //   Cp:  F - ending, N - stem,   Pl - ending
                //   Cpp: F - ending, N - ending, Pl - ending
                //
                RussianStress.Ap => false,
                RussianStress.Bp => props.Gender is RussianGender.Neuter or RussianGender.Feminine or RussianGender.Common,
                RussianStress.Cp => props.Gender is RussianGender.Feminine or RussianGender.Common,
                RussianStress.Cpp => props.Gender is RussianGender.Neuter or RussianGender.Feminine or RussianGender.Common,

                _ => throw new InvalidOperationException($"{stress} is not a valid stress pattern for adjectives."),
            };
        }

    }
}
