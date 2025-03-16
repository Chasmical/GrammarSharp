using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    using NounDecl = RussianDeclension;
    using NounProps = RussianNounProperties;

    public sealed partial class RussianNoun
    {
        [Pure] private static ReadOnlySpan<char> DetermineEnding(NounDecl decl, NounProps props)
        {
            const RussianDeclensionFlags allCircledNumbers
                = RussianDeclensionFlags.CircledOne |
                  RussianDeclensionFlags.CircledTwo |
                  RussianDeclensionFlags.CircledThree;

            // Handle ①②③ marks
            if ((decl.Flags & allCircledNumbers) != 0)
            {
                string? res = GetCircledNumberOverrideEnding(decl, props);
                if (res is not null) return res;
            }

            var (unStressedIndex, stressedIndex) = RussianEndings.GetNounEndingIndices(decl, props);

            // If the ending depends on the stress, determine the one needed here.
            // If the endings are the same, it doesn't matter which one is used.
            bool stressed = unStressedIndex != stressedIndex && IsEndingStressed(decl, props);

            return RussianEndings.Get(stressed ? stressedIndex : unStressedIndex);
        }

        [Pure] private static string? GetCircledNumberOverrideEnding(NounDecl decl, NounProps props)
        {
            if (props.IsPlural)
            {
                int stemType;
                if ((decl.Flags & RussianDeclensionFlags.CircledOne) != 0 && props.IsNominativeNormalized)
                {
                    stemType = decl.StemType;
                    switch (props.Gender)
                    {
                        case RussianGender.Neuter:
                            return stemType is 1 or 5 or 8 ? "ы" : "и";
                        case RussianGender.Masculine:
                            return stemType is 1 or 3 or 4 or 5 ? "а" : "я";
                        case RussianGender.Feminine:
                            throw new InvalidOperationException();
                    }
                }
                if ((decl.Flags & RussianDeclensionFlags.CircledTwo) != 0 && props.IsGenitiveNormalized)
                {
                    stemType = decl.StemType;
                    switch (props.Gender)
                    {
                        case RussianGender.Neuter:
                            return stemType switch
                            {
                                1 or 3 or 8 => "ов",
                                4 or 5 when IsEndingStressed(decl, props) => "ов",
                                2 or 6 or 7 when IsEndingStressed(decl, props) => "ёв",
                                _ => "ев",
                            };
                        case RussianGender.Masculine:
                            return stemType is 1 or 3 or 4 or 5 ? "" : "ь";
                        case RussianGender.Feminine:
                            return "ей";
                    }
                }
            }
            else // if (info.IsSingular)
            {
                if ((decl.Flags & RussianDeclensionFlags.CircledThree) != 0 && decl.StemType == 7)
                {
                    if (props.Case == RussianCase.Prepositional || props.Gender == RussianGender.Feminine && props.Case == RussianCase.Dative)
                        return "е";
                }
            }
            return null;
        }

        [Pure] private static bool IsEndingStressed(NounDecl decl, NounProps props)
        {
            bool plural = props.IsPlural;

            // Accusative case's endings and stresses depend on the noun's animacy.
            // Some stresses, though, depend on the original case, like d′ and f′.
            RussianCase normCase = props.Case;
            if (normCase == RussianCase.Accusative)
                normCase = props.IsAnimate ? RussianCase.Genitive : RussianCase.Nominative;

            return decl.StressPattern.Main switch
            {
                RussianStress.A => false,
                RussianStress.B => true,
                RussianStress.C => plural,
                RussianStress.D => !plural,
                RussianStress.E => plural && normCase != RussianCase.Nominative,
                RussianStress.F => !plural || normCase != RussianCase.Nominative,
                RussianStress.Bp => plural || normCase != RussianCase.Instrumental,
                RussianStress.Dp => !plural && props.Case != RussianCase.Accusative,
                RussianStress.Fp => plural ? normCase != RussianCase.Nominative : props.Case != RussianCase.Accusative,
                RussianStress.Fpp => plural ? normCase != RussianCase.Nominative : normCase != RussianCase.Instrumental,
                _ => throw new InvalidOperationException($"{decl.StressPattern} is not a valid stress pattern for nouns."),
            };
        }

    }
}
