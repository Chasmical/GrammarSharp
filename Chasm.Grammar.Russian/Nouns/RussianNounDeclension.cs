using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianNounDeclension : IEquatable<RussianNounDeclension>
    {
        // Representation (_data field):
        //   xxxx_1111 - stem type      (0000 - 0, 1000 - 8)
        //   1111_xxxx - stress pattern (see RussianStressPattern enum)
        //
        // Examples:
        //   0000_0000 - 0
        //   0001_0001 - 1a
        //   0001_1000 - 8a
        //   0110_1000 - 8f
        //   1110_1000 - 8f′
        //   1111_1000 - 8f″
        //
        private readonly byte _data;
        private readonly byte _flags;

        public int StemType => _data & 0x0F;
        public RussianStressPattern Stress => (RussianStressPattern)(_data >> 4);
        public RussianDeclensionFlags Flags => (RussianDeclensionFlags)_flags;

        public bool IsZero => _data == 0;

        public RussianNounDeclension(string text)
            => this = Parse(text);
        public RussianNounDeclension(ReadOnlySpan<char> text)
            => this = Parse(text);

        public RussianNounDeclension(int stemType, char stress)
        {
            Guard.ThrowIfNotInRange(stemType, 1, 8);
            Guard.ThrowIfNotInRange(stress, 'a', 'f');
            _data = (byte)(stemType | ((stress - '`') << 4));
        }
        public RussianNounDeclension(int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            Guard.ThrowIfNotInRange(stemType, 1, 8);
            ValidateNonZeroStressPattern(stress);
            _data = (byte)(stemType | ((int)stress << 4));
            _flags = (byte)flags;
        }

        private static void ValidateNonZeroStressPattern(RussianStressPattern stress)
        {
            // Allow values A through F
            if ((uint)(stress - RussianStressPattern.A) <= RussianStressPattern.F - RussianStressPattern.A) return;

            // Throw on values outside the range and on a′, c′, e′ and c″ (not valid for nouns)
            if (stress > RussianStressPattern.Fpp || ((int)stress & 1) == 0)
                ThrowOutOfRange(stress);

            static void ThrowOutOfRange(RussianStressPattern stress)
            {
                string msg = $"{nameof(stress)} ({(int)stress}) is not a valid noun stress pattern.";
                throw new ArgumentOutOfRangeException(nameof(stress), stress, msg);
            }
        }

        [Pure] public bool Equals(RussianNounDeclension other)
            => _data == other._data;
        [Pure] public override bool Equals(object? obj)
            => obj is RussianNounDeclension other && Equals(other);
        [Pure] public override int GetHashCode()
            => _data;

        [Pure] public static bool operator ==(RussianNounDeclension left, RussianNounDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianNounDeclension left, RussianNounDeclension right)
            => !(left == right);

        [Pure] public override string ToString()
        {
            if (_data < 0x0070)
            {
                if (_data == 0) return "0";

                // A common declension type - just the stem type index and the stress pattern
                return ((Span<char>)[(char)(StemType + '0'), (char)((int)Stress + '`')]).ToString();
            }
            // A rarer declension type - with special symbols
            return ToStringRare();
        }
        [Pure] private string ToStringRare()
        {
            // Longest form (11 chars): 1*°f″①②③, ё
            Span<char> buffer = stackalloc char[16];

            // Append the stem type index
            buffer[0] = (char)(StemType + '0');
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

            // Append the stress pattern
            RussianStressPattern stress = Stress;
            if (stress < RussianStressPattern.Ap)
            {
                // Regular stress patterns: a = 1, f = 6
                buffer[i++] = (char)((int)stress + '`');
            }
            else if (stress < RussianStressPattern.Cpp)
            {
                // Single-prime stress patterns: a′ = 8, b′ = 13
                buffer[i++] = (char)((int)stress + ('a' - RussianStressPattern.Ap));
                buffer[i++] = '′';
            }
            else
            {
                // Double-prime stress patterns c and f: c″ = 14, f″ = 15
                buffer[i++] = stress == RussianStressPattern.Cpp ? 'c' : 'f';
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
