using System;
using Chasm.Formatting;
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

        /// <summary>
        ///   <para>Converts the specified string representation of Russian adjective info to an equivalent <see cref="RussianAdjectiveInfo"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing Russian adjective info to convert.</param>
        /// <returns>The <see cref="RussianAdjectiveInfo"/> structure equivalent to the Russian adjective info specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian adjective info.</exception>
        [Pure] public static RussianAdjectiveInfo Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing Russian adjective info to an equivalent <see cref="RussianAdjectiveInfo"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian adjective info to convert.</param>
        /// <returns>The <see cref="RussianAdjectiveInfo"/> structure equivalent to the Russian adjective info specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian adjective info.</exception>
        [Pure] public static RussianAdjectiveInfo Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianAdjectiveInfo info).ReturnOrThrow(info, nameof(text));

        /// <summary>
        ///   <para>Tries to convert the specified string representation of Russian adjective info to an equivalent <see cref="RussianAdjectiveInfo"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing Russian adjective info to convert.</param>
        /// <param name="info">When this method returns, contains the <see cref="RussianAdjectiveInfo"/> structure equivalent to the Russian adjective info specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out RussianAdjectiveInfo info)
        {
            if (text is null)
            {
                info = default;
                return false;
            }
            return TryParse(text.AsSpan(), out info);
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing Russian adjective info to an equivalent <see cref="RussianAdjectiveInfo"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian adjective info to convert.</param>
        /// <param name="info">When this method returns, contains the <see cref="RussianAdjectiveInfo"/> structure equivalent to the Russian adjective info specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianAdjectiveInfo info)
            => ParseInternal(text, out info) is ParseCode.Success;

    }
}
