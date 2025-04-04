using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public struct RussianAdjectiveDeclension
    {
        // Representation (_stemTypeAndReflexive):
        //   01_xx_xxxx - adjective declension type constant
        //   xx_x1_xxxx - whether adjective is reflexive
        //   xx_1x_xxxx - < UNUSED >
        //   xx_xx_1111 - stem type
        // Representation (_stressPattern):
        //   1111_1111 - see RussianStressPattern enum
        // Representation (_declensionFlags):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        private byte _typesAndReflexive;
        private RussianStressPattern _stressPattern;
        private byte _declensionFlags;

        private const byte AdjectiveTypeId = (int)RussianDeclensionType.Adjective << 6;

        public int StemType
        {
            readonly get => _typesAndReflexive & 0x0F;
            set
            {
                ValidateStemType(value);
                _typesAndReflexive = (byte)((_typesAndReflexive & 0xF0) | value);
            }
        }
        public RussianStressPattern StressPattern
        {
            readonly get => _stressPattern;
            set
            {
                ValidateStress(ref value);
                _stressPattern = value;
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

        public bool IsReflexive
        {
            readonly get => (_typesAndReflexive & 0x10) != 0;
            set => _typesAndReflexive = (byte)((_typesAndReflexive & 0xEF) | (value ? 0x10 : 0));
        }

        public readonly bool IsZero => StemType == 0;

        public RussianAdjectiveDeclension(int stemType, RussianStressPattern stressPattern)
        {
            ValidateStemType(stemType);
            ValidateStress(ref stressPattern);
            _typesAndReflexive = (byte)(AdjectiveTypeId | stemType);
            _stressPattern = stressPattern;
        }
        public RussianAdjectiveDeclension(int stemType, RussianStressPattern stressPattern, RussianDeclensionFlags flags)
            : this(stemType, stressPattern)
        {
            ValidateFlags(flags);
            _declensionFlags = (byte)flags;
        }

        private static void ValidateStemType(int stemType, [CAE(nameof(stemType))] string? paramName = null)
        {
            // Adjectives can only have 0 through 7 stem types
            if ((uint)stemType > 7u)
                Throw(stemType, paramName);

            static void Throw(int stemType, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, stemType, $"{stemType} is not a valid stem type for adjectives.");
        }

        private static void ValidateStress(ref RussianStressPattern pattern, [CAE(nameof(pattern))] string? paramName = null)
        {
            if (!pattern.TryNormalizeForAdjective())
                Throw(pattern, paramName);

            static void Throw(RussianStressPattern pattern, string? paramName)
                => throw new ArgumentException($"{pattern} is not a valid stress schema for adjectives.", paramName);
        }

        private static void ValidateFlags(RussianDeclensionFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            // Adjectives can only have *, ①, ② and ё flags
            const RussianDeclensionFlags validFlags
                = RussianDeclensionFlags.Star | RussianDeclensionFlags.CircledOne |
                  RussianDeclensionFlags.CircledTwo | RussianDeclensionFlags.AlternatingYo;

            if ((flags & ~validFlags) != 0) Throw(flags, paramName);

            static void Throw(RussianDeclensionFlags flags, string? paramName)
                => throw new ArgumentException($"{flags & ~validFlags} is not a valid declension flag for adjectives.", paramName);
        }

        [Pure] public readonly bool Equals(RussianAdjectiveDeclension other)
            => (_typesAndReflexive | AdjectiveTypeId).Equals(other._typesAndReflexive | AdjectiveTypeId) &&
               _stressPattern.Equals(other._stressPattern) &&
               _declensionFlags == other._declensionFlags;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveDeclension other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_typesAndReflexive | AdjectiveTypeId, _stressPattern, _declensionFlags);

        [Pure] public static bool operator ==(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => !(left == right);

        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
