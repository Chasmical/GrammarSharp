using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        public RussianDeclension Declension;
        public RussianAdjectiveFlags Flags;

        public RussianAdjectiveInfo(RussianDeclension declension)
            : this(declension, declension.IsReflexiveAdjective ? RussianAdjectiveFlags.IsReflexive : 0) { }

        public RussianAdjectiveInfo(RussianDeclension declension, RussianAdjectiveFlags flags)
        {
            declension.Normalize(RussianDeclensionType.Adjective);

            if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
                throw new ArgumentException($"Declension {declension} is not valid for adjectives.", nameof(declension));
            if (flags != 0 && declension.Type != RussianDeclensionType.Adjective)
                throw new ArgumentException("Only pure adjectives (with adjective declension) can have flags.", nameof(flags));

            Declension = declension;
            Flags = flags;
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
