using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public struct RussianPronounDeclension : IEquatable<RussianPronounDeclension>
    {
        // Representation (_type):
        //   10_xxxxxx - pronoun declension type constant
        //   xx_111111 - < UNUSED >
        // Representation (_stemTypeAndStress):
        //   xxxx_1111 - stem type
        //   1111_xxxx - stress pattern
        // Representation (_declensionFlags):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        [UsedImplicitly] private readonly byte _typeAndPadding;
        private byte _stemTypeAndStress;
        private byte _declensionFlags;

        private const byte PronounTypeId = (int)RussianDeclensionType.Pronoun << 6;

        public int StemType
        {
            readonly get => _stemTypeAndStress & 0x0F;
            set
            {
                ValidateStemType(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0xF0) | value);
            }
        }
        public RussianStress Stress
        {
            readonly get => (RussianStress)(_stemTypeAndStress >> 4);
            set
            {
                ValidateStress(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0x0F) | ((int)value << 4));
            }
        }
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_declensionFlags;
            set
            {
                ValidateFlags(value);
                _declensionFlags = (byte)value;
            }
        }

        public readonly bool IsZero => StemType == 0;

        public RussianPronounDeclension(int stemType, RussianStress stress)
        {
            ValidateStemType(stemType);
            ValidateStress(stress);
            _typeAndPadding = PronounTypeId;
            _stemTypeAndStress = (byte)(stemType | ((int)stress << 4));
        }
        public RussianPronounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags)
            : this(stemType, stress)
        {
            ValidateFlags(flags);
            _declensionFlags = (byte)flags;
        }

        private static void ValidateStemType(int stemType, [CAE(nameof(stemType))] string? paramName = null)
        {
            // Pronouns can only have 0, 1, 2, 4 and 6 stem types
            if (stemType is not 0 and not 1 and not 2 and not 4 and not 6)
                Throw(stemType, paramName);

            static void Throw(int stemType, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, stemType, $"{stemType} is not a valid stem type for pronouns.");
        }

        private static void ValidateStress(RussianStress stress, [CAE(nameof(stress))] string? paramName = null)
        {
            // Pronouns can only have 0, a, b and f stress schemas
            if ((uint)stress is > (uint)RussianStress.B and not (uint)RussianStress.F)
                Throw(stress, paramName);

            static void Throw(RussianStress stress, string? paramName)
                => throw new ArgumentException($"{stress} is not a valid stress schema for pronouns.", paramName);
        }

        private static void ValidateFlags(RussianDeclensionFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            // Pronouns can only have a * flag
            if ((uint)flags > (uint)RussianDeclensionFlags.Star) Throw(flags, paramName);

            static void Throw(RussianDeclensionFlags flags, string? paramName)
            {
                const RussianDeclensionFlags invalidFlags = ~RussianDeclensionFlags.Star;
                throw new ArgumentException($"{flags & invalidFlags} is not a valid declension flag for pronouns.", paramName);
            }
        }

        [Pure] public readonly bool Equals(RussianPronounDeclension other)
            => _stemTypeAndStress == other._stemTypeAndStress &&
               _declensionFlags == other._declensionFlags;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianPronounDeclension other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(PronounTypeId, _stemTypeAndStress, _declensionFlags);

        [Pure] public static bool operator ==(RussianPronounDeclension left, RussianPronounDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianPronounDeclension left, RussianPronounDeclension right)
            => !(left == right);

        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
