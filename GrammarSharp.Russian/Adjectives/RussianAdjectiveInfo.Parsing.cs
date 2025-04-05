using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
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
            RussianDeclensionType defaultDeclensionType = RussianDeclensionType.Adjective;
            RussianDeclension declension;
            RussianAdjectiveFlags flags = 0;

            if (parser.Skip('ч', 'и', 'с', 'л'))
            {
                flags = RussianAdjectiveFlags.IsNumeral;
                parser.Skip('.');
                if (!parser.Skip('-')) return ParseCode.InvalidDeclension;
            }
            else if (parser.Skip('м', 'с'))
            {
                flags = RussianAdjectiveFlags.IsPronoun;
                defaultDeclensionType = RussianDeclensionType.Pronoun;
                if (!parser.Skip('-')) return ParseCode.InvalidDeclension;
            }

            if (!parser.Skip('п')) return ParseCode.InvalidDeclension;
            parser.SkipWhitespaces();

            bool hasEnteredDeclensionBraces = parser.Skip('<');
            if (hasEnteredDeclensionBraces)
            {
                // Parse full declension, allowing a different type
                var code = RussianDeclension.ParseInternal(ref parser, out declension);
                if (code > ParseCode.Leftovers) return code;

                if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
                    return ParseCode.InvalidDeclension;
            }
            else
            {
                // Parse simple declension of a strictly adjective/pronoun type
                var code = RussianDeclension.ParseInternal(ref parser, out declension, defaultDeclensionType);
                if (code > ParseCode.Leftovers) return code;
            }

            if (hasEnteredDeclensionBraces)
            {
                // Ensure the declension braces were closed properly
                if (!parser.Skip('>')) return ParseCode.UnclosedBraces;
            }
            else
            {
                // Only "pure" adjectives have short form and comparative indicators

                // Parse the short form difficulty indicators
                switch (parser.Peek())
                {
                    case '-' or '–' or '—':
                        parser.Skip();
                        flags = RussianAdjectiveFlags.Minus;
                        break;
                    case 'X' or '✕':
                        parser.Skip();
                        flags = RussianAdjectiveFlags.Cross;
                        break;
                    case '⌧':
                        parser.Skip();
                        flags = RussianAdjectiveFlags.BoxedCross;
                        break;
                }
                // Parse the "no comparative form" indicator
                if (parser.Skip('~'))
                    flags |= RussianAdjectiveFlags.NoComparativeForm;
            }

            info = new(declension, flags);
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
