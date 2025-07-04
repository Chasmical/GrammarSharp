using System;
using System.Text;

namespace GrammarSharp.Russian
{
    internal sealed class RussianOrdinal
    {
        internal static void DeclineInt32(StringBuilder sb, RussianNounProperties props, int number)
        {
            if (number >= (int)1e9)
            {
                int billions = Math.DivRem(number, (int)1e9, out number);
                if (number == 0)
                {
                    // двухмиллиардный
                    DeclineThousands(sb, "миллиард", RussianGender.Masculine, billions);
                    DeclineEndingUnstressed1(sb, props);
                    return;
                }
                // два миллиарда [третий]
                RussianCardinal.DeclineMillions(sb, billions, "миллиард", RussianCase.Nominative);
                sb.Append(' ');
            }
            if (number >= (int)1e6)
            {
                int millions = Math.DivRem(number, (int)1e6, out number);
                if (number == 0)
                {
                    // двухмиллионный
                    DeclineThousands(sb, "миллион", RussianGender.Masculine, millions);
                    sb.Append('н');
                    DeclineEndingUnstressed1(sb, props);
                    return;
                }
                // два миллиона [сороковой]
                RussianCardinal.DeclineMillions(sb, millions, "миллион", RussianCase.Nominative);
                sb.Append(' ');
            }
            if (number >= 1000)
            {
                int thousands = Math.DivRem(number, 1000, out number);
                if (number == 0)
                {
                    // двухтысячный
                    DeclineThousands(sb, "тысяч", RussianGender.Feminine, thousands);
                    sb.Append('н');
                    DeclineEndingUnstressed1(sb, props);
                    return;
                }
                // две тысячи [пятый]
                RussianCardinal.DeclineThousands(sb, thousands, RussianCase.Nominative);
                sb.Append(' ');
            }
            if (number >= 100)
            {
                int hundreds = Math.DivRem(number, 100, out number);
                if (number == 0)
                {
                    // двухсотый
                    if (hundreds == 1)
                        sb.Append("сот");
                    else
                        RussianCardinal.DeclineHundredToNineHundred(sb, RussianCase.Genitive, hundreds);
                    DeclineEndingUnstressed1(sb, props);
                    return;
                }
                // двести [шестой]
                RussianCardinal.DeclineHundredToNineHundred(sb, RussianCase.Nominative, hundreds);
                sb.Append(' ');
            }
            if (number >= 20)
            {
                int tens = Math.DivRem(number, 10, out number);
                if (number == 0)
                {
                    // двадцатый
                    DeclineTwentiethToNinetiethWithEnding(sb, props, tens);
                    return;
                }
                // двадцать [третий]
                RussianCardinal.DeclineTwentyToNinety(sb, RussianCase.Nominative, tens);
                sb.Append(' ');
            }

            bool stressedEnding = false;

            switch (number)
            {
                case 1:
                    sb.Append("перв");
                    break;
                case 2:
                    sb.Append("втор");
                    stressedEnding = true;
                    break;
                case 3:
                    var decl2 = new RussianPronounDeclension(6, RussianStress.A, RussianDeclensionFlags.Star);
                    sb.Append(RussianPronoun.DeclineCore("трети", decl2, props));
                    return;
                case 4:
                    sb.Append("четвёрт");
                    break;
                case 5:
                    sb.Append("пят");
                    break;
                case 6:
                    sb.Append("шест");
                    stressedEnding = true;
                    break;
                case 7:
                    sb.Append("седьм");
                    stressedEnding = true;
                    break;
                case 8:
                    sb.Append("восьм");
                    stressedEnding = true;
                    break;
                case 9:
                    sb.Append("девят");
                    break;
                case 10:
                    sb.Append("десят");
                    break;

                default: // case >= 11 and <= 19:
                    // Append 1-9 stem, and "надцат"
                    RussianCardinal.AppendOneToTenStem(sb, RussianCase.Nominative, number - 10);
                    sb.Append("надцат");
                    break;
            }

            var decl = new RussianAdjectiveDeclension(1, default, 0);
            var (unStr, str) = RussianEndings.GetAdjectiveEndingIndices(decl, props);
            sb.Append(RussianEndings.Get(stressedEnding ? str : unStr));
        }

        private static void DeclineThousands(StringBuilder sb, string stem, RussianGender gender, int number)
        {
            if (number != 1)
            {
                // двухмиллион-ный
                // стадвадцатимиллион-ный
                // двухсоттридцатимиллион-ный
                // TODO: двухсоттридцатиодногомиллион-ный

                RussianCardinal.DeclineBetween1And999(sb, new(gender, RussianCase.Genitive), number, number, false);
            }
            sb.Append(stem);
        }
        private static void DeclineEndingUnstressed1(StringBuilder sb, RussianNounProperties props)
        {
            var decl = new RussianAdjectiveDeclension(1, default, 0);
            var index = RussianEndings.GetAdjectiveEndingIndices(decl, props).Item1;
            sb.Append(RussianEndings.Get(index));
        }

        private static void DeclineTwentiethToNinetiethWithEnding(StringBuilder sb, RussianNounProperties props, int tens)
        {
            sb.Append(tens switch
            {
                2 => "двадцат",
                3 => "тридцат",
                4 => "сороков",
                5 => "пятидесят",
                6 => "шестидесят",
                7 => "семидесят",
                8 => "восьмидесят",
                _ => "девяност",
            });

            var decl = new RussianAdjectiveDeclension(1, default, 0);
            var (unStr, str) = RussianEndings.GetAdjectiveEndingIndices(decl, props);
            sb.Append(RussianEndings.Get(tens == 4 ? str : unStr));
        }

    }
}
