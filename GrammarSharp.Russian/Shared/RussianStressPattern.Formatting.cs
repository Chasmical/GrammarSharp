using System;
using System.Runtime.CompilerServices;
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
            => _data == 0 ? "" : ToStringBoth(Main, Alt);

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

            RussianStress main = Main, alt = Alt;

            // If format is empty/null, use the full format
            if (format.Length == 0) return ToStringBoth(main, alt);

            if (format.Length == 1)
            {
                switch (format[0] | ' ')
                {
                    case 'g':
                        // Always use the full format
                        return ToStringBoth(main, alt);

                    case 'n' or 'p':
                        // If alternative stress is zero, output just the main stress
                        if (alt == RussianStress.Zero) return singleStressLookup[(int)main];
                        // Otherwise, format them both (not really valid for nouns)
                        return ToStringBoth(main, alt);

                    case 'a':
                        // Shorten a/a to a, b/b to b
                        if (alt == main) return singleStressLookup[(int)main];
                        // Shorten a/a′ to a′, b/b′ to b′, c/c″ to c″
                        if (main is <= RussianStress.F and not 0)
                        {
                            if (
                                main == alt - 6 ||
                                alt >= RussianStress.Cpp && main == (alt == RussianStress.Cpp ? RussianStress.C : RussianStress.F)
                            )
                                return singleStressLookup[(int)alt];
                        }
                        // Otherwise, use the full format, a/b, b/c′
                        return ToStringBoth(main, alt);

                    case 'v':
                        // Shorten a/a to a, c/a to c, c′/a to c′
                        if (alt == RussianStress.A && main != 0) return singleStressLookup[(int)main];
                        // Otherwise, use the full format, b/b, c′/b
                        return ToStringBoth(main, alt);
                }
            }

            // The format specifier was not handled, throw exception
            throw new FormatException($"'{format.ToString()}' is not a valid format for Russian stress patterns.");
        }

        private static readonly string[] singleStressLookup =
            [null!, "a", "b", "c", "d", "e", "f", "a′", "b′", "c′", "d′", "e′", "f′", "c″", "f″"];

        [Pure] private static string ToStringBoth(RussianStress main, RussianStress alt)
        {
            // Longest: f″/f″ (5 chars)
            Span<char> buffer = stackalloc char[8];
            int offset = 0;

            AppendSingleStress(buffer, ref offset, main);
            buffer[offset++] = '/';
            AppendSingleStress(buffer, ref offset, alt);

            return new string(buffer[..offset]);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void AppendSingleStress(Span<char> buffer, ref int offset, RussianStress stress)
            {
                switch (stress)
                {
                    case 0:
                        break;
                    case <= RussianStress.F:
                        buffer[offset++] = (char)('a' - 1 + (int)stress);
                        break;
                    case <= RussianStress.Fp:
                        buffer[offset++] = (char)('a' - 7 + (int)stress);
                        buffer[offset++] = '′';
                        break;
                    default:
                        buffer[offset++] = stress == RussianStress.Cpp ? 'c' : 'f';
                        buffer[offset++] = '″';
                        break;
                }
            }
        }

    }
}
