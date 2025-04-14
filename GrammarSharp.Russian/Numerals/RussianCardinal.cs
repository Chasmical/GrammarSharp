using System;
using System.Text;

namespace GrammarSharp.Russian
{
    internal sealed class RussianCardinal
    {
        public static string CreateCardinal(int number, RussianCase @case, RussianNoun noun)
        {
            StringBuilder sb = new();

            RussianNounProperties props = noun.Info.Properties;
            bool plural = props.IsPluraleTantum;

            // Normalize "2nd" cases to the main 6 cases
            RussianGrammar.ValidateAndNormalizeCase(ref @case, ref plural);

            props.PrepareForDeclensionGenderCount(@case, plural);

            var agreement = AppendCardinalInt32(sb, props, number);
            AppendAgreeingNoun(sb, noun, @case, agreement);

            return sb.ToString();
        }

        private static Agreement AppendCardinalInt32(StringBuilder sb, RussianNounProperties props, int number)
        {
            if (number <= 0)
            {
                if (number == 0)
                {
                    AppendCardinalZero(sb, props.Case);
                    return Agreement.PluralCountForm;
                }
                sb.Append("минус ");
                number = -number;
            }
            int fullNumber = number;

            if (number >= (int)1e9)
            {
                int billions = Math.DivRem(number, (int)1e9, out number);
                AppendCardinalMillions(sb, billions, "миллиард", props.Case);
            }
            if (number >= (int)1e6)
            {
                int millions = Math.DivRem(number, (int)1e6, out number);
                AppendCardinalMillions(sb, millions, "миллион", props.Case);
            }
            if (number >= 1000)
            {
                int thousands = Math.DivRem(number, 1000, out number);
                AppendCardinalThousands(sb, thousands, props.Case);
            }

            if (number == 0)
            {
                return Agreement.PluralCountForm;
            }
            return AppendCardinalBetween1And999(sb, props, number, fullNumber);
        }

        private static void AppendCardinalMillions(StringBuilder sb, int millions, string stem, RussianCase @case)
        {
            var millionAgreement = AppendCardinalBetween1And999(sb, new(RussianGender.Masculine, @case), millions, millions);
            sb.Append(' ').Append(stem);
            AppendLlionWordEnding(sb, @case, millionAgreement);
        }
        private static void AppendCardinalThousands(StringBuilder sb, int thousands, RussianCase @case)
        {
            var thousandAgreement = AppendCardinalBetween1And999(sb, new(RussianGender.Feminine, @case), thousands, thousands);
            sb.Append(" тысяч");
            AppendThousandWordEnding(sb, @case, thousandAgreement);
        }
        private static void AppendAgreeingNoun(StringBuilder sb, RussianNoun noun, RussianCase @case, Agreement agreement)
        {
            sb.Append(' ').Append(agreement switch
            {
                Agreement.DeclineSingular => noun.Decline(@case, false),
                Agreement.DeclinePlural => noun.Decline(@case, true),
                Agreement.PaucalCountForm => noun.DeclineCountForm(false),
                _ => noun.DeclineCountForm(true),
            });
        }

