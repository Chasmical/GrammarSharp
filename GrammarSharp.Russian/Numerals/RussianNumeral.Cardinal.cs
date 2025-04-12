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

            // TODO: handle larger numbers

            if (number >= 1000)
            {
                int thousands = Math.DivRem(number, 1000, out number);

                var thousandProps = RussianNounProperties.WithGenderAndCase(RussianGender.Feminine, props.Case);
                var thousandForm = DeclineAppendBetween1And999(sb, thousandProps, thousands, false);

                DeclineAppendThousandWord(sb, thousandProps, thousandForm);
            }

            if (number > 0)
            {
                return DeclineAppendBetween1And999(sb, props, number, isSimple);
            }
            return Agreement.PluralCountForm;
        }

        private static void DeclineAppendThousandWord(StringBuilder sb, RussianNounProperties props, Agreement number)
        {
            if (sb.Length > 0) sb.Append(' ');
            sb.Append("тысяч");

            switch (props.Case)
            {
                case RussianCase.Nominative:
                    switch (number)
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
                    if (number == Agreement.DeclineSingular)
                        sb.Append('и');
                    break;

                case RussianCase.Dative:
                    if (number == Agreement.DeclineSingular)
                        sb.Append('е');
                    else
                        sb.Append('а').Append('м');
                    break;

                case RussianCase.Accusative:
                    switch (number)
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
                    if (number == Agreement.DeclineSingular)
                        sb.Append('ь').Append('ю');
                    else
                        sb.Append('а').Append('м').Append('и');
                    break;

                default: // case RussianCase.Prepositional:
                    if (number == Agreement.DeclineSingular)
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

            if (number == 0) return props.Case == RussianCase.Nominative ? Agreement.PluralCountForm : Agreement.DeclinePlural;
            if (sb.Length > 0) sb.Append(' ');

            switch (number)
            {
                case 1:
                    DeclineAppendOne(sb, props);
                    return Agreement.DeclineSingular;
                case 2 or 3 or 4:
                    return DeclineAppendTwoToFour(sb, props, number, isSimple);
                default: // case >= 5 and <= 19:
                    DeclineAppendFiveToNineteen(sb, props.Case, number);
                    break;
            }

            if (props.Case == RussianCase.Nominative)
                return number >= 5 ? Agreement.PluralCountForm : Agreement.PaucalCountForm;

            return Agreement.DeclinePlural;
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

        private static void DeclineAppendFiveToNineteen(StringBuilder sb, RussianCase @case, int number)
        {
            if (number <= 10)
            {
                DeclineAppendDigitStem(sb, @case, number);
            }
            else
            {
                DeclineAppendDigitStem(sb, RussianCase.Nominative, number - 10);
                sb.Append("надцат");
            }
            DeclineAppendTensEnding(sb, @case);
        }
        private static void DeclineAppendTwentyToNinety(StringBuilder sb, RussianCase @case, int tens)
        {
            switch (tens)
            {
                case 2:
                    sb.Append("двадцат");
                    DeclineAppendTensEnding(sb, @case);
                    break;
                case 3:
                    sb.Append("тридцат");
                    DeclineAppendTensEnding(sb, @case);
                    break;
                case 4:
                    sb.Append("сорок");
                    if (@case is not RussianCase.Nominative and not RussianCase.Accusative)
                        sb.Append('а');
                    break;
                case >= 5 and <= 8:
                    DeclineAppendFiveToNineteen(sb, @case, tens / 10);
                    DeclineAppendFiveToNineteen(sb, @case, 10);

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
                    DeclineAppendFiveToNineteen(sb, @case, hundreds);
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

        private enum Agreement
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
