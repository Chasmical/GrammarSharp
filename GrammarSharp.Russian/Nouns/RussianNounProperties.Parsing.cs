using System;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
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

        /// <summary>
        ///   <para>Converts the specified string representation of Russian noun properties to an equivalent <see cref="RussianNounProperties"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing Russian noun properties to convert.</param>
        /// <returns>The <see cref="RussianNounProperties"/> structure equivalent to the Russian noun properties specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian noun properties.</exception>
        [Pure] public static RussianNounProperties Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing Russian noun properties to an equivalent <see cref="RussianNounProperties"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian noun properties to convert.</param>
        /// <returns>The <see cref="RussianNounProperties"/> structure equivalent to the Russian noun properties specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not valid Russian noun properties.</exception>
        [Pure] public static RussianNounProperties Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianNounProperties props).ReturnOrThrow(props, nameof(text));

        /// <summary>
        ///   <para>Tries to convert the specified string representation of Russian noun properties to an equivalent <see cref="RussianNounProperties"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing Russian noun properties to convert.</param>
        /// <param name="properties">When this method returns, contains the <see cref="RussianNounProperties"/> structure equivalent to the Russian noun properties specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out RussianNounProperties properties)
        {
            if (text is null)
            {
                properties = default;
                return false;
            }
            return TryParse(text.AsSpan(), out properties);
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing Russian noun properties to an equivalent <see cref="RussianNounProperties"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing Russian noun properties to convert.</param>
        /// <param name="properties">When this method returns, contains the <see cref="RussianNounProperties"/> structure equivalent to the Russian noun properties specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianNounProperties properties)
            => ParseInternal(text, out properties) is ParseCode.Success;

    }
}
