using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian declension, according to Zaliznyak's classification, consisting of: declension type, stem type, stress pattern, declension flags, and, optionally, special noun declension properties.</para>
    /// </summary>
    public partial struct RussianDeclension : IEquatable<RussianDeclension>
    {
        // Representation (_field1):
        //   11_xxxxxx - declension type (see all Russian[…]Declension types)
        //   xx_111111 - < type-specific data >
        // Representation (_field2):
        //   1111_1111 - < type-specific data >
        // Representation (_field3):
        //   1111_1111 - < type-specific data >
        //
        private byte _field1;
        [UsedImplicitly] private readonly byte _field2;
        [UsedImplicitly] private readonly byte _field3;

        // TODO: what about this implementation of IsZero?
        // public readonly bool IsZero => (_field1 & 0x3F) == 0 && _field2 == 0 && _field3 == 0;

        public readonly bool IsZero => Type switch
        {
            RussianDeclensionType.Noun => this.AsNounUnsafeRef().IsZero,
            RussianDeclensionType.Adjective => this.AsAdjectiveUnsafeRef().IsZero,
            RussianDeclensionType.Pronoun => this.AsPronounUnsafeRef().IsZero,
            _ => throw new InvalidOperationException(),
        };

        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_field1 >> 6);
            private set => _field1 = (byte)((_field1 & 0x3F) | ((int)value << 6));
        }

        [Pure] public static implicit operator RussianDeclension(RussianNounDeclension declension)
            => Unsafe.As<RussianNounDeclension, RussianDeclension>(ref declension);
        [Pure] public static implicit operator RussianDeclension(RussianAdjectiveDeclension declension)
        {
            var decl = Unsafe.As<RussianAdjectiveDeclension, RussianDeclension>(ref declension);
            // Ensure that the type marker is not lost (argument could have a default value)
            decl._field1 |= (int)RussianDeclensionType.Adjective << 6;
            return decl;
        }
        [Pure] public static implicit operator RussianDeclension(RussianPronounDeclension declension)
        {
            var decl = Unsafe.As<RussianPronounDeclension, RussianDeclension>(ref declension);
            // Ensure that the type marker is not lost (argument could have a default value)
            decl._field1 |= (int)RussianDeclensionType.Pronoun << 6;
            return decl;
        }

        public readonly bool TryAsNoun(out RussianNounDeclension nounDeclension)
        {
            nounDeclension = this.AsNounUnsafeRef();
            return Type == RussianDeclensionType.Noun;
        }
        public readonly bool TryAsAdjective(out RussianAdjectiveDeclension adjectiveDeclension)
        {
            adjectiveDeclension = this.AsAdjectiveUnsafeRef();
            return Type == RussianDeclensionType.Adjective;
        }
        public readonly bool TryAsPronoun(out RussianPronounDeclension pronounDeclension)
        {
            pronounDeclension = this.AsPronounUnsafeRef();
            return Type == RussianDeclensionType.Pronoun;
        }

#pragma warning disable IDE0251 // Make member 'readonly' — method uses mutating AsAdjectiveUnsafeRef() method
        public string ExtractStem(string word)
#pragma warning restore IDE0251
        {
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Adjective:
                    string stem = RussianAdjective.ExtractStem(word, out bool isAdjReflexive);
                    if (isAdjReflexive) this.AsAdjectiveUnsafeRef().IsReflexive = true;
                    return stem;

                default: // case RussianDeclensionType.Noun or RussianDeclensionType.Pronoun:
                    return RussianNoun.ExtractStem(word);
            }
        }

        public readonly bool Equals(RussianDeclension other)
            => _field1 == other._field1 && _field2 == other._field2 && _field3 == other._field3;
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianDeclension other && Equals(other);
        public readonly override int GetHashCode()
            => HashCode.Combine(_field1, _field2, _field3);

        [Pure] public static bool operator ==(RussianDeclension left, RussianDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianDeclension left, RussianDeclension right)
            => !(left == right);

    }
}
