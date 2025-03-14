﻿using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        public RussianDeclension Declension;
        public bool IsReflexive;

        public RussianAdjectiveInfo(RussianDeclension declension)
        {
            if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
            {
                if (declension.IsZero)
                    declension.Type = RussianDeclensionType.Adjective;
                else
                    throw new ArgumentException($"Declension {declension} is not valid for adjectives.", nameof(declension));
            }
            Declension = declension;
        }

        internal RussianAdjectiveInfo(RussianDeclension declension, RussianNounProperties nounProps)
        {
            Declension = declension;
            // RussianNoun treats ExtraData = 1 as "is reflexive" flag
            IsReflexive = nounProps.ExtraData != 0;
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
