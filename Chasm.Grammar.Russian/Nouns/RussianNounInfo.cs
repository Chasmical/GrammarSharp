using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianNounInfo : IEquatable<RussianNounInfo>
    {
        public RussianNounProperties Properties;
        public RussianDeclension Declension;

        public RussianNounInfo(RussianNounProperties properties, RussianDeclension declension)
        {
            if (declension.Type is not RussianDeclensionType.Noun and not RussianDeclensionType.Adjective)
            {
                if (declension.IsZero)
                    declension = new(RussianDeclensionType.Noun, 0, default, 0);
                else
                    throw new ArgumentException($"Declension {declension} is not valid for nouns.", nameof(declension));
            }
            Properties = properties;
            Declension = declension;
        }

        [Pure] public readonly bool Equals(RussianNounInfo other)
            => Declension.Equals(other.Declension) && Properties.Equals(other.Properties);
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounInfo other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(Properties.GetHashCode(), Declension.GetHashCode());

        [Pure] public static bool operator ==(RussianNounInfo left, RussianNounInfo right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianNounInfo left, RussianNounInfo right)
            => !(left == right);

    }
}
