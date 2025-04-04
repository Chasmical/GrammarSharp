using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianAdjectiveInfo : IEquatable<RussianAdjectiveInfo>
    {
        // Note: access needed by RussianAdjective ctor, to set IsReflexive when extracting adjective stem
        internal RussianDeclension _declension;
        private RussianAdjectiveFlags _flags;

        public RussianDeclension Declension
        {
            readonly get => _declension;
            set
            {
                ValidateDeclension(value);
                _declension = value;
            }
        }
        public RussianAdjectiveFlags Flags
        {
            readonly get => _flags;
            set
            {
                ValidateFlags(value);
                _flags = value;
            }
        }

        public RussianAdjectiveInfo(RussianDeclension declension)
            : this(declension, RussianAdjectiveFlags.None) { }
        public RussianAdjectiveInfo(RussianDeclension declension, RussianAdjectiveFlags flags)
        {
            ValidateDeclension(declension);
            ValidateFlags(flags);
            _declension = declension;
            _flags = flags;
        }

        private static void ValidateDeclension(RussianDeclension declension, [CAE(nameof(declension))] string? paramName = null)
        {
            if (declension.Type is not RussianDeclensionType.Adjective and not RussianDeclensionType.Pronoun)
                Throw(declension, paramName);

            static void Throw(RussianDeclension declension, string? paramName)
                => throw new ArgumentException($"Adjectives cannot have a declension of type {declension.Type}.", paramName);
        }

        private static void ValidateFlags(RussianAdjectiveFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            const RussianAdjectiveFlags allFlags = RussianAdjectiveFlags.BoxedCross | RussianAdjectiveFlags.NoComparativeForm;
            if ((uint)flags > (uint)allFlags) Throw(flags, paramName);

            static void Throw(RussianAdjectiveFlags flags, string? paramName)
                => throw new ArgumentException($"{flags & ~allFlags} is not a valid flag for adjectives.", paramName);
        }

        [Pure] public readonly bool Equals(RussianAdjectiveInfo other)
            => _declension.Equals(other._declension) && _flags == other._flags;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveInfo other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_declension, _flags);

        [Pure] public static bool operator ==(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianAdjectiveInfo left, RussianAdjectiveInfo right)
            => !(left == right);

    }
}
