using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianNounDeclension
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianNounDeclension declension)
        {
            declension = default;
            SpanParser parser = new SpanParser(text);

            if (!parser.OnAsciiDigit) return ParseCode.DigitNotFound;
            int digit = parser.Read() - '0';

            if (digit == 0) return parser.CanRead() ? ParseCode.Unknown : ParseCode.Success;
            if (digit > 8) return ParseCode.InvalidDigit;

            RussianDeclensionFlags flags = 0;

            if (parser.Skip('*')) flags |= RussianDeclensionFlags.Star;
            if (parser.Skip('°')) flags |= RussianDeclensionFlags.Circle;

            if (!parser.OnAsciiLetter) return ParseCode.LetterNotFound;
            RussianDeclensionAccent accent = (RussianDeclensionAccent)((parser.Read() | ' ')  - '`');

            if (parser.SkipAny('\'', '′'))
                accent += 0b0111;
            else if (parser.SkipAny('"', '″'))
            {
                if (accent == RussianDeclensionAccent.C)
                    accent = RussianDeclensionAccent.Cpp;
                else if (accent == RussianDeclensionAccent.F)
                    accent = RussianDeclensionAccent.Fpp;
                else
                    return ParseCode.InvalidLetter;
            }

            while (parser.Skip('('))
            {
                if (!parser.OnAsciiDigit) return ParseCode.Unknown;
                int num = parser.Read() - '0';

                if ((uint)(num - 1) <= '3' - '1') return ParseCode.Unknown;
                if (!parser.Skip(')')) return ParseCode.Unknown;

                flags |= num switch
                {
                    1 => RussianDeclensionFlags.CircledOne,
                    2 => RussianDeclensionFlags.CircledTwo,
                    3 => RussianDeclensionFlags.CircledThree,
                };
            }

            if (parser.Skip('①')) flags |= RussianDeclensionFlags.CircledOne;
            if (parser.Skip('②')) flags |= RussianDeclensionFlags.CircledTwo;
            if (parser.Skip('③')) flags |= RussianDeclensionFlags.CircledThree;

            if (parser.Skip(',', ' ', 'ё') || parser.Skip(' ', 'ё'))
                flags |= RussianDeclensionFlags.AlternatingYo;

            declension = new RussianNounDeclension(digit, accent, flags);
            return ParseCode.Success;
        }

        [Pure] public static RussianNounDeclension Parse(string? text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianNounDeclension Parse(ReadOnlySpan<char> text)
        {
            ParseCode code = ParseInternal(text, out RussianNounDeclension declension);
            if (code is ParseCode.Success) return declension;
            throw new ArgumentException(code.ToString(), nameof(text));
        }

        [Pure] public static bool TryParse(string? text, out RussianNounDeclension declension)
        {
            if (text is null) return Util.Fail(out declension);
            return TryParse(text.AsSpan(), out declension);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounDeclension declension)
            => ParseInternal(text, out declension) is ParseCode.Success;

        private enum ParseCode
        {
            Success,
            Unknown,
            DigitNotFound,
            LetterNotFound,
            InvalidDigit,
            InvalidLetter,
        }
    }
}
