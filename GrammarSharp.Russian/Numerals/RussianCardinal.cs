using System;
using System.Text;

namespace GrammarSharp.Russian
{
    using Agreement = RussianNumeralAgreement;

    internal sealed class RussianCardinal
    {
        internal static Agreement DeclineInt32(StringBuilder sb, RussianNounProperties props, int number)
        {
            if (number <= 0)
            {
                if (number == 0)
                {
                    DeclineZero(sb, props.Case);
                    return Agreement.PluralCountForm;
                }
                sb.Append("минус");
                number = -number;
            }
            int fullNumber = number;

            if (number >= (int)1e9)
                DeclineMillions(sb, Math.DivRem(number, (int)1e9, out number), "миллиард", props.Case);
            if (number >= (int)1e6)
                DeclineMillions(sb, Math.DivRem(number, (int)1e6, out number), "миллион", props.Case);
            if (number >= 1000)
                DeclineThousands(sb, Math.DivRem(number, 1000, out number), props.Case);

            if (number == 0) return Agreement.PluralCountForm;
            return DeclineBetween1And999(sb, props, number, fullNumber);
        }

        internal static Agreement GetAgreementSimple(int number, RussianCase @case, bool isAnimate)
        {
            // TODO: rework; doesn't handle 4 and 124 in accusative case properly

            int num = number % 1000;
            if (num == 0) return Agreement.PluralCountForm;

            if (num % 10 == 1 && num % 100 != 11)
                return Agreement.DeclineSingular;
            if (@case != RussianCase.Nominative)
                return Agreement.DeclinePlural;

            if (num % 10 is 2 or 3 or 4 && num % 100 is not (>= 12 and <= 14))
            {
                if (number <= 4 && isAnimate) return Agreement.DeclinePlural;
                return Agreement.PaucalCountForm;
            }
            return Agreement.PluralCountForm;
        }

        internal static void DeclineMillions(StringBuilder sb, int millions, string stem, RussianCase @case)
        {
            var agreement = DeclineBetween1And999(sb, new(RussianGender.Masculine, @case), millions, millions);
            sb.Append(' ').Append(stem);

            _ = (@case, agreement) switch
            {
                (RussianCase.Nominative or RussianCase.Accusative, Agreement.DeclinePlural) => sb.Append('ы'),
                (RussianCase.Nominative or RussianCase.Accusative, Agreement.PaucalCountForm) => sb.Append('а'),
                (RussianCase.Nominative or RussianCase.Accusative, Agreement.PluralCountForm) => sb.Append('о').Append('в'),
                (RussianCase.Genitive, Agreement.DeclineSingular) => sb.Append('а'),
                (RussianCase.Genitive, _) => sb.Append('о').Append('в'),
                (RussianCase.Dative, Agreement.DeclineSingular) => sb.Append('у'),
                (RussianCase.Dative, _) => sb.Append('а').Append('м'),
                (RussianCase.Instrumental, Agreement.DeclineSingular) => sb.Append('о').Append('м'),
                (RussianCase.Instrumental, _) => sb.Append('а').Append('м').Append('и'),
                (RussianCase.Prepositional, Agreement.DeclineSingular) => sb.Append('е'),
                (RussianCase.Prepositional, _) => sb.Append('а').Append('х'),
                (_, _) => sb,
            };
        }
        internal static void DeclineThousands(StringBuilder sb, int thousands, RussianCase @case)
        {
            var agreement = DeclineBetween1And999(sb, new(RussianGender.Feminine, @case), thousands, thousands);
            sb.Append(" тысяч");

            _ = (@case, agreement) switch
            {
                (RussianCase.Nominative, Agreement.DeclineSingular) => sb.Append('а'),
                (RussianCase.Nominative, Agreement.PaucalCountForm) => sb.Append('и'),
                (RussianCase.Genitive, Agreement.DeclineSingular) => sb.Append('и'),
                (RussianCase.Dative, Agreement.DeclineSingular) => sb.Append('е'),
                (RussianCase.Dative, _) => sb.Append('а').Append('м'),
                (RussianCase.Accusative, Agreement.DeclineSingular) => sb.Append('у'),
                (RussianCase.Accusative, Agreement.PaucalCountForm) => sb.Append('и'),
                (RussianCase.Instrumental, Agreement.DeclineSingular) => sb.Append('ь').Append('ю'),
                (RussianCase.Instrumental, _) => sb.Append('а').Append('м').Append('и'),
                (RussianCase.Prepositional, Agreement.DeclineSingular) => sb.Append('е'),
                (RussianCase.Prepositional, _) => sb.Append('а').Append('х'),
                (_, _) => sb,
            };
        }

