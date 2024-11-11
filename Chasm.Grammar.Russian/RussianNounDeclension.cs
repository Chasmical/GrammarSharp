using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianNounDeclension : IEquatable<RussianNounDeclension>
    {
        // Representation:
        //   xxxx_xxxx_xxxx_1111 - word stem type (0000 - 0, 1000 - 8)
        //   xxxx_xxxx_1111_xxxx - accenting type (see RussianDeclensionAccent enum)
        //   1111_1111_xxxx_xxxx - various flags  (see RussianDeclensionFlags enum)
        //
        // Examples:
        //   xxxx_xxxx_0000_0000 - 0
        //   xxxx_xxxx_0001_0001 - 1a
        //   xxxx_xxxx_0001_1000 - 8a
        //   xxxx_xxxx_0110_1000 - 8f
        //   xxxx_xxxx_1110_1000 - 8f′
        //   xxxx_xxxx_1111_1000 - 8f″
        //
        private readonly ushort _data;

        public int Digit => _data & 0x000F;
        public RussianDeclensionAccent Letter => (RussianDeclensionAccent)((byte)_data >> 4);
        public RussianDeclensionFlags Flags => (RussianDeclensionFlags)(_data >> 8);

        public bool IsZero => _data == 0;

        public RussianNounDeclension(string text)
            => this = Parse(text);
        public RussianNounDeclension(ReadOnlySpan<char> text)
            => this = Parse(text);

        public RussianNounDeclension(int digit, char letter)
        {
            Guard.ThrowIfNotInRange(digit, 1, 8);
            Guard.ThrowIfNotInRange(letter, 'a', 'f');
            _data = (ushort)(digit | ((letter - '`') << 4));
        }
        public RussianNounDeclension(int digit, RussianDeclensionAccent letter, RussianDeclensionFlags flags)
        {
            Guard.ThrowIfNotInRange(digit, 1, 8);
            ValidateNonZeroAccentLetter(letter);
            _data = (ushort)(digit | ((int)letter << 4) | ((int)flags << 8));
        }

        private static void ValidateNonZeroAccentLetter(RussianDeclensionAccent letter)
        {
            // Allow values A through F
            if ((uint)(letter - RussianDeclensionAccent.A) < RussianDeclensionAccent.F - RussianDeclensionAccent.A) return;

            // Throw on values outside the range and on a′, c′, e′ and c″ (not valid for nouns)
            if (letter > RussianDeclensionAccent.Fpp || ((int)letter & 1) == 0)
                ThrowOutOfRange(letter);

            static void ThrowOutOfRange(RussianDeclensionAccent letter)
            {
                string msg = $"{nameof(letter)} ({(int)letter}) is not a valid declension accent letter.";
                throw new ArgumentOutOfRangeException(nameof(letter), letter, msg);
            }
        }

        [Pure] public bool Equals(RussianNounDeclension other)
            => _data == other._data;
        [Pure] public override bool Equals(object? obj)
            => obj is RussianNounDeclension other && Equals(other);
        [Pure] public override int GetHashCode()
            => _data;

        [Pure] public override string ToString()
        {
            if (_data < 0x0070)
            {
                if (_data == 0) return "0";

                // A common declension type - just the digit and the letter
                Span<char> buffer = stackalloc char[2];
                buffer[0] = (char)(Digit + '0');
                buffer[1] = (char)((int)Letter + '`');
                return buffer.ToString();
            }
            // A rarer declension type - with special symbols
            return ToStringRare();
        }
        [Pure] private string ToStringRare()
        {
            // Longest form (11 chars): 1*°f″①②③, ё
            Span<char> buffer = stackalloc char[16];

            // Append the index digit
            buffer[0] = (char)(Digit + '0');
            int i = 1;

            // Append the star and the circle
            RussianDeclensionFlags flags = Flags;
            if ((flags & (RussianDeclensionFlags.Star | RussianDeclensionFlags.Circle)) != 0)
            {
                if ((flags & RussianDeclensionFlags.Star) != 0)
                    buffer[i++] = '*';
                if ((flags & RussianDeclensionFlags.Circle) != 0)
                    buffer[i++] = '°';
            }

            // Append the index letter
            RussianDeclensionAccent letter = Letter;
            if (letter < RussianDeclensionAccent.Ap)
            {
                // Regular letters: a = 1, f = 6
                buffer[i++] = (char)((int)letter + '`');
            }
            else if (letter < RussianDeclensionAccent.Cpp)
            {
                // Single-prime letters: a′ = 8, b′ = 13
                buffer[i++] = (char)((int)letter + ('a' - RussianDeclensionAccent.Ap));
                buffer[i++] = '′';
            }
            else
            {
                // Double-prime letters c and f: c″ = 14, f″ = 15
                buffer[i++] = letter == RussianDeclensionAccent.Cpp ? 'c' : 'f';
                buffer[i++] = '″';
            }

            const RussianDeclensionFlags trailingFlags
                = RussianDeclensionFlags.CircledOne
                | RussianDeclensionFlags.CircledTwo
                | RussianDeclensionFlags.CircledThree
                | RussianDeclensionFlags.AlternatingYo;

            if ((flags & trailingFlags) != 0)
            {
                // Append the digits in circles
                if ((flags & RussianDeclensionFlags.CircledOne) != 0)
                    buffer[i++] = '①';
                if ((flags & RussianDeclensionFlags.CircledTwo) != 0)
                    buffer[i++] = '②';
                if ((flags & RussianDeclensionFlags.CircledThree) != 0)
                    buffer[i++] = '③';

                // Append the ё mark
                if ((flags & RussianDeclensionFlags.AlternatingYo) != 0)
                {
                    buffer[i++] = ',';
                    buffer[i++] = ' ';
                    buffer[i++] = 'ё';
                }
            }

            // Build and return the string
            return buffer[..i].ToString();
        }

    }
}
