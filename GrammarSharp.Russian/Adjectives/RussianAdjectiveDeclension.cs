using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public struct RussianAdjectiveDeclension
    {
        // Representation (_stemTypeAndReflexive):
        //   01_xx_xxxx - adjective declension type constant
        //   xx_x1_xxxx - whether adjective is reflexive
        //   xx_1x_xxxx - < UNUSED >
        //   xx_xx_1111 - stem type
        // Representation (_stressPattern):
        //   1111_1111 - see RussianStressPattern enum
        // Representation (_declensionFlags):
        //   1111_1111 - see RussianDeclensionFlags enum
        //
        private byte _typesAndReflexive;
        private RussianStressPattern _stressPattern;
        private byte _declensionFlags;

        public int StemType
        {
            readonly get => _typesAndReflexive & 0x0F;
            set
            {
                // TODO: validate
                _typesAndReflexive = (byte)((_typesAndReflexive & 0xF0) | value);
            }
        }
        public RussianStressPattern StressPattern
        {
            readonly get => _stressPattern;
            set => _stressPattern = value;
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

        public bool IsReflexive
        {
            readonly get => (_typesAndReflexive & 0x10) != 0;
            set => _typesAndReflexive = (byte)((_typesAndReflexive & 0xEF) | (value ? 0x10 : 0));
        }

        public readonly bool IsZero => StemType == 0;

        // TODO: more constructors
        public RussianAdjectiveDeclension(int stemType, RussianStressPattern stressPattern, RussianDeclensionFlags flags)
        {
            // TODO: validate parameters

            _typesAndReflexive = (byte)(0x40 | stemType);
            _stressPattern = stressPattern;
            _declensionFlags = (byte)flags;
        }

        [Pure] public readonly bool Equals(RussianAdjectiveDeclension other)
            => _typesAndReflexive.Equals(other._typesAndReflexive) &&
               _stressPattern.Equals(other._stressPattern) &&
               _declensionFlags == other._declensionFlags;
        [Pure] public readonly override bool Equals([NotNullWhen(true)] object? obj)
            => obj is RussianAdjectiveDeclension other && Equals(other);
        [Pure] public readonly override int GetHashCode()
            => HashCode.Combine(_typesAndReflexive, _stressPattern, _declensionFlags);

        [Pure] public static bool operator ==(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => left.Equals(right);
        [Pure] public static bool operator !=(RussianAdjectiveDeclension left, RussianAdjectiveDeclension right)
            => !(left == right);

        [Pure] public readonly override string ToString()
            => ((RussianDeclension)this).ToString();

    }
}
