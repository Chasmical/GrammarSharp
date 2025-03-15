using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
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
            var code = RussianNounProperties.ParseInternal(ref parser, out RussianNounProperties properties);
            if (code > ParseCode.Leftovers) return code;
            parser.SkipWhitespaces();

            RussianDeclensionType declensionType = RussianDeclensionType.Noun;
            RussianNounProperties declensionProps = default;
            bool parsedDeclensionProps = false;

            // See if the noun has some special declension properties
            bool hasEnteredDeclensionBraces = parser.Skip('<');
            if (hasEnteredDeclensionBraces)
            {
                if (parser.Skip('п'))
                {
                    // A noun using adjective declension
                    declensionType = RussianDeclensionType.Adjective;
                }
                else
                {
                    // A noun using noun declension (but with different gender and/or animacy)
                    code = RussianNounProperties.ParseInternal(ref parser, out declensionProps);
                    if (code > ParseCode.Leftovers) return code;
                    parsedDeclensionProps = true;

                    // Declension properties cannot have tantums
                    if (declensionProps.IsTantum) return ParseCode.InvalidTantums;
                }
                parser.SkipWhitespaces();
            }

            // Parse the noun's declension type/class/info
            code = RussianDeclension.ParseInternal(ref parser, out RussianDeclension declension, declensionType);
            if (code > ParseCode.Leftovers) return code;

            // Parse the singulare tantum indicator
            if (parser.SkipAny('-', '–', '—'))
            {
                if (properties.IsTantum) return ParseCode.InvalidTantums;
                properties.IsSingulareTantum = true;
            }

            // Set the special declension properties
            if (parsedDeclensionProps)
            {
                // Pass on the main properties' tantums for correct count
                declensionProps.CopyTantumsFrom(properties);
                declension.SpecialNounProperties = declensionProps;
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
