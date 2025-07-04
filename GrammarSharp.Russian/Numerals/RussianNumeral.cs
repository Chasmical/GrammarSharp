using System;
using System.Text;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    using Agreement = RussianNumeralAgreement;

    public sealed partial class RussianNumeral
    {
        [Pure] public static string DeclineCardinalWithNoun(int number, RussianCase @case, RussianNoun noun)
        {
            StringBuilder sb = new();
            // Decline the cardinal numeral
            var agreement = DeclineCardinal(sb, number, @case, noun.Info.Properties);
            // Decline the noun in agreement
            sb.Append(' ').Append(noun.Decline(@case, agreement));
            // Build and return the string
            return sb.ToString();
        }

        [Pure] private static Agreement DeclineCardinal(StringBuilder sb, int number, RussianCase @case, RussianNounProperties props)
        {
            // Normalize "2nd" cases to the main 6 cases (translative is plural)
            bool plural = RussianGrammar.ValidateAndNormalizeCase(ref @case);
            // Prepare properties, set case and number
            props.PrepareForDeclensionGendersAndPlural(@case, plural);
            // Decline Int32 as a cardinal
            return RussianCardinal.DeclineInt32(sb, props, number);
        }

        [Pure] public static string DeclineOrdinal(int number, RussianCase @case, bool plural, RussianNounProperties props)
        {
            // Normalize "2nd" cases to the main 6 cases (translative is plural)
            RussianGrammar.ValidateAndNormalizeCase(ref @case, ref plural);
            // Prepare properties, set case and number
            props.PrepareForDeclensionGendersAndPlural(@case, plural);

            StringBuilder sb = new();
            RussianOrdinal.DeclineInt32(sb, props, number);
            return sb.ToString();
        }

    }
}
