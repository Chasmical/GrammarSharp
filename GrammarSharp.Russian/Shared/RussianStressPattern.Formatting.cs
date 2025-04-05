using System;
using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianStressPattern
#if NET6_0_OR_GREATER
        : ISpanFormattable
#else
        : IFormattable
#endif
    {
        /// <summary>
        ///   <para>Returns a string representation of this Russian stress pattern.</para>
        /// </summary>
        /// <returns>The string representation of this Russian stress pattern.</returns>
        [Pure] public readonly override string ToString()
            => ToString(default(ReadOnlySpan<char>));
        /// <summary>
        ///   <para>Converts this Russian stress pattern to its equivalent string representation, formatting it for the specified declension <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The declension type to format this Russian stress pattern to.</param>
        /// <returns>The string representation of this Russian stress pattern, formatted for declension <paramref name="type"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="type"/> is not a valid declension type.</exception>
        [Pure] public readonly string ToString(RussianDeclensionType type)
            => ToString(GetFormatSpecifierForType(type));
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
        /// <exception cref="FormatException"><paramref name="format"/> is not a valid format specifier.</exception>
        [Pure] public readonly string ToString(ReadOnlySpan<char> format)
        {
            if (IsZero) return "";

            RussianStress main = Main, alt = Alt;
            int requiredLength;

            // Alternative form stress is not specified, output just the main form stress
            if (alt == RussianStress.Zero) return ToStringSingleStress(main);

            // If main form stress is missing, allocate and format a string
            if (main == RussianStress.Zero)
            {
                requiredLength = 2 + (alt > RussianStress.F ? 1 : 0);
                return string.Create(requiredLength, (main, alt), formatBothToStringAction);
            }

            // Both forms' stresses are specified, see if the format specifier can shorten it
            if (format.Length > 0)
            {
                if (format.Length > 1) ThrowInvalidFormatException(format.ToString());

                RussianStress shortened = ProcessFormatSpecifier(main, alt, (char)(format[0] | ' '));
                if (shortened != RussianStress.Zero) return ToStringSingleStress(shortened);
            }

            // Both forms' stresses are specified, allocate and format a string
            requiredLength = 3 + (main > RussianStress.F ? 1 : 0) + (alt > RussianStress.F ? 1 : 0);
            return string.Create(requiredLength, (main, alt), formatBothToStringAction);
        }

        /// <summary>
        ///   <para>Tries to format this Russian stress pattern into the provided span of characters.</para>
        /// </summary>
        /// <param name="destination">When this method returns, this Russian stress pattern formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in <paramref name="destination"/>.</param>
        /// <returns><see langword="true"/>, if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool TryFormat(Span<char> destination, out int charsWritten)
            => TryFormat(destination, out charsWritten, default(ReadOnlySpan<char>));
        /// <summary>
        ///   <para>Tries to format this Russian stress pattern into the provided span of characters, formatting it for the specified declension <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="destination">When this method returns, this Russian stress pattern formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in <paramref name="destination"/>.</param>
        /// <param name="type">The declension type to format this Russian stress pattern to.</param>
        /// <returns><see langword="true"/>, if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="type"/> is not a valid declension type.</exception>
        [Pure] public readonly bool TryFormat(Span<char> destination, out int charsWritten, RussianDeclensionType type)
            => TryFormat(destination, out charsWritten, GetFormatSpecifierForType(type));
        /// <summary>
        ///   <para>Tries to format this Russian stress pattern into the provided span of characters using the specified <paramref name="format"/>.</para>
        /// </summary>
        /// <inheritdoc cref="ToString(System.ReadOnlySpan{char})" path="/remarks" />
        /// <param name="destination">When this method returns, this Russian stress pattern formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in <paramref name="destination"/>.</param>
        /// <param name="format">The format to use.</param>
        /// <returns><see langword="true"/>, if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is not a valid format specifier.</exception>
        [Pure] public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format)
        {
            if (IsZero)
            {
                charsWritten = 0;
                return true;
            }

            RussianStress main = Main, alt = Alt;
            int offset = 0, requiredLength;

            // If alternative form stress is not specified, output just the main form stress
            if (alt == RussianStress.Zero) goto FORMAT_SINGLE_MAIN;

            // If both forms' stresses are specified, see if the format specifier can shorten it
            if (main != RussianStress.Zero && format.Length != 0)
            {
                if (format.Length > 1) ThrowInvalidFormatException(format.ToString());

                RussianStress shortened = ProcessFormatSpecifier(main, alt, (char)(format[0] | ' '));
                if (shortened != RussianStress.Zero)
                {
                    main = shortened;
                    goto FORMAT_SINGLE_MAIN;
                }
            }

            // Format both forms' stresses (main could be zero)

            // If there may not be enough space (max 5 chars), perform a more accurate check
            if (destination.Length < 5)
            {
                requiredLength = 3 + (main > RussianStress.F ? 1 : main == 0 ? -1 : 0) + (alt > RussianStress.F ? 1 : 0);
                if (requiredLength > destination.Length) goto FAIL;
            }
            FormatSingleStress(destination, ref offset, main);
            destination[offset++] = '/';
            FormatSingleStress(destination, ref offset, alt);
            charsWritten = offset;
            return true;

        FORMAT_SINGLE_MAIN:
            // If there may not be enough space (max 2 chars), perform a more accurate check
            if (destination.Length < 2)
            {
                requiredLength = 1 + (main > RussianStress.F ? 1 : 0);
                if (requiredLength > destination.Length) goto FAIL;
            }
            FormatSingleStress(destination, ref offset, main);
            charsWritten = offset;
            return true;

        FAIL:
            charsWritten = 0;
            return false;
        }

        readonly string IFormattable.ToString(string? format, IFormatProvider? _)
            => ToString(format);
