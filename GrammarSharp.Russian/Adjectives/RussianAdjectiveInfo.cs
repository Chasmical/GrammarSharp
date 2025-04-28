using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents information about a Russian adjective, according to Zaliznyak's classification.</para>
    /// </summary>
    public partial struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        // Note: access needed by RussianAdjective ctor, to set IsReflexive when extracting adjective stem
        internal RussianDeclension _declension;
        private byte _flags;

        /// <summary>
        ///   <para>Gets or sets the adjective's declension.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not an adjective or pronoun declension.</exception>
        public RussianDeclension Declension
        {
            readonly get => _declension;
            set
            {
                if (value.IsZero) value = default(RussianAdjectiveDeclension);
                ValidateDeclension(value);
                _declension = value;
            }
        }
        /// <summary>
        ///   <para>Gets or sets the adjective's flags.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid <seealso cref="RussianAdjectiveFlags"/> value.</exception>
        public RussianAdjectiveFlags Flags
        {
            readonly get => (RussianAdjectiveFlags)_flags;
            set
            {
                ValidateFlags(value);
                _flags = (byte)value;
            }
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjectiveInfo"/> structure with the specified <paramref name="declension"/>.</para>
        /// </summary>
        /// <param name="declension">The adjective's declension.</param>
        /// <exception cref="ArgumentException"><paramref name="declension"/> is not an adjective or pronoun declension.</exception>
        public RussianAdjectiveInfo(RussianDeclension declension)
            : this(declension, RussianAdjectiveFlags.None) { }
        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjectiveInfo"/> structure with the specified <paramref name="declension"/> and <paramref name="flags"/>.</para>
        /// </summary>
        /// <param name="declension">The adjective's declension.</param>
        /// <param name="flags">The adjective's flags.</param>
        /// <exception cref="ArgumentException"><paramref name="declension"/> is not an adjective or pronoun declension.</exception>
        /// <exception cref="ArgumentException"><paramref name="flags"/> is not a valid <seealso cref="RussianAdjectiveFlags"/> value.</exception>
        public RussianAdjectiveInfo(RussianDeclension declension, RussianAdjectiveFlags flags)
        {
            if (declension.IsZero) declension = default(RussianAdjectiveDeclension);
            ValidateDeclension(declension);
            ValidateFlags(flags);
            _declension = declension;
            _flags = (byte)flags;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianAdjectiveInfo"/> structure from the specified <paramref name="adjectiveInfo"/> string.</para>
        /// </summary>
        /// <param name="adjectiveInfo">The string containing Russian adjective info to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="adjectiveInfo"/> is not valid Russian adjective info.</exception>
        public RussianAdjectiveInfo(string adjectiveInfo)
            => this = Parse(adjectiveInfo);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianAdjectiveInfo"/> structure from the specified <paramref name="adjectiveInfo"/> span.</para>
        /// </summary>
        /// <param name="adjectiveInfo">The read-only span of characters containing Russian adjective info to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="adjectiveInfo"/> is not valid Russian adjective info.</exception>
        public RussianAdjectiveInfo(ReadOnlySpan<char> adjectiveInfo)
            => this = Parse(adjectiveInfo);

        private static void ValidateDeclension(RussianDeclension declension, [CAE(nameof(declension))] string? paramName = null)
        {
            if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
                Throw(declension, paramName);

            static void Throw(RussianDeclension declension, string? paramName)
                => throw new ArgumentException($"Adjectives cannot have a declension of type {declension.Type}.", paramName);
        }
        private static void ValidateFlags(RussianAdjectiveFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            const RussianAdjectiveFlags allFlags
                = RussianAdjectiveFlags.BoxedCross | RussianAdjectiveFlags.NoComparativeForm | RussianAdjectiveFlags.IsPronoun;

            if ((uint)flags > (uint)allFlags) Throw(flags, paramName);

            static void Throw(RussianAdjectiveFlags flags, string? paramName)
                => throw new ArgumentException($"{flags & ~allFlags} is not a valid flag for adjectives.", paramName);
        }

        /// <summary>
        ///   <para>Determines whether this Russian adjective info is equal to another specified Russian adjective info.</para>
        /// </summary>
        /// <param name="other">The Russian adjective info to compare with this Russian adjective info.</param>
        /// <returns><see langword="true"/>, if this Russian adjective info is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianAdjectiveInfo other)
            => _declension.Equals(other._declension) && _flags == other._flags;
        /// <summary>
        ///   <para>Determines whether this Russian adjective info is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian adjective info.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianAdjectiveInfo"/> instance equal to this Russian adjective info; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveInfo other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian adjective info.</para>
        /// </summary>
        /// <returns>A hash code for this Russian adjective info.</returns>
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_declension, _flags);

        /// <summary>
        ///   <para>Determines whether the two specified Russian adjective infos are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian adjective info to compare.</param>
        /// <param name="right">The second Russian adjective info to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian adjective infos are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian adjective info to compare.</param>
        /// <param name="right">The second Russian adjective info to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => !(left == right);

    }
}
