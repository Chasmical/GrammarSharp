using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianNounInfo
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianNounInfo info)
        {
            SpanParser parser = new SpanParser(text);
            return ParseInternal(ref parser, out info);
        }
        [Pure] internal static ParseCode ParseInternal(ref SpanParser parser, out RussianNounInfo info)
        {
            info = default;

            // Parse the noun's primary properties
            var code = RussianNounProperties.ParseInternal(ref parser, out var properties);
            if (code > ParseCode.Leftovers) return code;
            parser.SkipWhitespaces();

            RussianDeclension declension;

            bool hasEnteredDeclensionBraces = parser.Skip('<');
            if (hasEnteredDeclensionBraces)
            {
                // Parse full declension, allowing a different type (but not pro-adj) and special properties
                code = RussianDeclension.ParseInternal(ref parser, out declension);
                if (code > ParseCode.Leftovers) return code;

                if (declension.Type > RussianDeclensionType.Pronoun)
                    return ParseCode.InvalidDeclension;
            }
            else
            {
                // Parse simple declension of noun type and without special properties
                code = RussianDeclension.ParseInternal(ref parser, out declension);
                if (code > ParseCode.Leftovers) return code;

                if (declension.Type != RussianDeclensionType.Noun || declension.AsNounUnsafeRef().SpecialProperties is not null)
                    return ParseCode.InvalidDeclension;
            }

            // Parse the singulare tantum indicator
            if (parser.SkipAny('-', '–', '—'))
            {
                if (properties.IsTantum) return ParseCode.InvalidTantums;
                properties.IsSingulareTantum = true;
            }

            // Ensure the declension braces were closed properly
            if (hasEnteredDeclensionBraces && !parser.Skip('>'))
                return ParseCode.UnclosedBraces;

            info = new(properties, declension);
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
        }

        [Pure] public static RussianNounInfo Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianNounInfo Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianNounInfo info).ReturnOrThrow(info, nameof(text));

        [Pure] public static bool TryParse(string? text, out RussianNounInfo info)
        {
            if (text is null) return Util.Fail(out info);
            return TryParse(text.AsSpan(), out info);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounInfo info)
            => ParseInternal(text, out info) is ParseCode.Success;

    }
}
