using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianNounProperties
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianNounProperties properties)
        {
            SpanParser parser = new SpanParser(text);
            return ParseInternal(ref parser, out properties);
        }
        [Pure] internal static ParseCode ParseInternal(ref SpanParser parser, out RussianNounProperties properties)
        {
            properties = default;
            RussianNounFlags flags = 0;

            var code = ParseSimpleGenderAndAnimacy(ref parser, out RussianGender gender, out bool isAnimate);
            if (code != ParseCode.Success) return code;

            if (gender == RussianGender.Masculine && !isAnimate && parser.Skip('н'))
            {
                parser.Skip('.');
                flags = RussianNounFlags.IsPluraleTantum;

                if (parser.Skip(' ', 'о', 'т', ' '))
                {
                    code = ParseSimpleGenderAndAnimacy(ref parser, out gender, out isAnimate);
                    if (code != ParseCode.Success) return code;
                }
                else
                    gender = RussianGender.Common;
            }

            properties = new(gender, isAnimate, flags);
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
        }

        [Pure] private static ParseCode ParseSimpleGenderAndAnimacy(ref SpanParser parser, out RussianGender gender, out bool isAnimate)
        {
            if (parser.Skip('м'))
                gender = RussianGender.Masculine;
            else if (parser.Skip('ж'))
                gender = RussianGender.Feminine;
            else if (parser.Skip('с'))
                gender = RussianGender.Neuter;
            else
            {
                gender = default;
                isAnimate = false;
                return ParseCode.GenderNotFound;
            }

            isAnimate = parser.Skip('о');
            if (gender != RussianGender.Neuter && parser.Skip('-'))
            {
                if (parser.Skip(gender is RussianGender.Masculine ? 'ж' : 'м'))
                {
                    gender = RussianGender.Common;
                    isAnimate |= parser.Skip('о');
                }
                else
                    parser.position--;
            }
            // Note: does not check if parser is done reading
            return ParseCode.Success;
        }

        [Pure] public static RussianNounProperties Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianNounProperties Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianNounProperties props).ReturnOrThrow(props, nameof(text));

        [Pure] public static bool TryParse(string? text, out RussianNounProperties properties)
        {
            if (text is null)
            {
                properties = default;
                return false;
            }
            return TryParse(text.AsSpan(), out properties);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounProperties properties)
            => ParseInternal(text, out properties) is ParseCode.Success;

    }
}
