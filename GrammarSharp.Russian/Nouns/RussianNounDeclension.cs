using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian declension of noun type, according to Zaliznyak's classification.</para>
    /// </summary>
    public struct RussianNounDeclension : IEquatable<RussianNounDeclension>
    {
        // Representation (_typeAndProps):
        //   00_x_xxxxx - noun declension type constant
        //   xx_1_xxxxx - noun has special declension properties
        //   xx_x_11111 - declension gender, animacy and tantums (see RussianNounProperties enum)
        // Representation (_stemTypeAndStress):
        //   xxxx_1111 - stem type
        //   1111_xxxx - stress pattern
        // Representation (_declensionFlags):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        private RussianNounProperties _typeAndProps;
        private byte _stemTypeAndStress;
        private byte _declensionFlags;

        /// <summary>
        ///   <para>Gets or sets the noun declension's stem type.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not between 0 and 8.</exception>
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
        ///   <para>Gets or sets the noun declension's stress schema.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not one of the following: 0, a, b, c, d, e, f, b′, d′, f′, f″.</exception>
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
        ///   <para>Gets or sets the noun declension's flags.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid <seealso cref="RussianDeclensionFlags"/> value.</exception>
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
        ///   <para>Gets or sets the noun declension's special properties.</para>
        /// </summary>
        public RussianNounProperties? SpecialProperties
        {
            readonly get => _typeAndProps.ExtraData != 0 ? _typeAndProps.WithoutExtraData() : null;
            set
            {
                _typeAndProps = value.GetValueOrDefault();
                _typeAndProps.StripTantumsAndSetBoolExtraData(value.HasValue);
            }
        }
        /// <summary>
        ///   <para>Determines whether this noun declension is zero.</para>
        /// </summary>
        public readonly bool IsZero => StemType == 0;

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounDeclension"/> structure with the specified <paramref name="stemType"/> and <paramref name="stress"/>.</para>
        /// </summary>
        /// <param name="stemType">The noun declension's stem type.</param>
        /// <param name="stress">The noun declension's stress schema.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not between 0 and 8.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> is not one of the following: 0, a, b, c, d, e, f, b′, d′, f′, f″.</exception>
        public RussianNounDeclension(int stemType, RussianStress stress)
        {
            ValidateStemType(stemType);
            ValidateStress(stress);
            _stemTypeAndStress = (byte)(stemType | ((int)stress << 4));
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounDeclension"/> structure with the specified <paramref name="stemType"/>, <paramref name="stress"/> and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="stemType">The noun declension's stem type.</param>
        /// <param name="stress">The noun declension's stress schema.</param>
        /// <param name="flags">The noun declension's flags.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not between 0 and 8.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> is not one of the following: 0, a, b, c, d, e, f, b′, d′, f′, f″.</exception>
        /// <exception cref="ArgumentException"><paramref name="flags"/> is not a valid <seealso cref="RussianDeclensionFlags"/> value.</exception>
        public RussianNounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags)
            : this(stemType, stress)
        {
            ValidateFlags(flags);
            _declensionFlags = (byte)flags;
        }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounDeclension"/> structure with the specified <paramref name="stemType"/>, <paramref name="stress"/> and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="stemType">The noun declension's stem type.</param>
        /// <param name="stress">The noun declension's stress schema.</param>
        /// <param name="flags">The noun declension's flags.</param>
        /// <param name="specialProps">The noun declension's special properties.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="stemType"/> is not between 0 and 8.</exception>
        /// <exception cref="ArgumentException"><paramref name="stress"/> is not one of the following: 0, a, b, c, d, e, f, b′, d′, f′, f″.</exception>
        /// <exception cref="ArgumentException"><paramref name="flags"/> is not a valid <seealso cref="RussianDeclensionFlags"/> value.</exception>
        public RussianNounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags, RussianNounProperties? specialProps)
            : this(stemType, stress, flags)
        {
            SpecialProperties = specialProps;
        }

        private static void ValidateStemType(int stemType, [CAE(nameof(stemType))] string? paramName = null)
        {
            // Nouns can have 0 through 8 stem types (8 is exclusive to nouns)
            if ((uint)stemType > 8u)
                Throw(stemType, paramName);

            static void Throw(int stemType, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, stemType, $"{stemType} is not a valid stem type for nouns.");
        }

        private static void ValidateStress(RussianStress stress, [CAE(nameof(stress))] string? paramName = null)
        {
            // Nouns can only have 0, a through f, b′, d′, f′ and f″ stress schemas
            if ((uint)stress > (uint)RussianStress.Fpp || (uint)stress > (uint)RussianStress.F && ((uint)stress & 1) != 0)
                Throw(stress, paramName);

            static void Throw(RussianStress stress, string? paramName)
                => throw new ArgumentException($"{stress} is not a valid stress schema for nouns.", paramName);
        }

        private static void ValidateFlags(RussianDeclensionFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            // Nouns can have all the flags in the enumeration
            const uint validFlags = ((uint)RussianDeclensionFlags.AlternatingYo << 1) - 1;
            if ((uint)flags > validFlags) Throw(flags, paramName);

            static void Throw(RussianDeclensionFlags flags, string? paramName)
            {
                const RussianDeclensionFlags invalidFlags = unchecked((RussianDeclensionFlags)~validFlags);
                throw new ArgumentException($"{flags & invalidFlags} is not a valid declension flag for nouns.", paramName);
            }
        }

        /// <summary>
        ///   <para>Determines whether this Russian noun declension is equal to another specified Russian noun declension.</para>
        /// </summary>
        /// <param name="other">The Russian declension to compare with this Russian noun declension.</param>
        /// <returns><see langword="true"/>, if this Russian noun declension is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianNounDeclension other)
            => _typeAndProps.Equals(other._typeAndProps) &&
               _stemTypeAndStress == other._stemTypeAndStress &&
               _declensionFlags == other._declensionFlags;
        /// <summary>
        ///   <para>Determines whether this Russian noun declension is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian noun declension.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianNounDeclension"/> instance equal to this Russian noun declension; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounDeclension other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian noun declension.</para>
        /// </summary>
        /// <returns>A hash code for this Russian noun declension.</returns>
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_typeAndProps, _stemTypeAndStress, _declensionFlags);

        /// <summary>
        ///   <para>Determines whether the two specified Russian noun declensions are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun declension to compare.</param>
        /// <param name="right">The second Russian noun declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianNounDeclension left, RussianNounDeclension right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian noun declensions are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun declension to compare.</param>
        /// <param name="right">The second Russian noun declension to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianNounDeclension left, RussianNounDeclension right)
            => !(left == right);

        /// <summary>
        ///   <para>Returns a string representation of this Russian noun declension.</para>
        /// </summary>
        /// <returns>The string representation of this Russian noun declension.</returns>
        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
