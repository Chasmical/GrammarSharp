using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian declension of pronoun type, according to Zaliznyak's classification.</para>
    /// </summary>
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

        /// <summary>
        ///   <para>Gets or sets the pronoun declension's stem type.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not one of the following: 0, 1, 2, 4, 6.</exception>
        public int StemType
        {
            readonly get => _stemTypeAndStress & 0x0F;
            set
            {
                ValidateStemType(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0xF0) | value);
            }
        }
        /// <summary>
        ///   <para>Gets or sets the pronoun declension's stress schema.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not one of the following: 0, a, b, f.</exception>
        public RussianStress Stress
        {
            readonly get => (RussianStress)(_stemTypeAndStress >> 4);
            set
            {
                ValidateStress(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0x0F) | ((int)value << 4));
            }
        }
        /// <summary>
        ///   <para>Gets or sets the pronoun declension's flags.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> specifies a flag other than a *.</exception>
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
        ///   <para>Determines whether this pronoun declension is zero.</para>
        /// </summary>
        public readonly bool IsZero => StemType == 0;

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianPronounDeclension"/> structure with the specified <paramref name="stemType"/> and <paramref name="stress"/>.</para>
        /// </summary>
        /// <param name="stemType">The pronoun declension's stem type.</param>
        /// <param name="stress">The pronoun declension's stress schema.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not one of the following: 0, 1, 2, 4, 6.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> is not one of the following: 0, a, b, f.</exception>
        public RussianPronounDeclension(int stemType, RussianStress stress)
        {
            ValidateStemType(stemType);
            ValidateStress(stress);
            _typeAndPadding = PronounTypeId;
            _stemTypeAndStress = (byte)(stemType | ((int)stress << 4));
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianPronounDeclension"/> structure with the specified <paramref name="stemType"/>, <paramref name="stress"/> and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="stemType">The pronoun declension's stem type.</param>
        /// <param name="stress">The pronoun declension's stress schema.</param>
        /// <param name="flags">The pronoun declension's flags.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not one of the following: 0, 1, 2, 4, 6.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> is not one of the following: 0, a, b, f.</exception>
        /// <exception cref="ArgumentException"><paramref name="flags"/> specifies a flag other than a *.</exception>
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

        /// <summary>
        ///   <para>Determines whether this Russian pronoun declension is equal to another specified Russian pronoun declension.</para>
        /// </summary>
        /// <param name="other">The Russian pronoun declension to compare with this Russian pronoun declension.</param>
        /// <returns><see langword="true"/>, if this Russian pronoun declension is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianPronounDeclension other)
            => _stemTypeAndStress == other._stemTypeAndStress &&
               _declensionFlags == other._declensionFlags;
        /// <summary>
        ///   <para>Determines whether this Russian pronoun declension is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian pronoun declension.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianPronounDeclension"/> instance equal to this Russian pronoun declension; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianPronounDeclension other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian pronoun declension.</para>
        /// </summary>
        /// <returns>A hash code for this Russian pronoun declension.</returns>
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(PronounTypeId, _stemTypeAndStress, _declensionFlags);

        /// <summary>
        ///   <para>Determines whether the two specified Russian pronoun declensions are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian pronoun declension to compare.</param>
        /// <param name="right">The second Russian pronoun declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianPronounDeclension left, RussianPronounDeclension right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian pronoun declensions are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian pronoun declension to compare.</param>
        /// <param name="right">The second Russian pronoun declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianPronounDeclension left, RussianPronounDeclension right)
            => !(left == right);

        /// <summary>
        ///   <para>Returns a string representation of this Russian pronoun declension.</para>
        /// </summary>
        /// <returns>The string representation of this Russian pronoun declension.</returns>
        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
