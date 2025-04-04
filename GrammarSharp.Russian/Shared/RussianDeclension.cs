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

        public readonly bool IsZero => _field1 == 0 && _field2 == 0 && _field3 == 0;

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