        internal static Agreement DeclineBetween1And999(StringBuilder sb, RussianNounProperties props, int number, int fullNumber, bool addSpaces = true)
        {
            if (number >= 100)
            {
                if (addSpaces && sb.Length > 0) sb.Append(' ');
                int hundreds = Math.DivRem(number, 100, out number);
                // Append 100, 200, 300, 400, 500, 600, 700, 800, or 900
                DeclineHundredToNineHundred(sb, props.Case, hundreds);
                // Continue to resolve the remaining 0-99
            }
            if (number >= 20)
            {
                if (addSpaces && sb.Length > 0) sb.Append(' ');
                int tens = Math.DivRem(number, 10, out number);
                // Append 20, 30, 40, 50, 60, 70, 80, or 90
                DeclineTwentyToNinety(sb, props.Case, tens);
                // Continue to resolve the remaining 0-9
            }

            // If number is not zero, append 1-19
            if (number != 0)
            {
                if (addSpaces && sb.Length > 0) sb.Append(' ');

                switch (number)
                {
                    case 1:
                        // Decline "one" and the quantified noun together
                        RussianPronounDeclension decl = new(1, RussianStress.B, RussianDeclensionFlags.Star);
                        sb.Append(RussianPronoun.DeclineCore("один", decl, props));
                        return Agreement.DeclineSingular;

                    case 2 or 3 or 4:
                        // Decline "two"/"three"/"four" and determine the agreement inside the method
                        return DeclineTwoToFour(sb, props, number, fullNumber);

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

        private static void DeclineZero(StringBuilder sb, RussianCase @case)
        {
            sb.Append('н').Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'у').Append('л');

            _ = @case switch
            {
                RussianCase.Nominative or RussianCase.Accusative => sb.Append('ь'),
                RussianCase.Genitive => sb.Append('я'),
                RussianCase.Dative => sb.Append('ю'),
                RussianCase.Instrumental => sb.Append('ё').Append('м'),
                _ /* RussianCase.Prepositional */ => sb.Append('е'),
            };
        }
        private static Agreement DeclineTwoToFour(StringBuilder sb, RussianNounProperties props, int number, int fullNumber)
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
                    sb.Append(number switch { 2 => 'у', 3 => 'е', _ => 'ь' }).Append('м').Append('я');
                    break;

                default: // case RussianCase.Genitive or RussianCase.Prepositional:
                    sb.Append(number == 2 ? 'у' : 'ё').Append('х');
                    break;
            }
            return Agreement.DeclinePlural;
        }
        internal static void DeclineTwentyToNinety(StringBuilder sb, RussianCase @case, int tens)
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
        internal static void DeclineHundredToNineHundred(StringBuilder sb, RussianCase @case, int hundreds)
        {
            switch (hundreds)
            {
                case 1:
                    // Append 'сто'/'ста' and return
                    sb.Append('с').Append('т').Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
                    return;

                case 2 or 3 or 4:
                    // Append 'две', 'три' or 'четыреста', and continue
                    DeclineTwoToFour(sb, new(RussianGender.Feminine, @case), hundreds, hundreds);
                    break;
                default:
                    // Append 'пять'-'девять', and continue
                    AppendOneToTenStem(sb, @case, hundreds);
                    AppendVoicedSoftEnding(sb, @case);
                    break;
            }

            // Decline 'сто' according to the specified case
            sb.Append('с');
            switch (@case)
            {
                case RussianCase.Nominative or RussianCase.Accusative:
                    if (hundreds <= 4) // special: двести, триста, четыреста
                        sb.Append('т').Append(hundreds == 2 ? 'и' : 'а');
                    else // otherwise: пятьсот, шестьсот, семьсот, восемьсот, девятьсот
                        goto case RussianCase.Genitive;
                    break;
                case RussianCase.Genitive: // -сот
                    sb.Append('о').Append('т');
                    break;
                case RussianCase.Dative: // -стам
                    sb.Append('т').Append('а').Append('м');
                    break;
                case RussianCase.Instrumental: // -стами
                    sb.Append('т').Append('а').Append('м').Append('и');
                    break;
                case RussianCase.Prepositional: // -стах
                    sb.Append('т').Append('а').Append('х');
                    break;
            }
        }

        internal static void AppendOneToTenStem(StringBuilder sb, RussianCase @case, int number)
        {
            sb.Append(number switch
            {
                // 1/2/3/4 are nominative forms (not stems), and used only for "-дцать" and "-надцать"
                1 => "один",
                2 => "две",
                3 => "три",
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
            // Endings of type "ж 8b", for cardinals: 5-19, 20-30, 50-80.
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

    }
}
