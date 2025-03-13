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
            RussianGender gender;

            if (parser.Skip('м'))
            {
                gender = RussianGender.Masculine;

                if (parser.Skip('н'))
                {
                    parser.Skip('.');
                    properties = new(gender, false, RussianNounFlags.IsPluraleTantum);
                    return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
                }
            }
            else if (parser.Skip('ж'))
                gender = RussianGender.Feminine;
            else if (parser.Skip('с'))
                gender = RussianGender.Neuter;
            else
                return ParseCode.GenderNotFound;

            bool isAnimate = parser.Skip('о');

            if (isAnimate && gender != RussianGender.Neuter)
            {
                if (parser.Skip('-', gender is RussianGender.Masculine ? 'ж' : 'м', 'о'))
                    gender = RussianGender.Common;
            }

            properties = new(gender, isAnimate, 0);
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
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
