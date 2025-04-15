using System;
using System.Text;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public sealed partial class RussianNumeral
    {
        public static string Create(int number, ReadOnlySpan<char> format, RussianNounInfo info, RussianCase @case)
        {
            char formatSpecifier = ParseFormatSpecifier(format, out bool withPlus, out int maxLongLength);

            StringBuilder sb = new();
            switch (formatSpecifier)
            {
                case 'G':
                    break;

                case 'g':
                    sb.Append(number);
                    var agreement = RussianCardinal.GetAgreementSimple(number, @case);

                    break;

                case 'C':
                    break;

                case 'O':
                    break;

                case 'o':
                    break;
            }

        }
        private static char ParseFormatSpecifier(ReadOnlySpan<char> format, out bool withPlus, out int maxLongLength)
        {
            // Parse leading plus
            withPlus = format.Length > 0 && format[0] == '+';
            if (withPlus) format = format[1..];

            // Parse format specifier, and long form's max length
            char formatSpecifier = format.Length > 0 ? format[0] : 'g';
            maxLongLength = format.Length > 1 ? int.Parse(format[1..]) : int.MaxValue;
            // TODO: check if maxLongLength is valid for this specifier

            return formatSpecifier;
        }

    }
}
