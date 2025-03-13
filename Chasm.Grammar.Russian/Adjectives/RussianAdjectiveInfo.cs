using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        public RussianDeclension Declension;

        public RussianAdjectiveInfo(RussianDeclension declension)
            => Declension = declension;

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