        // TODO: turn these two huge word declension methods into lookups or something
        private static void AppendThousandWordEnding(StringBuilder sb, RussianCase @case, Agreement agreement)
        {
            switch (@case)
            {
                case RussianCase.Nominative:
                    switch (agreement)
                    {
                        case Agreement.DeclineSingular:
                            sb.Append('а');
                            break;
                        case Agreement.PaucalCountForm:
                            sb.Append('и');
                            break;
                    }
                    break;

                case RussianCase.Genitive:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('и');
                    break;

                case RussianCase.Dative:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('м');
                    break;

                case RussianCase.Accusative:
                    switch (agreement)
                    {
                        case Agreement.DeclineSingular:
                            sb.Append('у');
                            break;
                        case Agreement.PaucalCountForm:
                            sb.Append('и');
                            break;
                    }
                    break;

                case RussianCase.Instrumental:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('ь').Append('ю');
                    else
                        sb.Append('а').Append('м').Append('и');
                    break;

                default: // case RussianCase.Prepositional:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('х');
                    break;
            }
        }
        private static void AppendLlionWordEnding(StringBuilder sb, RussianCase @case, Agreement agreement)
        {
            switch (@case)
            {
                case RussianCase.Nominative or RussianCase.Accusative:
                    switch (agreement)
                    {
                        case Agreement.DeclinePlural:
                            sb.Append('ы');
                            break;
                        case Agreement.PaucalCountForm:
                            sb.Append('а');
                            break;
                        case Agreement.PluralCountForm:
                            sb.Append('о').Append('в');
                            break;
                    }
                    break;

                case RussianCase.Genitive:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('а');
                    else
                        sb.Append('о').Append('в');
                    break;

                case RussianCase.Dative:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('у');
                    else
                        sb.Append('а').Append('м');
                    break;

                case RussianCase.Instrumental:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('о').Append('м');
                    else
                        sb.Append('а').Append('м').Append('и');
                    break;

                default: // case RussianCase.Prepositional:
                    if (agreement == Agreement.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('х');
                    break;
            }
        }

        private static Agreement AppendCardinalBetween1And999(StringBuilder sb, RussianNounProperties props, int number, int fullNumber)
        {
            if (number >= 100)
            {
                if (sb.Length > 0) sb.Append(' ');
                int hundreds = Math.DivRem(number, 100, out number);
                // Append 100, 200, 300, 400, 500, 600, 700, 800, or 900
                AppendCardinalHundredToNineHundred(sb, props.Case, hundreds);
                // Continue to resolve the remaining 0-99
            }
            if (number >= 20)
            {
                if (sb.Length > 0) sb.Append(' ');
                int tens = Math.DivRem(number, 10, out number);
                // Append 20, 30, 40, 50, 60, 70, 80, or 90
                AppendCardinalTwentyToNinety(sb, props.Case, tens);
                // Continue to resolve the remaining 0-9
            }

            // If number is not zero, append 1-19
            if (number != 0)
            {
                if (sb.Length > 0) sb.Append(' ');

                switch (number)
                {
                    case 1:
                        // Decline "one" and the quantified noun together
                        AppendCardinalOne(sb, props);
                        return Agreement.DeclineSingular;

                    case 2 or 3 or 4:
                        // Decline "two"/"three"/"four" and determine the agreement inside the method
                        return AppendCardinalTwoToFour(sb, props, number, fullNumber);

                    case >= 5 and <= 10:
                        // Append 5-10 stem, and a voiced soft ending that 5-10 have
                        AppendOneToTenStem(sb, props.Case, number);
                        AppendVoicedSoftEnding(sb, props.Case);
                        break;

                    default: // case >= 11 and <= 19:
                        // Append 1-9 stem, and "надцат" with a voiced soft ending
                        AppendOneToTenStem(sb, RussianCase.Nominative, number - 10);
                        sb.Append("надцат");
                        AppendVoicedSoftEnding(sb, props.Case);
                        break;
                }
            }
            // "пять грамм" (nominative, plural count form), but "пятью граммами" (instrumental)
            return props.Case == RussianCase.Nominative ? Agreement.PluralCountForm : Agreement.DeclinePlural;
        }

        private static void AppendCardinalZero(StringBuilder sb, RussianCase @case)
        {
            sb.Append('н').Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'у').Append('л');

            switch (@case)
            {
                case RussianCase.Nominative or RussianCase.Accusative:
                    sb.Append('ь');
                    break;
                case RussianCase.Genitive:
                    sb.Append('я');
                    break;
                case RussianCase.Dative:
                    sb.Append('ю');
                    break;
                case RussianCase.Instrumental:
                    sb.Append('ё').Append('м');
                    break;
                default: // case RussianCase.Prepositional:
                    sb.Append('е');
                    break;
            }
        }
        private static void AppendCardinalOne(StringBuilder sb, RussianNounProperties props)
        {
            RussianPronounDeclension decl = new(1, RussianStress.B, RussianDeclensionFlags.Star);
            sb.Append(RussianPronoun.DeclineCore("один", decl, props));
        }
        private static Agreement AppendCardinalTwoToFour(StringBuilder sb, RussianNounProperties props, int number, int fullNumber)
        {
            _ = number switch
            {
                2 => sb.Append('д').Append('в'),
                3 => sb.Append('т').Append('р'),
                _ => sb.Append("четыр"),
            };

            switch (props.Case)
            {
                case RussianCase.Nominative:
                    sb.Append(number switch
                    {
                        2 => props.Gender == RussianGender.Feminine ? 'е' : 'а',
                        3 => 'и',
                        _ => 'е',
                    });
                    return Agreement.PaucalCountForm;

                case RussianCase.Dative:
                    sb.Append(number == 2 ? 'у' : 'ё').Append('м');
                    break;

                case RussianCase.Accusative:
                    // Larger numbers ending with 2, 3 or 4 "lose animacy" in accusative case;
                    // "Proper" animate accusative is sometimes used colloquially though.
                    if (fullNumber <= 4 && props.IsAnimate) goto default;
                    goto case RussianCase.Nominative;

                case RussianCase.Instrumental:
                    sb.Append(number switch { 2 => 'у', 3 => 'е', _ => 'ь' });
                    sb.Append('м').Append('я');
                    break;

                default: // case RussianCase.Genitive or RussianCase.Prepositional:
                    sb.Append(number == 2 ? 'у' : 'ё').Append('х');
                    break;
            }
            return Agreement.DeclinePlural;
        }

