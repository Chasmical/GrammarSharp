using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianAdjectiveInfo
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianAdjectiveInfo info)
        {
            SpanParser parser = new(text);
            return ParseInternal(ref parser, out info);
        }
        [Pure] private static ParseCode ParseInternal(ref SpanParser parser, out RussianAdjectiveInfo info)
        {
            info = default;
            if (!parser.Skip('п')) return ParseCode.InvalidDeclension;
            parser.SkipWhitespaces();

            RussianDeclensionType declensionType = RussianDeclensionType.Adjective;

            // See if the adjective uses a different declension
            bool hasEnteredDeclensionBraces = parser.Skip('<');
            if (hasEnteredDeclensionBraces)
            {
                if (parser.Skip('п'))
                {
                    // An adjective using adjective declension (???)
                    declensionType = RussianDeclensionType.Adjective;
                }
                else if (parser.Skip('м', 'с'))
                {
                    // An adjective using pronoun declension
                    declensionType = RussianDeclensionType.Pronoun;
                }
                parser.SkipWhitespaces();
            }

            // Parse the adjective's declension type/class/info
            var code = RussianDeclension.ParseInternal(ref parser, out RussianDeclension declension, declensionType);
            if (code > ParseCode.Leftovers) return code;

            // Ensure the declension braces were closed properly
            if (hasEnteredDeclensionBraces && !parser.Skip('>'))
                return ParseCode.UnclosedBraces;

            info = new(declension);
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
        }

        [Pure] public static RussianAdjectiveInfo Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianAdjectiveInfo Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianAdjectiveInfo info).ReturnOrThrow(info, nameof(text));

        [Pure] public static bool TryParse(string? text, out RussianAdjectiveInfo info)
        {
            if (text is null) return Util.Fail(out info);
            return TryParse(text.AsSpan(), out info);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianAdjectiveInfo info)
            => ParseInternal(text, out info) is ParseCode.Success;

    }
}
