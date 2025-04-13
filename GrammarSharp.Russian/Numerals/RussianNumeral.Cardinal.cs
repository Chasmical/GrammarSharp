using System;
using System.Text;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianNumeral
    {
        public static string CreateCardinal(int number, RussianCase @case, RussianNoun noun)
        {
            StringBuilder sb = new();

            RussianNounProperties props = noun.Info.Properties;
            bool plural = props.IsPluraleTantum;

            // Normalize "2nd" cases to the main 6 cases
            RussianGrammar.ValidateAndNormalizeCase(ref @case, ref plural);

            props.PrepareForDeclensionGenderCount(@case, plural);

            var nounForm = AppendCardinal(sb, props, number);
            sb.Append(' ');

            sb.Append(
                nounForm <= Agreement.DeclinePlural
                    ? noun.Decline(@case, nounForm == Agreement.DeclinePlural)
                    : noun.DeclineCountForm(nounForm == Agreement.PluralCountForm)
            );

            return sb.ToString();
        }

        // TODO: generalize this method to make it work for multiple numeric types
        private static Agreement AppendCardinal(StringBuilder sb, RussianNounProperties props, int number)
        {
            if (number <= 0)
            {
                if (number == 0)
                {
                    DeclineAppendZero(sb, props.Case);
                    return Agreement.PluralCountForm;
                }
                sb.Append("минус ");
                number = -number;
            }

            // Larger numbers ending with 2, 3 or 4 "lose animacy" in accusative case;
            // "Proper" animate accusative is sometimes used colloquially though.
            bool isSimple = number <= 4;

            if (number >= (int)1e9)
            {
                int billions = Math.DivRem(number, (int)1e9, out number);

                var billionProps = RussianNounProperties.WithGenderAndCase(RussianGender.Masculine, props.Case);
                var billionAgreement = DeclineAppendBetween1And999(sb, billionProps, billions, false);

                if (sb.Length > 0) sb.Append(' ');
                DeclineAppendLlionWord(sb, "миллиард", props.Case, billionAgreement);
            }

            if (number >= (int)1e6)
            {
                int millions = Math.DivRem(number, (int)1e6, out number);

                var millionProps = RussianNounProperties.WithGenderAndCase(RussianGender.Masculine, props.Case);
                var millionAgreement = DeclineAppendBetween1And999(sb, millionProps, millions, false);

                if (sb.Length > 0) sb.Append(' ');
                DeclineAppendLlionWord(sb, "миллион", props.Case, millionAgreement);
            }

            if (number >= 1000)
            {
                int thousands = Math.DivRem(number, 1000, out number);

                var thousandProps = RussianNounProperties.WithGenderAndCase(RussianGender.Feminine, props.Case);
                var thousandAgreement = DeclineAppendBetween1And999(sb, thousandProps, thousands, false);

                if (sb.Length > 0) sb.Append(' ');
                DeclineAppendThousandWord(sb, props.Case, thousandAgreement);
            }

            if (number > 0)
            {
                return DeclineAppendBetween1And999(sb, props, number, isSimple);
            }
            return Agreement.PluralCountForm;
        }

        // TODO: turn these two huge word declension methods into lookups or something
        private static void DeclineAppendThousandWord(StringBuilder sb, RussianCase @case, Agreement agreement)
        {
            sb.Append("тысяч");

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
        private static void DeclineAppendLlionWord(StringBuilder sb, string stem, RussianCase @case, Agreement agreement)
        {
            sb.Append(stem);

            switch (@case)
            {
                case RussianCase.Nominative or RussianCase.Accusative:
                    switch (agreement)
                    {
                        case Agreement.DeclineSingular:
                            break;
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

        private static Agreement DeclineAppendBetween1And999(StringBuilder sb, RussianNounProperties props, int number, bool isSimple)
        {
            if (number >= 100)
            {
                if (sb.Length > 0) sb.Append(' ');
                int hundreds = Math.DivRem(number, 100, out number);

                // Append 100, 200, 300, 400, 500, 600, 700, 800, or 900
                DeclineAppendOneToNineHundred(sb, props.Case, hundreds);

                // Continue to resolve the remaining 0-99
            }
            if (number >= 20)
            {
                if (sb.Length > 0) sb.Append(' ');
                int tens = Math.DivRem(number, 10, out number);

                // Append 20, 30, 40, 50, 60, 70, 80, or 90
                DeclineAppendTwentyToNinety(sb, props.Case, tens);

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
                        DeclineAppendOne(sb, props);
                        return Agreement.DeclineSingular;

                    case 2 or 3 or 4:
                        // Decline "two"/"three"/"four" and determine the agreement inside the method
                        return DeclineAppendTwoToFour(sb, props, number, isSimple);

                    case >= 5 and <= 10:
                        // Append 5-10 stem, and a voiced soft ending that 5-10 have
                        DeclineAppendOneToTenStem(sb, props.Case, number);
                        DeclineAppendVoicedSoftEnding(sb, props.Case);
                        break;

                    default: // case >= 11 and <= 19:
                        // Append 1-9 stem, and "надцат" with a voiced soft ending
                        DeclineAppendOneToTenStem(sb, RussianCase.Nominative, number - 10);
                        sb.Append("надцат");
                        DeclineAppendVoicedSoftEnding(sb, props.Case);
                        break;
                }
            }
            // "пять грамм" (nominative, plural count form), but "пятью граммами" (instrumental)
            return props.Case == RussianCase.Nominative ? Agreement.PluralCountForm : Agreement.DeclinePlural;

        }

        private static void DeclineAppendZero(StringBuilder sb, RussianCase @case)
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
        private static void DeclineAppendOne(StringBuilder sb, RussianNounProperties props)
        {
            RussianPronounDeclension decl = new(1, RussianStress.B, RussianDeclensionFlags.Star);
            sb.Append(RussianPronoun.DeclineCore("один", decl, props));
        }
        private static Agreement DeclineAppendTwoToFour(StringBuilder sb, RussianNounProperties props, int number, bool isSimple)
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
                    if (isSimple && props.IsAnimate) goto default;
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

        private static void DeclineAppendTwentyToNinety(StringBuilder sb, RussianCase @case, int tens)
        {
            switch (tens)
            {
                case 2:
                    sb.Append("двадцат");
                    DeclineAppendVoicedSoftEnding(sb, @case);
                    break;
                case 3:
                    sb.Append("тридцат");
                    DeclineAppendVoicedSoftEnding(sb, @case);
                    break;
                case 4:
                    sb.Append("сорок");
                    if (@case is not RussianCase.Nominative and not RussianCase.Accusative)
                        sb.Append('а');
                    break;
                case >= 5 and <= 8:
                    DeclineAppendOneToTenStem(sb, @case, tens);
                    DeclineAppendVoicedSoftEnding(sb, @case);
                    sb.Append("десят");
                    DeclineAppendVoicedSoftEnding(sb, @case);

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
        private static void DeclineAppendOneToNineHundred(StringBuilder sb, RussianCase @case, int hundreds)
        {
            switch (hundreds)
            {
                case 1:
                    sb.Append('с').Append('т').Append(@case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
                    return;
                case 2 or 3 or 4:
                    DeclineAppendTwoToFour(sb, RussianNounProperties.WithGenderAndCase(RussianGender.Feminine, @case), hundreds, true);
                    break;
                default:
                    DeclineAppendOneToTenStem(sb, @case, hundreds);
                    DeclineAppendVoicedSoftEnding(sb, @case);
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

        private static void DeclineAppendOneToTenStem(StringBuilder sb, RussianCase @case, int number)
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
        private static void DeclineAppendVoicedSoftEnding(StringBuilder sb, RussianCase @case)
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