#if NET6_0_OR_GREATER
        readonly bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? _)
            => TryFormat(destination, out charsWritten, format);
#endif

        [Pure] private static ReadOnlySpan<char> GetFormatSpecifierForType(RussianDeclensionType type)
        {
            if ((uint)type > (uint)RussianDeclensionType.Pronoun) Throw(type);
            return "nap".AsSpan((int)type, 1);

            static void Throw(RussianDeclensionType type)
                => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(RussianDeclensionType));
        }
        [Pure] private static RussianStress ProcessFormatSpecifier(RussianStress main, RussianStress alt, char formatSpecifier)
        {
            switch (formatSpecifier)
            {
                case 'g' or 'n' or 'p':
                    break;
                case 'a':
                {
                    // Shorten a/a to a, b/b to b
                    if (alt == main) return main;
                    // Shorten a/a′ to a′, b/b′ to b′, c/c″ to c″
                    if (main <= RussianStress.F)
                    {
                        if (
                            main == alt - 6 ||
                            alt >= RussianStress.Cpp && main == (alt == RussianStress.Cpp ? RussianStress.C : RussianStress.F)
                        )
                            return main;
                    }
                    // Otherwise, use the full format, a/b, b/c′
                    break;
                }
                case 'v':
                {
                    // Shorten a/a to a, c/a to c, c′/a to c′
                    if (alt == RussianStress.A && main != 0) return main;
                    // Otherwise, use the full format, b/b, c′/b
                    break;
                }
                default:
                {
                    ThrowInvalidFormatException(formatSpecifier.ToString());
                    break;
                }
            }
            return RussianStress.Zero;
        }

        private static readonly SpanAction<char, (RussianStress, RussianStress)> formatBothToStringAction = FormatBothToString;
        private static void FormatBothToString(Span<char> destination, (RussianStress Main, RussianStress Alt) stresses)
        {
            int offset = 0;
            FormatSingleStress(destination, ref offset, stresses.Main);
            destination[offset++] = '/';
            FormatSingleStress(destination, ref offset, stresses.Alt);
        }

        private static void ThrowInvalidFormatException(string format)
            => throw new FormatException($"'{format}' is not a valid format for Russian stress patterns.");

        [Pure] internal static string ToStringSingleStress(RussianStress stress)
            => singleStressLookup[(int)stress];

        private static readonly string[] singleStressLookup =
            ["", "a", "b", "c", "d", "e", "f", "a′", "b′", "c′", "d′", "e′", "f′", "c″", "f″"];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FormatSingleStress(Span<char> buffer, ref int offset, RussianStress stress)
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
