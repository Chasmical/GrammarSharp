using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianNounInfo
    {
        private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianNounInfo info)
        {
            info = default;
            SpanParser parser = new SpanParser(text);

            RussianGender gender;

            if (parser.Skip('с'))
                gender = RussianGender.Neuter;
            else if (parser.Skip('м'))
                gender = RussianGender.Masculine;
            else if (parser.Skip('ж'))
                gender = RussianGender.Feminine;
            else
                return ParseCode.GenderNotFound;

            bool animate = parser.Skip('о');

            if (animate && gender != RussianGender.Neuter)
            {
                if (parser.Skip('-', gender is RussianGender.Masculine ? 'ж' : 'м', 'о'))
                    gender = RussianGender.Common;
            }

            // TODO: declension-specific stuff, tantums, etc.

            info = new RussianNounInfo(gender, animate);

            return ParseCode.Success;
        }

        [Pure] public static RussianNounInfo Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianNounInfo Parse(ReadOnlySpan<char> text)
        {
            ParseCode code = ParseInternal(text, out RussianNounInfo info);
            if (code is ParseCode.Success) return info;
            throw new ArgumentException(code.ToString(), nameof(text));
        }

        [Pure] public static bool TryParse(string? text, out RussianNounInfo info)
        {
            if (text is null) return Util.Fail(out info);
            return TryParse(text.AsSpan(), out info);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounInfo info)
            => ParseInternal(text, out info) is ParseCode.Success;

        private enum ParseCode
        {
            Success,
            Unknown,
            GenderNotFound,
            AnimacyNotFound,
        }

    }
}
