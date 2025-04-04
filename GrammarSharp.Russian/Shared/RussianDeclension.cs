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
        private byte _field1;
        private byte _field2;
        private byte _field3;

        public readonly bool IsZero => Type switch
        {
            RussianDeclensionType.Noun => ForNounUnsafe().IsZero,
            RussianDeclensionType.Adjective => ForAdjectiveUnsafe().IsZero,
            // TODO: use pronoun struct for IsZero?
            _ => ForAdjectiveUnsafe().IsZero,
        };

        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_field1 >> 6);
            private set => _field1 = (byte)((_field1 & 0x3F) | ((int)value << 6));
        }

        [Pure] internal readonly RussianNounDeclension ForNounUnsafe()
            => Unsafe.As<RussianDeclension, RussianNounDeclension>(ref Unsafe.AsRef(in this));
        [Pure] internal readonly RussianAdjectiveDeclension ForAdjectiveUnsafe()
            => Unsafe.As<RussianDeclension, RussianAdjectiveDeclension>(ref Unsafe.AsRef(in this));
        // TODO: ForPronounUnsafe()
        // TODO: ForPronounAdjectiveUnsafe()

        [Pure] public static implicit operator RussianDeclension(RussianNounDeclension declension)
            => Unsafe.As<RussianNounDeclension, RussianDeclension>(ref declension);
        [Pure] public static implicit operator RussianDeclension(RussianAdjectiveDeclension declension)
        {
            var decl = Unsafe.As<RussianAdjectiveDeclension, RussianDeclension>(ref declension);
            // Ensure that the type marker is not lost (argument could have a default value)
            decl.Type = RussianDeclensionType.Adjective;
            return decl;
        }
        // TODO: implicit from PronounDeclension
        // TODO: implicit from PronounAdjectiveDeclension

        public string ExtractStem(string word)
        {
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Noun or RussianDeclensionType.Pronoun:
                    return RussianNoun.ExtractStem(word);

                case RussianDeclensionType.Adjective:
                    string res = RussianAdjective.ExtractStem(word, out bool isAdjReflexive);
                    if (isAdjReflexive)
                    {
                        ref var self = ref Unsafe.As<RussianDeclension, RussianAdjectiveDeclension>(ref this);
                        self.IsReflexive = true;
                    }
                    return res;

                default:
                    throw new NotImplementedException("pro-adj declension: extract stem");
            }
        }

        // TODO: get rid of RemovePluraleTantum?
        internal void RemovePluraleTantum()
        {
            if (Type == RussianDeclensionType.Noun)
            {
                ref var forNoun = ref Unsafe.As<RussianDeclension, RussianNounDeclension>(ref this);
                forNoun.RemovePluraleTantum();
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
