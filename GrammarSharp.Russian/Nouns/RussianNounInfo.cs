using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents information about a Russian noun, according to Zaliznyak's classification.</para>
    /// </summary>
    public partial struct RussianNounInfo : IEquatable<RussianNounInfo>
    {
        private RussianNounProperties _properties;
        // Note: access needed by RussianNoun ctor, to set IsReflexive when extracting adjective stem
        internal RussianDeclension _declension;

        /// <summary>
        ///   <para>Gets or sets the noun's properties.</para>
        /// </summary>
        public RussianNounProperties Properties
        {
            readonly get => _properties;
            // Note: RussianNounProperties are never invalid in a public-facing way
            set => _properties = value;
        }
        /// <summary>
        ///   <para>Gets or sets the noun's declension.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a noun or adjective declension.</exception>
        public RussianDeclension Declension
        {
            readonly get => _declension;
            set
            {
                if (value.IsZero) value = default(RussianNounDeclension);
                ValidateDeclension(value);
                _declension = value;
            }
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNounInfo"/> structure with the specified <paramref name="properties"/> and <paramref name="declension"/>.</para>
        /// </summary>
        /// <param name="properties">The noun's properties.</param>
        /// <param name="declension">The noun's declension.</param>
        /// <exception cref="ArgumentException"><paramref name="declension"/> is not a noun or adjective declension.</exception>
        public RussianNounInfo(RussianNounProperties properties, RussianDeclension declension)
        {
            if (declension.IsZero) declension = default(RussianNounDeclension);
            ValidateDeclension(declension);
            _properties = properties;
            _declension = declension;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianNounInfo"/> structure from the specified <paramref name="nounInfo"/> string.</para>
        /// </summary>
        /// <param name="nounInfo">The string containing Russian noun info to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="nounInfo"/> is not valid Russian noun info.</exception>
        public RussianNounInfo(string nounInfo)
            => this = Parse(nounInfo);
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianNounInfo"/> structure from the specified <paramref name="nounInfo"/> span.</para>
        /// </summary>
        /// <param name="nounInfo">The read-only span of characters containing Russian noun info to convert.</param>
        /// <exception cref="ArgumentException"><paramref name="nounInfo"/> is not valid Russian noun info.</exception>
        public RussianNounInfo(ReadOnlySpan<char> nounInfo)
            => this = Parse(nounInfo);

        private static void ValidateDeclension(RussianDeclension declension, [CAE(nameof(declension))] string? paramName = null)
        {
            if (declension.Type is not RussianDeclensionType.Noun and not RussianDeclensionType.Adjective)
                Throw(declension, paramName);

            static void Throw(RussianDeclension declension, string? paramName)
                => throw new ArgumentException($"Nouns cannot have a declension of type {declension.Type}.", paramName);
        }

        /// <summary>
        ///   <para>Determines whether this Russian noun info is equal to another specified Russian noun info.</para>
        /// </summary>
        /// <param name="other">The Russian noun info to compare with this Russian noun info.</param>
        /// <returns><see langword="true"/>, if this Russian noun info is equal to <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly bool Equals(RussianNounInfo other)
            => _properties.Equals(other._properties) && _declension.Equals(other._declension);
        /// <summary>
        ///   <para>Determines whether this Russian noun info is equal to the specified <paramref name="obj"/>.</para>
        /// </summary>
        /// <param name="obj">The object to compare with this Russian noun info.</param>
        /// <returns><see langword="true"/>, if <paramref name="obj"/> is a <see cref="RussianNounInfo"/> instance equal to this Russian noun info; otherwise, <see langword="false"/>.</returns>
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounInfo other && Equals(other);
        /// <summary>
        ///   <para>Returns a hash code for this Russian noun info.</para>
        /// </summary>
        /// <returns>A hash code for this Russian noun info.</returns>
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_properties, _declension);

        /// <summary>
        ///   <para>Determines whether the two specified Russian noun infos are equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun info to compare.</param>
        /// <param name="right">The second Russian noun info to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator ==(RussianNounInfo left, RussianNounInfo right)
            => left.Equals(right);
        /// <summary>
        ///   <para>Determines whether the two specified Russian noun infos are not equal.</para>
        /// </summary>
        /// <param name="left">The first Russian noun info to compare.</param>
        /// <param name="right">The second Russian noun info to compare.</param>
        /// <returns><see langword="true"/>, if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, <see langword="false"/>.</returns>
        [Pure] public static bool operator !=(RussianNounInfo left, RussianNounInfo right)
            => !(left == right);

    }
}
