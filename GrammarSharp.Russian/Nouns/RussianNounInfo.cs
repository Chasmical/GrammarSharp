using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianNounInfo : IEquatable<RussianNounInfo>
    {
        private RussianNounProperties _properties;
        // Note: access needed by RussianNoun ctor, to set IsReflexive when extracting adjective stem
        internal RussianDeclension _declension;

        public RussianNounProperties Properties
        {
            readonly get => _properties;
            // Note: RussianNounProperties are never invalid in a public-facing way
            set => _properties = value;
        }
        public RussianDeclension Declension
        {
            readonly get => _declension;
            set
            {
                ValidateDeclension(value);
                _declension = value;
            }
        }

        public RussianNounInfo(RussianNounProperties properties, RussianDeclension declension)
        {
            ValidateDeclension(declension);
            _properties = properties;
            _declension = declension;
        }

        private static void ValidateDeclension(RussianDeclension declension, [CAE(nameof(declension))] string? paramName = null)
        {
            if (declension.Type is not RussianDeclensionType.Noun and not RussianDeclensionType.Adjective)
                Throw(declension, paramName);

            static void Throw(RussianDeclension declension, string? paramName)
                => throw new ArgumentException($"Nouns cannot have a declension of type {declension.Type}.", paramName);
        }

        [Pure] public readonly bool Equals(RussianNounInfo other)
            => _properties.Equals(other._properties) && _declension.Equals(other._declension);
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounInfo other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_properties, _declension);

        [Pure] public static bool operator ==(RussianNounInfo left, RussianNounInfo right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianNounInfo left, RussianNounInfo right)
            => !(left == right);

    }
}
