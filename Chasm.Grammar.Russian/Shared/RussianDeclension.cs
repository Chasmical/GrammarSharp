using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianDeclension : IEquatable<RussianDeclension>
    {
        // Representation (_types field):
        //   xxxx_1111 - stem type       (0000 - 0, 1000 - 8)
        //   1111_xxxx - declension type (see RussianDeclensionType enum)
        //
        // Representation (_stress field):
        //   1111_1111 - see RussianStressPattern struct
        //
        // Representation (_flags field):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        // Representation (_nounProps field):
        //   1111_1111 - see RussianNounProperties struct
        //
        //   xxxx_x111 - special noun gender and animacy
        //   111x_xxxx - 'special noun props are present' flag
        //
        private byte _types;
        private RussianStressPattern _stress;
        private byte _flags;
        private RussianNounProperties _nounProps;

        public int StemType
        {
            readonly get => _types & 0x0F;
            set
            {
                if ((uint)value > 8)
                    throw new ArgumentOutOfRangeException(nameof(value), value, ""); // TODO: exception
                _types = (byte)((_types & 0xF0) | value);
            }
        }
        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_types >> 4);
            set
            {
                if ((uint)value > (uint)RussianDeclensionType.Pronoun)
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(RussianDeclensionType));
                _types = (byte)((_types & 0x0F) | ((int)value << 4));
            }
        }
        public RussianStressPattern StressPattern
        {
            readonly get => _stress;
            set => _stress = value;
        }
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_flags;
            set => _flags = (byte)value;
        }
        public RussianNounProperties? SpecialNounProperties
        {
            // This property uses the field's ExtraData as a non-null value flag
            readonly get => _nounProps.ExtraData != 0 ? _nounProps : null;
            set
            {
                _nounProps = value.GetValueOrDefault();
                _nounProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }

        public readonly bool IsZero => StemType == 0;

        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            if ((uint)stemType > 8)
                throw new ArgumentOutOfRangeException(nameof(stemType), stemType, ""); // TODO: exception

            stress = type switch
            {
                RussianDeclensionType.Noun => stress.NormalizeForNoun(nameof(stress)),
                RussianDeclensionType.Adjective => stress.NormalizeForAdjective(nameof(stress)),
                RussianDeclensionType.Pronoun => stress.NormalizeForPronoun(nameof(stress)),
                _ => throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(RussianDeclensionType)),
            };

            _types = (byte)(stemType | ((int)type << 4));
            _stress = stress;
            _flags = (byte)flags;
        }

        [Pure] public readonly bool Equals(RussianDeclension other)
        {
            // Nothing special is needed here, so just perform bitwise equality
            return Unsafe.As<RussianDeclension, int>(ref Unsafe.AsRef(in this)) == Unsafe.As<RussianDeclension, int>(ref other);
        }
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianDeclension other && Equals(other);
        [Pure] public readonly override int GetHashCode()
        {
            // Note: this hash code may not be consistent across machines
            return Unsafe.As<RussianDeclension, int>(ref Unsafe.AsRef(in this));
        }

        [Pure] public static bool operator ==(RussianDeclension left, RussianDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianDeclension left, RussianDeclension right)
            => !(left == right);

    }
}
