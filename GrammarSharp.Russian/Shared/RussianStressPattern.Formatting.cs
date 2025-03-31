using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianStressPattern
    {
        /// <summary>
        ///   <para>Returns a string representation of this Russian stress pattern.</para>
        /// </summary>
        /// <returns>The string representation of this Russian stress pattern.</returns>
        [Pure] public readonly override string ToString()
        {
            if (_data == 0) return "";

            var (main, alt) = this;

            if (alt == RussianStress.Zero) return stressStringLookup[(int)main];

            return ToStringBoth(stressStringLookup, main, alt);
        }
        [Pure] private static string ToStringBoth(string[] lookup, RussianStress main, RussianStress alt)
            => $"{lookup[(int)main]}/{lookup[(int)alt]}";

        /// <summary>
        ///   <para>Converts this Russian stress pattern to its equivalent string representation, using the specified <paramref name="format"/>.</para>
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     <c>G</c> — outputs both the main and alternative stress schemas;<br/>
        ///     <c>N</c> — formats the pattern for nouns (a/ ⇒ a, d′/ ⇒ d′);<br/>
        ///     <c>A</c> — formats the pattern for adjectives (a/a ⇒ a, b/b ⇒ b, b/b′ ⇒ b′);<br/>
        ///     <c>V</c> — formats the pattern for verbs (b/a ⇒ b, c/a ⇒ c);
        ///   </para>
        /// </remarks>
        /// <param name="format">The format to use.</param>
        /// <returns>The string representation of this Russian stress pattern, as specified by <paramref name="format"/>.</returns>
        /// <exception cref="FormatException">The specified <paramref name="format"/> is not a valid format specifier.</exception>
        [Pure] public readonly string ToString(ReadOnlySpan<char> format)
        {
            if (_data == 0) return "";

            if (format.Length == 0) return ToString();

            if (format.Length == 1)
            {
                var (main, alt) = this;
                string[] lookup = stressStringLookup;

                // If the alternative stress is not specified, use the short format regardless
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
                        if (main <= RussianStress.F && alt - 6 == main) return lookup[(int)alt];
                        // Otherwise, use full format, a/b, b/c′
                        return ToStringBoth(lookup, main, alt);
                    case 'v':
                        // Shorten a/a to a, c/a to c, c′/a to c′
                        if (alt == RussianStress.A) return lookup[(int)main];
                        // Otherwise, use full format, b/b, c′/b
                        return ToStringBoth(lookup, main, alt);
                }
            }
            throw new FormatException($"'{format.ToString()}' is not a valid format for Russian stress patterns.");
        }

        private static readonly string[] stressStringLookup =
            [null!, "a", "b", "c", "d", "e", "f", "a′", "b′", "c′", "d′", "e′", "f′", "c″", "f″", ""];

    }
}
