using System;
using System.Text;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianNumeral
    {
        public static string CreateCardinal(int number, RussianCase @case, RussianNoun noun)
        {
            StringBuilder sb = new();

            // Larger numbers ending with 2, 3 or 4 "lose animacy";
            // "Proper" animate accusative is sometimes used colloquially.
            // TODO: move deeper to final unit's resolution
            //if (@case == RussianCase.Accusative && number > 4 && number % 10 is 2 or 3 or 4)
            //    @case = RussianCase.Nominative;

            RussianNounProperties props = noun.Info.Properties;
            props.PrepareForAdjectiveDeclension(@case, noun.Info.Properties.IsPluraleTantum);

            var nounForm = DeclineAppendCardinal(sb, props, number);
            sb.Append(' ');

            sb.Append(nounForm switch
            {
                Number.DeclineSingular => noun.Decline(@case, false),
                Number.DeclinePlural => noun.Decline(@case, true),
                Number.PaucalCountForm => noun.Decline(RussianCase.Genitive, false),
                Number.PluralCountForm => noun.Decline(RussianCase.Genitive, true),
            });

            return sb.ToString();
        }

        private static Number DeclineAppendCardinal(StringBuilder sb, RussianNounProperties props, int number)
        {
            if (number <= 0)
            {
                if (number == 0)
                {
                    DeclineAppendZero(sb, props);
                    return Number.PluralCountForm;
                }
                sb.Append("минус ");
                number = -number;
            }

            bool isBigEnoughToLoseAnimacy = number > 4;

            // TODO: handle larger numbers

            if (number >= 1000)
            {
                int thousands = Math.DivRem(number, 1000, out number);

                RussianNounProperties thousandProps = props.WithGenderInanimate(RussianGender.Feminine);
                var thousandForm = DeclineAppendBetween1And999(sb, thousandProps, thousands, true);

                DeclineAppendThousandWord(sb, props, thousandForm);
            }

            if (number > 0)
            {
                return DeclineAppendBetween1And999(sb, props, number, isBigEnoughToLoseAnimacy);
            }
            return Number.PluralCountForm;
        }

        private static void DeclineAppendThousandWord(StringBuilder sb, RussianNounProperties props, Number number)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append("тысяч");

            switch (props.Case)
            {
                case RussianCase.Nominative:
                    switch (number)
                    {
                        case Number.DeclineSingular:
                            sb.Append('а');
                            break;
                        case Number.PaucalCountForm:
                            sb.Append('и');
                            break;
                    }
                    break;

                case RussianCase.Genitive:
                    if (number == Number.DeclineSingular)
                        sb.Append('и');
                    break;

                case RussianCase.Dative:
                    if (number == Number.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('м');
                    break;

                case RussianCase.Accusative:
                    switch (number)
                    {
                        case Number.DeclineSingular:
                            sb.Append('у');
                            break;
                        case Number.PaucalCountForm:
                            sb.Append('и');
                            break;
                    }
                    break;

                case RussianCase.Instrumental:
                    if (number == Number.DeclineSingular)
                        sb.Append('ь').Append('ю');
                    else
                        sb.Append('а').Append('м').Append('и');
                    break;

                default: // case RussianCase.Prepositional:
                    if (number == Number.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('х');
                    break;
            }
        }

        private static Number DeclineAppendBetween1And999(StringBuilder sb, RussianNounProperties props, int number, bool isBigEnoughToLoseAnimacy)
        {
            if (number >= 100)
            {
                if (sb.Length > 0) sb.Append(' ');
                int hundreds = Math.DivRem(number, 100, out number);

                if (hundreds == 1)
                    DeclineAppendOneHundred(sb, props);
                else
                    DeclineAppendTwoToNineHundred(sb, props, hundreds);

                // Continue to resolve the remaining 0-99
            }
            if (number >= 20)
            {
                if (sb.Length > 0) sb.Append(' ');
                int tens = Math.DivRem(number, 10, out number);

                switch (tens)
                {
                    case 2 or 3:
                        DeclineAppendTwentyOrThirty(sb, props, tens);
                        break;
                    case 4:
                        DeclineAppendForty(sb, props);
                        break;
                    case 9:
                        DeclineAppendNinety(sb, props);
                        break;
                    default:
                        DeclineAppendFiftyToEighty(sb, props, tens);
                        break;
                }

                // Continue to resolve the remaining 0-9
            }

            if (number == 0) return props.Case == RussianCase.Nominative ? Number.PluralCountForm : Number.DeclinePlural;
            if (sb.Length > 0) sb.Append(' ');

            switch (number)
            {
                case 1:
                    DeclineAppendOne(sb, props);
                    return Number.DeclineSingular;
                case 2:
                    return DeclineAppendTwo(sb, props, isBigEnoughToLoseAnimacy);
                case 3 or 4:
                    return DeclineAppendThreeOrFour(sb, props, number, isBigEnoughToLoseAnimacy);
                case >= 5 and <= 10:
                    DeclineAppendFiveToTen(sb, props, number);
                    break;
                default: // case >= 11 and <= 19:
                    DeclineAppendElevenToNineteen(sb, props, number);
                    break;
            }

            if (props.Case == RussianCase.Nominative)
                return number >= 5 ? Number.PluralCountForm : Number.PaucalCountForm;

            return Number.DeclinePlural;
        }

        private static void DeclineAppendZero(StringBuilder sb, RussianNounProperties props)
        {
            sb.Append('н').Append(props.Case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'у').Append('л');

            switch (props.Case)
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
        private static Number DeclineAppendTwo(StringBuilder sb, RussianNounProperties props, bool isBigEnoughToLoseAnimacy)
        {
            sb.Append('д').Append('в');
            switch (props.Case)
            {
                case RussianCase.Nominative:
                    sb.Append(props.Gender is RussianGender.Feminine ? 'е' : 'а');
                    return Number.PaucalCountForm;
                case RussianCase.Genitive:
                    sb.Append('у').Append('х');
                    break;
                case RussianCase.Dative:
                    sb.Append('у').Append('м');
                    break;
                case RussianCase.Accusative:
                    if (!isBigEnoughToLoseAnimacy && props.IsAnimate)
                        goto case RussianCase.Genitive;
                    goto case RussianCase.Nominative;
                case RussianCase.Instrumental:
                    sb.Append('у').Append('м').Append('я');
                    break;
                default: // case RussianCase.Prepositional:
                    goto case RussianCase.Genitive;
            }
            return Number.DeclinePlural;
        }
        private static Number DeclineAppendThreeOrFour(StringBuilder sb, RussianNounProperties props, int number, bool isBigEnoughToLoseAnimacy)
        {
            if (number == 3)
                sb.Append('т').Append('р');
            else
                sb.Append("четыр");

            switch (props.Case)
            {
                case RussianCase.Nominative:
                    sb.Append(number == 3 ? 'и' : 'е');
                    return Number.PaucalCountForm;
                case RussianCase.Genitive:
                    sb.Append('ё').Append('х');
                    break;
                case RussianCase.Dative:
                    sb.Append('ё').Append('м');
                    break;
                case RussianCase.Accusative:
                    if (!isBigEnoughToLoseAnimacy && props.IsAnimate)
                        goto case RussianCase.Genitive;
                    goto case RussianCase.Nominative;
                case RussianCase.Instrumental:
                    sb.Append(number == 3 ? 'е' : 'ь').Append('м').Append('я');
                    break;
                default: // case RussianCase.Prepositional:
                    goto case RussianCase.Genitive;
            }
            return Number.DeclinePlural;
        }
        private static void DeclineAppendFiveToTen(StringBuilder sb, RussianNounProperties props, int number)
        {
            DeclineAppendDigitStem(sb, props.Case, number);
            DeclineAppendTensEnding(sb, props.Case);
        }
        private static void DeclineAppendElevenToNineteen(StringBuilder sb, RussianNounProperties props, int number)
        {
            DeclineAppendDigitStem(sb, default, number - 10);
            sb.Append("надцат");
            DeclineAppendTensEnding(sb, props.Case);
        }

        private static void DeclineAppendDigitStem(StringBuilder sb, RussianCase @case, int number)
        {
            sb.Append(number switch
            {
                1 => "один",
                2 => "две",
                3 => "три",
                4 => "четыр",
                5 => "пят",
                6 => "шест",
                7 => "сем",
                8 => @case is RussianCase.Nominative or RussianCase.Accusative ? "восем" : "восьм",
                _ => "девят",
            });
        }
        private static void DeclineAppendTensEnding(StringBuilder sb, RussianCase @case)
        {
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

        private static void DeclineAppendTwentyOrThirty(StringBuilder sb, RussianNounProperties props, int tens)
        {
            sb.Append(tens is 2 ? "два" : "три").Append("дцат");

            switch (props.Case)
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
        private static void DeclineAppendForty(StringBuilder sb, RussianNounProperties props)
        {
            sb.Append("сорок");
            if (props.Case is not RussianCase.Nominative and not RussianCase.Accusative)
                sb.Append('а');
        }
        private static void DeclineAppendFiftyToEighty(StringBuilder sb, RussianNounProperties props, int tens)
        {
            DeclineAppendFiveToTen(sb, props, tens / 10);
            DeclineAppendFiveToTen(sb, props, 10);

            // Remove trailing 'ь' in nominative of 'десять'
            if (props.Case is RussianCase.Nominative or RussianCase.Accusative)
                sb.Remove(sb.Length - 1, 1);
        }
        private static void DeclineAppendNinety(StringBuilder sb, RussianNounProperties props)
        {
            sb.Append("девяност");
            sb.Append(props.Case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
        }

        private static void DeclineAppendOneHundred(StringBuilder sb, RussianNounProperties props)
        {
            sb.Append('с').Append('т');
            sb.Append(props.Case is RussianCase.Nominative or RussianCase.Accusative ? 'о' : 'а');
        }
        private static void DeclineAppendTwoToNineHundred(StringBuilder sb, RussianNounProperties props, int hundreds)
        {
            switch (hundreds)
            {
                case 2:
                    DeclineAppendTwo(sb, new(RussianGender.Feminine, false), false);
                    break;
                case 3 or 4:
                    DeclineAppendThreeOrFour(sb, new(RussianGender.Masculine, false), hundreds, false);
                    break;
                default:
                    DeclineAppendFiveToTen(sb, props, hundreds);
                    break;
            }
            sb.Append('с');

            switch (props.Case)
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

        private enum Number
        {
            DeclineSingular,
            DeclinePlural,
            PaucalCountForm,
            PluralCountForm,
        }

        // All the numeral declensions for 1-900:
        //  1: мс-п 1*b (gender and count)
        //  2: два/две/двух (gender/animacy)
        //  3: три/трёх, четыре/четырёх (animacy)
        //  4: пять, шесть, семь, восемь, девять, десять
        //  5: -надцать
        //  6: -дцать
        //  7: сорок
        //  8: -десят
        //  9: девяносто/девяноста, сто/ста
        // 10: -сти/-ста/-сот

        //     И             Р              Д               В             И                 П
        // 1 тысяча,     1 тысячи,      1 тысяче,       1 тысячу,     1 тысячью,        1 тысяче
        // 2 тысячи,     2 тысяч,       2 тысячам,      2 тысячи,     2 тысячами,       2 тысячах
        // 5 тысяч,      5 тысяч,       5 тысячам,      5 тысяч,      5 тысячами,       5 тысячах
        //
        // 1 миллион,    1 миллиона,    1 миллиону,     1 миллион,    1 миллионом,      1 миллионе
        // 2 миллиона,   2 миллионов,   2 миллионам,    2 миллиона,   2 миллионами,     2 миллионах
        // 5 миллионов,  5 миллионов,   5 миллионам,    5 миллионов,  5 миллионами,     5 миллионах

        //     И             Р              Д               В             И                 П
        // один - мс-п 1*b
        // два/две,      двух,          двум,           два/две/двух, двумя,            двух
        // три,          трёх,          трём,           три/трёх,     тремя,            трёх
        // четыре,       четырёх,       четырём,        четыре[х]?,   четырьмя,         четырёх
        // пять,         пяти,          пяти,           пять,         пятью,            пяти
        // шесть,        шести,         шести,          шесть,        шестью,           шести
        // семь,         семи,          семи,           семь,         семью,            семи
        // восемь,       восьми,        восьми,         восемь,       восьмью,          восьми
        // девять,       девяти,        девяти,         девять,       девятью,          девяти

        //     И             Р              Д               В             И                 П
        // одиннадцать,  одиннадцати,   одиннадцати,    одиннадцать,  одиннадцатью,     одиннадцати
        // двенадцать,   двенадцати,    двенадцати,     двенадцать,   двенадцатью,      двенадцати
        // тринадцать,   тринадцати,    тринадцати,     тринадцать,   тринадцатью,      тринадцати
        // четырнадцать, четырнадцати,  четырнадцати,   четырнадцать, четырнадцатью,    четырнадцати
        // пятнадцать,   пятнадцати,    пятнадцати,     пятнадцать,   пятнадцатью,      пятнадцати
        // шестнадцать,  шестнадцати,   шестнадцати,    шестнадцать,  шестнадцатью,     шестнадцати
        // семнадцать,   семнадцати,    семнадцати,     семнадцать,   семнадцатью,      семнадцати
        // восемнадцать, восемнадцати,  восемнадцати,   восемнадцать, восемнадцатью,    восемнадцати
        // девятнадцать, девятнадцати,  девятнадцати,   девятнадцать, девятнадцатью,    девятнадцати

        //     И             Р              Д               В             И                 П
        // десять,       десяти,        десяти,         десять,       десятью,          десяти
        // двадцать,     двадцати,      двадцати,       двадцать,     двадцатью,        двадцати
        // тридцать,     тридцати,      тридцати,       тридцать,     тридцатью,        тридцати
        // сорок,        сорока,        сорока,         сорок,        сорока,           сорока
        // пятьдесят,    пятидесяти,    пятидесяти,     пятьдесят,    пятьюдесятью,     пятидесяти
        // шестьдесят,   шестидесяти,   шестидесяти,    шестьдесят,   шестьюдесятью,    шестидесяти
        // семьдесят,    семидесяти,    семидесяти,     семьдесят,    семьюдесятью,     семидесяти
        // восемьдесят,  восьмидесяти,  восьмидесяти,   восемьдесят,  восьмьюдесятью,   восьмидесяти
        // девяносто,    девяноста,     девяноста,      девяносто,    девяноста,        девяноста

        //     И             Р              Д               В             И                 П
        // сто,          ста,           ста,            сто,          ста,              ста
        // двести,       двухсот,       двумстам,       двести,       двумястами,       двухстах
        // триста,       трёхсот,       трёмстам,       триста,       тремястами,       трёхстах
        // четыреста,    четырёхсот,    четырёмстам,    четыреста,    четырьмястами,    четырёхстах
        // пятьсот,      пятисот,       пятистам,       пятьсот,      пятьюстами,       пятистах
        // шестьсот,     шестисот,      шестистам,      шестьсот,     шестьюстами,      шестистах
        // семьсот,      семисот,       семистам,       семьсот,      семьюстами,       семистах
        // восемьсот,    восьмисот,     восьмистам,     восемьсот,    восьмьюстами,     восьмистах
        // девятьсот,    девятисот,     девятистам,     девятьсот,    девятьюстами,     девятистах

    }
}
