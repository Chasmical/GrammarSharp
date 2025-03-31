using System;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianStressPattern
    {
        [Pure] private static ParseCode ParseInternal(ReadOnlySpan<char> text, out RussianStressPattern pattern)
        {
            SpanParser parser = new(text);
            return ParseInternal(ref parser, out pattern);
        }
        [Pure] internal static ParseCode ParseInternal(ref SpanParser parser, out RussianStressPattern pattern)
        {
            pattern = default;

            // Try to parse the main stress schema
            ParseCode code = ParseSingleStress(ref parser, out RussianStress main);
            if (code > ParseCode.StressNotFound) return code;

            RussianStress alt = 0;
            if (parser.Skip('/'))
            {
                // Try to parse the alternative stress schema
                ParseCode code2 = ParseSingleStress(ref parser, out alt);
                if (code2 > ParseCode.StressNotFound) return code2;
            }
            // If nothing was parsed at all, return StressNotFound
            else if (code == ParseCode.StressNotFound) return code;

            pattern = new((byte)((int)main | ((int)alt << 4)));
            return parser.CanRead() ? ParseCode.Leftovers : ParseCode.Success;
        }

        [Pure] private static ParseCode ParseSingleStress(ref SpanParser parser, out RussianStress stress)
        {
            if (parser.TryPeek(out char first) && first is >= 'a' and <= 'f')
            {
                parser.Skip();
                // Read and parse the letter class
                stress = first - ('a' - RussianStress.A);

                bool skippedDoublePrime = false;

                // Skip single prime
                if (parser.SkipAny('\'', '′'))
                {
                    // If there's another one right after, treat it as a double prime
                    if (parser.SkipAny('\'', '′'))
                        skippedDoublePrime = true;
                    // Otherwise, add single prime to the stress
                    else
                        stress += 6;
                }
                // Skip double prime
                else if (parser.SkipAny('"', '″'))
                    skippedDoublePrime = true;

                if (skippedDoublePrime)
                {
                    // If a double prime was skipped, add double prime to the stress
                    switch (stress)
                    {
                        case RussianStress.C:
                            stress = RussianStress.Cpp;
                            break;
                        case RussianStress.F:
                            stress = RussianStress.Fpp;
                            break;
                        default:
                            return ParseCode.InvalidStressPrime;
                    }
                }
                // Note: does not check if parser is done reading
                return ParseCode.Success;
            }

            stress = 0;
            return ParseCode.StressNotFound;
        }

        /// <summary>
        ///   <para>Converts the specified string representation of a Russian stress pattern to an equivalent <see cref="RussianStressPattern"/> structure.</para>
        /// </summary>
        /// <param name="text">The string containing a Russian stress pattern to convert.</param>
        /// <returns>The <see cref="RussianStressPattern"/> structure equivalent to the Russian stress pattern specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid Russian stress pattern.</exception>
        [Pure] public static RussianStressPattern Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        /// <summary>
        ///   <para>Converts the specified read-only span of characters representing a Russian stress pattern to an equivalent <see cref="RussianStressPattern"/> structure.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a Russian stress pattern to convert.</param>
        /// <returns>The <see cref="RussianStressPattern"/> structure equivalent to the Russian stress pattern specified in the <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="text"/> is not a valid Russian stress pattern.</exception>
        [Pure] public static RussianStressPattern Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianStressPattern pattern).ReturnOrThrow(pattern, nameof(text));

        /// <summary>
        ///   <para>Tries to convert the specified string representation of a Russian stress pattern to an equivalent <see cref="RussianStressPattern"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The string containing a Russian stress pattern to convert.</param>
        /// <param name="pattern">When this method returns, contains the <see cref="RussianStressPattern"/> structure equivalent to the Russian stress pattern specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse([NotNullWhen(true)] string? text, out RussianStressPattern pattern)
        {
            if (text is null)
            {
                pattern = default;
                return false;
            }
            return TryParse(text.AsSpan(), out pattern);
        }
        /// <summary>
        ///   <para>Tries to convert the specified read-only span of characters representing a Russian stress pattern to an equivalent <see cref="RussianStressPattern"/> structure, and returns a value indicating whether the conversion was successful.</para>
        /// </summary>
        /// <param name="text">The read-only span of characters containing a Russian stress pattern to convert.</param>
        /// <param name="pattern">When this method returns, contains the <see cref="RussianStressPattern"/> structure equivalent to the Russian stress pattern specified in the <paramref name="text"/>, if the conversion succeeded, or <see langword="default"/> if the conversion failed.</param>
        /// <returns><see langword="true"/>, if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianStressPattern pattern)
            => ParseInternal(text, out pattern) is ParseCode.Success;

    }
}
