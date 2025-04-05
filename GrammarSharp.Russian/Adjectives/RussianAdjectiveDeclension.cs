using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian declension of adjective type, according to Zaliznyak's classification.</para>
    /// </summary>
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

        /// <summary>
        ///   <para>Gets or sets the adjective declension's stem type.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not between 0 and 7.</exception>
        public int StemType
        {
            readonly get => _typesAndReflexive & 0x0F;
            set
            {
                ValidateStemType(value);
                _typesAndReflexive = (byte)((_typesAndReflexive & 0xF0) | value);
            }
        }
        /// <summary>
        ///   <para>Gets or sets the adjective declension's stress pattern.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/>'s main form's stress is not one of the following: 0, a, b, c; or <paramref name="value"/>'s alternative form's stress is not one of the following: 0, a, b, c, a′, b′, c′, c″.</exception>
        public RussianStressPattern StressPattern
        {
            readonly get => _stressPattern;
            set
            {
                ValidateStress(ref value);
                _stressPattern = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the adjective declension's flags.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> specifies a flag other than the following: *, ①, ②, ё.</exception>
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_declensionFlags;
            set
            {
                ValidateFlags(value);
                _declensionFlags = (byte)value;
            }
        }

        /// <summary>
        ///   <para>Gets or sets whether the adjective is reflexive (ends with 'ся').</para>
        /// </summary>
        public bool IsReflexive
        {
            readonly get => (_typesAndReflexive & 0x10) != 0;
            set => _typesAndReflexive = (byte)((_typesAndReflexive & 0xEF) | (value ? 0x10 : 0));
        }
        /// <summary>
        ///   <para>Determines whether this adjective declension is zero.</para>
        /// </summary>
        public readonly bool IsZero => StemType == 0;

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjectiveDeclension"/> structure with the specified <paramref name="stemType"/> and <paramref name="stressPattern"/>.</para>
        /// </summary>
        /// <param name="stemType">The adjective declension's stem type.</param>
        /// <param name="stressPattern">The adjective declension's stress pattern.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not between 0 and 7.</exception>
        /// <exception cref="ArgumentException"><paramref name="stressPattern"/>'s main form's stress is not one of the following: 0, a, b, c; or <paramref name="stressPattern"/>'s alternative form's stress is not one of the following: 0, a, b, c, a′, b′, c′, c″.</exception>
        public RussianAdjectiveDeclension(int stemType, RussianStressPattern stressPattern)
        {
            ValidateStemType(stemType);
            ValidateStress(ref stressPattern);
            _typesAndReflexive = (byte)(AdjectiveTypeId | stemType);
            _stressPattern = stressPattern;
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjectiveDeclension"/> structure with the specified <paramref name="stemType"/>, <paramref name="stressPattern"/> and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="stemType">The adjective declension's stem type.</param>
        /// <param name="stressPattern">The adjective declension's stress pattern.</param>
        /// <param name="flags">The adjective declension's flags.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not between 0 and 7.</exception>
        /// <exception cref="ArgumentException"><paramref name="stressPattern"/>'s main form's stress is not one of the following: 0, a, b, c; or <paramref name="stressPattern"/>'s alternative form's stress is not one of the following: 0, a, b, c, a′, b′, c′, c″.</exception>
        /// <exception cref="ArgumentException"><paramref name="flags"/> specifies a flag other than the following: *, ①, ②, ё.</exception>
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

        /// <summary>
        ///   <para>Determines whether this Russian adjective declension is equal to another specified Russian adjective declension.</para>
        /// </summary>
        /// <param name="other">The Russian adjective declension to compare with this Russian adjective declension.</param>
        /// <returns><see langword="true"/>, if this Russian adjective declension is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianAdjectiveDeclension other)
            => (_typesAndReflexive | AdjectiveTypeId).Equals(other._typesAndReflexive | AdjectiveTypeId) &&
               _stressPattern.Equals(other._stressPattern) &&
               _declensionFlags == other._declensionFlags;
        /// <summary>
        ///   <para>Determines whether this Russian adjective declension is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian adjective declension.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianAdjectiveDeclension"/> instance equal to this Russian adjective declension; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveDeclension other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian adjective declension.</para>
        /// </summary>
        /// <returns>A hash code for this Russian adjective declension.</returns>
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_typesAndReflexive | AdjectiveTypeId, _stressPattern, _declensionFlags);

        /// <summary>
        ///   <para>Determines whether the two specified Russian adjective declensions are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian adjective declension to compare.</param>
        /// <param name="right">The second Russian adjective declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian adjective declensions are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian adjective declension to compare.</param>
        /// <param name="right">The second Russian adjective declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => !(left == right);

        /// <summary>
        ///   <para>Returns a string representation of this Russian adjective declension.</para>
        /// </summary>
        /// <returns>The string representation of this Russian adjective declension.</returns>
        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
