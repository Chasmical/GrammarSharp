using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianStressPattern
    {
        [Pure] public override string ToString()
        {
            if (_data == 0) return "";

            var (main, alt) = this;

            if (alt == RussianStress.Zero) return stressStringLookup[(int)main];

            return ToStringBoth(stressStringLookup, main, alt);
        }
        [Pure] private static string ToStringBoth(string[] lookup, RussianStress main, RussianStress alt)
            => $"{lookup[(int)main]}/{lookup[(int)alt]}";

        [Pure] public string ToString(ReadOnlySpan<char> format)
        {
            if (_data == 0) return "";

            if (format.Length == 0) return ToString();

            if (format.Length == 1)
            {
                var (main, alt) = this;
                string[] lookup = stressStringLookup;

                // If the alternate stress is not specified, use the short format regardless
                if (alt == RussianStress.Zero) return lookup[(int)main];

                switch (format[0] | ' ')
                {
                    case 'n' or 'g':
                        // Always use full format, unless alt is unspecified
                        return ToStringBoth(lookup, main, alt);
                    case 'a':
                        // Shorten a/a to a, b/b to b
                        if (alt == main) return lookup[(int)main];
                        // Shorten a/a′ to a′, b/b′ to b′
                        if (main <= RussianStress.F && alt - 0b_0111 == main) return lookup[(int)alt];
                        // Otherwise, use full format, a/b, b/c′
                        return ToStringBoth(lookup, main, alt);
                    case 'v':
                        // Shorten a/a to a, c/a to c, c′/a to c′
                        if (alt == RussianStress.A) return lookup[(int)main];
                        // Otherwise, use full format, b/b, c′/b
                        return ToStringBoth(lookup, main, alt);
                    default:
                        throw new FormatException(); // TODO: exception
                }
            }
            throw new FormatException(); // TODO: exception
        }

        private static readonly string[] stressStringLookup =
        [
            null!,
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "",
            "a′",
            "b′",
            "c′",
            "d′",
            "e′",
            "f′",
            "c″",
            "f″",
        ];

    }
}
