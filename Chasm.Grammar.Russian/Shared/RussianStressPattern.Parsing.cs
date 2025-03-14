using System;
using System.Diagnostics.CodeAnalysis;
using Chasm.Formatting;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
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

            ParseCode code = ParseSingleStress(ref parser, out RussianStress main);
            if (code != ParseCode.Success) return code;

            RussianStress alt = 0;
            if (parser.Skip('/'))
            {
                code = ParseSingleStress(ref parser, out alt);
                if (code != ParseCode.Success) return code;
            }

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
                return ParseCode.Success;
            }

            stress = 0;
            return ParseCode.StressNotFound;
        }

        [Pure] public static RussianStressPattern Parse(string text)
        {
            Guard.ThrowIfNull(text);
            return Parse(text.AsSpan());
        }
        [Pure] public static RussianStressPattern Parse(ReadOnlySpan<char> text)
            => ParseInternal(text, out RussianStressPattern pattern).ReturnOrThrow(pattern, nameof(text));

        [Pure] public static bool TryParse([NotNullWhen(true)] string? text, out RussianStressPattern pattern)
        {
            if (text is null)
            {
                pattern = default;
                return false;
            }
            return TryParse(text.AsSpan(), out pattern);
        }
        [Pure] public static bool TryParse(ReadOnlySpan<char> text, out RussianStressPattern pattern)
            => ParseInternal(text, out pattern) is ParseCode.Success;

    }
}
