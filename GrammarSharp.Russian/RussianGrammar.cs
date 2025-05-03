using System.ComponentModel;

namespace GrammarSharp.Russian
{
    public static class RussianGrammar
    {
        internal static void ValidateAndNormalizeCase(ref RussianCase @case, ref bool plural, [CAE(nameof(@case))] string? paramName = null)
        {
            plural |= ValidateAndNormalizeCase(ref @case, paramName);
        }
        internal static bool ValidateAndNormalizeCase(ref RussianCase @case, [CAE(nameof(@case))] string? paramName = null)
        {
            if ((uint)@case > (uint)RussianCase.Prepositional)
            {
                switch (@case)
                {
                    case RussianCase.Partitive:
                        @case = RussianCase.Genitive;
                        break;
                    case RussianCase.Translative:
                        @case = RussianCase.Nominative;
                        return true;
                    case RussianCase.Locative:
                        @case = RussianCase.Prepositional;
                        break;
                    default:
                        Throw(@case, paramName);
                        break;
                }
            }
            return false;

            static void Throw(RussianCase @case, string? paramName)
                => throw new InvalidEnumArgumentException(paramName, (int)@case, typeof(RussianCase));
        }

    }
}
