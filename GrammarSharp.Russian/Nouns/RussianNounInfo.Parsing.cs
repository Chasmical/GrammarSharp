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

        /// <summary>
        ///   <para>Converts the specified string representation of Russian noun info to an equivalent <see cref="RussianNounInfo"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing Russian noun info to convert.</param>
        /// <returns>The <see cref="RussianNounInfo"/> structure equivalent to the Russian noun info specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian noun info.</exception>
        [Pure] public static RussianNounInfo Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing Russian noun info to an equivalent <see cref="RussianNounInfo"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian noun info to convert.</param>
        /// <returns>The <see cref="RussianNounInfo"/> structure equivalent to the Russian noun info specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian noun info.</exception>
        [Pure] public static RussianNounInfo Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianNounInfo info).ReturnOrThrow(info, nameof(text));

        /// <summary>
        ///   <para>Tries to convert the specified string representation of Russian noun info to an equivalent <see cref="RussianNounInfo"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing Russian noun info to convert.</param>
        /// <param name="info">When this method returns, contains the <see cref="RussianNounInfo"/> structure equivalent to the Russian noun info specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out RussianNounInfo info)
        {
            if (text is null) return Util.Fail(out info);
            return TryParse(text.AsSpan(), out info);
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing Russian noun info to an equivalent <see cref="RussianNounInfo"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian noun info to convert.</param>
        /// <param name="info">When this method returns, contains the <see cref="RussianNounInfo"/> structure equivalent to the Russian noun info specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounInfo info)
            => ParseInternal(text, out info) is ParseCode.Success;

    }
}
