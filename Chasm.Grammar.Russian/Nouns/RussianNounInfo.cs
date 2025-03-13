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
            Properties = properties;
            Declension = declension;
        }

        [Pure] internal readonly RussianNounProperties PrepareForDeclension(RussianCase @case, bool plural)
        {
            var props = Declension.SpecialNounProperties ?? Properties;
            return props.PrepareForDeclension(@case, plural);
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
