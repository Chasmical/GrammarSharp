using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        public RussianDeclension Declension;
        public bool IsReflexive;

        public RussianAdjectiveInfo(RussianDeclension declension)
        {
            if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
            {
                if (declension.IsZero)
                    declension = new(RussianDeclensionType.Adjective, 0, default, 0);
                else
                    throw new ArgumentException($"Declension {declension} is not valid for adjectives.", nameof(declension));
            }
            Declension = declension;
            IsReflexive = declension.IsReflexiveAdjective;
        }

        [Pure] public readonly bool Equals(RussianAdjectiveInfo other)
            => Declension.Equals(other.Declension);
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveInfo other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => Declension.GetHashCode();

        [Pure] public static bool operator ==(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => !(left == right);

    }
}