        private static void AppendCardinalTwentyToNinety(StringBuilder sb, RussianCase @case, int tens)
        {
            switch (tens)
            {
                case 2:
                    sb.Append("двадцат");
                    AppendVoicedSoftEnding(sb, @case);
                    break;
                case 3:
                    sb.Append("тридцат");
                    AppendVoicedSoftEnding(sb, @case);
                    break;
                case 4:
                    sb.Append("сорок");
                    if (@case is not RussianCase.Nominative and not RussianCase.Accusative)
                        sb.Append('а');
                    break;
                case >= 5 and <= 8:
                    AppendOneToTenStem(sb, @case, tens);
                    AppendVoicedSoftEnding(sb, @case);
                    sb.Append("десят");
                    AppendVoicedSoftEnding(sb, @case);

                    // Remove trailing 'ь' in nominative of 'десять'
                    if (@case is RussianCase.Nominative or RussianCase.Accusative)
                        sb.Remove(sb.Length - 1, 1);
                    break;
                default:
                    sb.Append("девяност");
                    sb.Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
                    break;
            }
        }
        private static void AppendCardinalHundredToNineHundred(StringBuilder sb, RussianCase @case, int hundreds)
        {
            switch (hundreds)
            {
                case 1:
                    sb.Append('с').Append('т').Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
                    return;
                case 2 or 3 or 4:
                    AppendCardinalTwoToFour(sb, new(RussianGender.Feminine, @case), hundreds, hundreds);
                    break;
                default:
                    AppendOneToTenStem(sb, @case, hundreds);
                    AppendVoicedSoftEnding(sb, @case);
                    break;
            }
            sb.Append('с');

            switch (@case)
            {
                default: // case RussianCase.Nominative or RussianCase.Accusative:
                    if (hundreds <= 4)
                        sb.Append('т').Append(hundreds == 2 ? 'и' : 'а');
                    else
                        sb.Append('о').Append('т');
                    break;
                case RussianCase.Genitive:
                    sb.Append('о').Append('т');
                    break;
                case RussianCase.Dative:
                    sb.Append("там");
                    break;
                case RussianCase.Instrumental:
                    sb.Append("тами");
                    break;
                case RussianCase.Prepositional:
                    sb.Append("тах");
                    break;
            }
        }

        private static void AppendOneToTenStem(StringBuilder sb, RussianCase @case, int number)
        {
            sb.Append(number switch
            {
                1 => "один",
                2 => "две", // "две" and "три" aren't stems in a classic sense, but just nominative forms,
                3 => "три", // but whatever, these forms are used only in the "-надцать" construction.
                4 => "четыр",
                5 => "пят",
                6 => "шест",
                7 => "сем",
                8 => @case is RussianCase.Nominative or RussianCase.Accusative ? "восем" : "восьм",
                9 => "девят",
                _ => "десят",
            });
        }
        private static void AppendVoicedSoftEnding(StringBuilder sb, RussianCase @case)
        {
            // Applicable for cardinals: 5-19, 20-30, 50-80.
            switch (@case)
            {
                case RussianCase.Nominative or RussianCase.Accusative:
                    sb.Append('ь');
                    break;
                case RussianCase.Instrumental:
                    sb.Append('ь').Append('ю');
                    break;
                default:
                    sb.Append('и');
                    break;
            }
        }

        private enum Agreement
        {
            DeclineSingular,
            DeclinePlural,
            PaucalCountForm,
            PluralCountForm,
        }

    }
}
