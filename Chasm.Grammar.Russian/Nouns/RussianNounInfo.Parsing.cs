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

            var code = RussianNounProperties.ParseInternal(ref parser, out RussianNounProperties properties);
            if (code > ParseCode.Leftovers) return ParseCode.InvalidProperties;

            RussianDeclensionType declensionType = RussianDeclensionType.Noun;
            RussianNounProperties declensionProps = default;
            bool parsedDeclensionProps = false;

            parser.SkipWhitespaces();
            bool hasEnteredDeclensionBraces = parser.Skip('<');

            if (hasEnteredDeclensionBraces)
            {
                if (parser.Skip('п'))
                {
                    // adjective declension
                    declensionType = RussianDeclensionType.Adjective;
                }
                else if (parser.Skip('м', 'с'))
                {
                    // pronoun declension
                    declensionType = RussianDeclensionType.Pronoun;
                }
                else
                {
                    // noun declension (different declension gender or animacy)
                    code = RussianNounProperties.ParseInternal(ref parser, out declensionProps);
                    if (code > ParseCode.Leftovers) return ParseCode.InvalidProperties;
                    parsedDeclensionProps = true;

                    // Declension properties cannot have tantums (?)
                    if (declensionProps.IsTantum) throw new NotImplementedException();
                    // Pass on the main properties' plurale tantum
                    if (properties.IsPluraleTantum) declensionProps.IsPluraleTantum = true;
                }

                parser.SkipWhitespaces();
            }

            code = RussianDeclension.ParseInternal(ref parser, out RussianDeclension declension);
            if (code > ParseCode.Leftovers) return ParseCode.InvalidDeclension;

            declension.Type = declensionType;
            if (parsedDeclensionProps) declension.SpecialNounProperties = declensionProps;

            info = new(properties, declension);
            return ParseCode.Success;
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
