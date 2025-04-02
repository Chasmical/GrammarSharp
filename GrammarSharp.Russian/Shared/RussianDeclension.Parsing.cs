using System;
using Chasm.Formatting;
using Chasm.Utilities;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianDeclension
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianDeclension declension)
        {
            SpanParser parser = new SpanParser(text);
            return ParseInternal(ref parser, out declension, RussianDeclensionType.Unknown);
        }
        [Pure] internal static ParseCode ParseInternal(ref SpanParser parser, out RussianDeclension declension, RussianDeclensionType type)
        {
            declension = default;

            if (!parser.OnAsciiDigit) return ParseCode.StemTypeNotFound;
            int stemType = parser.Read() - '0';

            if (stemType == 0) return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
            if (stemType > 8) return ParseCode.InvalidStemType;

            RussianDeclensionFlags flags = 0;

            if (parser.Skip('*')) flags |= RussianDeclensionFlags.Star;
            if (parser.Skip('°')) flags |= RussianDeclensionFlags.Circle;

            if (!parser.OnAsciiLetter) return ParseCode.StressNotFound;
            var code = RussianStressPattern.ParseInternal(ref parser, out var stressPattern);
            if (code > ParseCode.Leftovers) return ParseCode.InvalidStress;

            while (parser.Skip('('))
            {
                char read = parser.Read();
                if (!parser.Skip(')') || (uint)(read - '1') > '3' - '1')
                {
                    // undo skipping these digit and parenthesis, they're not a valid flag
                    parser.position -= 2;
                    break;
                }

                flags |= (RussianDeclensionFlags)((int)RussianDeclensionFlags.Circle << (read - '0'));
            }

            if (parser.Skip('①')) flags |= RussianDeclensionFlags.CircledOne;
            if (parser.Skip('②')) flags |= RussianDeclensionFlags.CircledTwo;
            if (parser.Skip('③')) flags |= RussianDeclensionFlags.CircledThree;

            if (parser.Skip(',', ' ', 'ё') || parser.Skip(' ', 'ё'))
                flags |= RussianDeclensionFlags.AlternatingYo;

            declension = new RussianDeclension(type, stemType, stressPattern, flags);
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a Russian declension to an equivalent <see cref="RussianDeclension"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing a Russian declension to convert.</param>
        /// <returns>The <see cref="RussianDeclension"/> structure equivalent to the Russian declension specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid Russian declension.</exception>
        [Pure] public static RussianDeclension Parse(string? text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a Russian declension to an equivalent <see cref="RussianDeclension"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a Russian declension to convert.</param>
        /// <returns>The <see cref="RussianDeclension"/> structure equivalent to the Russian declension specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid Russian declension.</exception>
        [Pure] public static RussianDeclension Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianDeclension declension).ReturnOrThrow(declension, nameof(text));

        /// <summary>
        ///   <para>Tries to convert the specified string representation of a Russian declension to an equivalent <see cref="RussianDeclension"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a Russian declension to convert.</param>
        /// <param name="declension">When this method returns, contains the <see cref="RussianDeclension"/> structure equivalent to the Russian declension specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(string? text, out RussianDeclension declension)
        {
            if (text is null) return Util.Fail(out declension);
            return TryParse(text.AsSpan(), out declension);
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a Russian declension to an equivalent <see cref="RussianDeclension"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a Russian declension to convert.</param>
        /// <param name="declension">When this method returns, contains the <see cref="RussianDeclension"/> structure equivalent to the Russian declension specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianDeclension declension)
            => ParseInternal(text, out declension) is ParseCode.Success;

    }
}
