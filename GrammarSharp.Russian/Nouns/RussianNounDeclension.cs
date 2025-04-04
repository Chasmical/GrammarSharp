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
                // TODO: validate
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0xF0) | value);
            }
        }
        public RussianStress Stress
        {
            readonly get => (RussianStress)(_stemTypeAndStress >> 4);
            set
            {
                // TODO: validate
                _stemTypeAndStress = (byte)((_stemTypeAndStress & 0x0F) | ((int)value << 4));
            }
        }
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_declensionFlags;
            set
            {
                // TODO: validate
                _declensionFlags = (byte)value;
            }
        }

        public RussianNounProperties? SpecialProperties
        {
            readonly get => _typeAndProps.ExtraData != 0 ? _typeAndProps.WithoutExtraData() : null;
            set
            {
                _typeAndProps = value.GetValueOrDefault();
                _typeAndProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }

        public readonly bool IsZero => StemType == 0;

        // TODO: more constructors
        public RussianNounDeclension(int stemType, RussianStress stress, RussianDeclensionFlags flags, RussianNounProperties? specialProps)
        {
            // TODO: validate parameters

            SpecialProperties = specialProps;
            _stemTypeAndStress = (byte)(stemType | ((int)stress << 4));
            _declensionFlags = (byte)flags;
        }

        // TODO: get rid of RemovePluraleTantum?
        internal void RemovePluraleTantum()
            => _typeAndProps.IsPluraleTantum = false;
        internal void CopyTantumsFrom(RussianNounProperties props)
            => _typeAndProps.CopyTantumsFrom(props);

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
