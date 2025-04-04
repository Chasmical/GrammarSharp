using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public struct RussianNounDeclension : IEquatable<RussianNounDeclension>
    {
        // Representation (_typeAndProps):
        //   00_x_xxxxx - noun declension type constant
        //   xx_1_xxxxx - noun has special declension properties
        //   xx_x_11111 - declension gender, animacy and tantums (see RussianNounProperties enum)
        // Representation (_stemTypeAndStress):
        //   xxxx_1111 - stem type
        //   1111_xxxx - stress pattern
        // Representation (_declensionFlags):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        private RussianNounProperties _typeAndProps;
        private byte _stemTypeAndStress;
        private byte _declensionFlags;

        public int StemType
        {
            readonly get => _stemTypeAndStress & 0x0F;
            set
            {
                ValidateStemType(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0xF0) | value);
            }
        }
        public RussianStress Stress
        {
            readonly get => (RussianStress)(_stemTypeAndStress >> 4);
            set
            {
                ValidateStress(value);
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0x0F) | ((int)value << 4));
            }
        }
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_declensionFlags;
            set
            {
                ValidateFlags(value);
                _declensionFlags = (byte)value;
            }
        }

        public RussianNounProperties? SpecialProperties
        {
            readonly get => _typeAndProps.ExtraData != 0 ? _typeAndProps.WithoutExtraData() : null;
            set
            {
                _typeAndProps = value.GetValueOrDefault();
                _typeAndProps.StripTantumsAndSetBoolExtraData(value.HasValue);
            }
        }

        public readonly bool IsZero => StemType == 0;

        public RussianNounDeclension(int stemType, RussianStress stress)
        {
            ValidateStemType(stemType);
            ValidateStress(stress);
            _stemTypeAndStress = (byte)(stemType | ((int)stress << 4));
        }
        public RussianNounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags)
            : this(stemType, stress)
        {
            ValidateFlags(flags);
            _declensionFlags = (byte)flags;
        }
        public RussianNounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags, RussianNounProperties? specialProps)
            : this(stemType, stress, flags)
        {
            SpecialProperties = specialProps;
        }

        private static void ValidateStemType(int stemType, [CAE(nameof(stemType))] string? paramName = null)
        {
            // Nouns can have 0 through 8 stem types (8 is exclusive to nouns)
            if ((uint)stemType > 8u)
                Throw(stemType, paramName);

            static void Throw(int stemType, string? paramName)
                => throw new ArgumentOutOfRangeException(paramName, stemType, $"{stemType} is not a valid stem type for nouns.");
        }

        private static void ValidateStress(RussianStress stress, [CAE(nameof(stress))] string? paramName = null)
        {
            // Nouns can only have 0, a through f, b′, d′, f′ and f″ stress schemas
            if ((uint)stress > (uint)RussianStress.Fpp || (uint)stress > (uint)RussianStress.F && ((uint)stress & 1) != 0)
                Throw(stress, paramName);

            static void Throw(RussianStress stress, string? paramName)
                => throw new ArgumentException($"{stress} is not a valid stress schema for nouns.", paramName);
        }

        private static void ValidateFlags(RussianDeclensionFlags flags, [CAE(nameof(flags))] string? paramName = null)
        {
            // Nouns can have all the flags in the enumeration
            const uint validFlags = ((uint)RussianDeclensionFlags.AlternatingYo << 1) - 1;
            if ((uint)flags > validFlags) Throw(flags, paramName);

            static void Throw(RussianDeclensionFlags flags, string? paramName)
            {
                const RussianDeclensionFlags invalidFlags = unchecked((RussianDeclensionFlags)~validFlags);
                throw new ArgumentException($"{flags & invalidFlags} is not a valid declension flag for nouns.", paramName);
            }
        }

        [Pure] public readonly bool Equals(RussianNounDeclension other)
            => _typeAndProps.Equals(other._typeAndProps) &&
               _stemTypeAndStress == other._stemTypeAndStress &&
               _declensionFlags == other._declensionFlags;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianNounDeclension other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_typeAndProps, _stemTypeAndStress, _declensionFlags);

        [Pure] public static bool operator ==(RussianNounDeclension left, RussianNounDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianNounDeclension left, RussianNounDeclension right)
            => !(left == right);

        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
