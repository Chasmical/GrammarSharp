using System;
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
                ValidateStemType(Type, value);
                _types = (byte)((_types & 0xF0) | value);
            }
        }
        public readonly RussianDeclensionType Type => (RussianDeclensionType)(_types >> 4);
        public RussianStressPattern StressPattern
        {
            readonly get => _stress;
            set
            {
                value.Normalize(Type, nameof(value));
                _stress = value;
            }
        }
        public RussianDeclensionFlags Flags
        {
            readonly get => (RussianDeclensionFlags)_flags;
            set => _flags = (byte)value;
        }

        public RussianNounProperties? SpecialNounProperties
        {
            // This property uses the field's ExtraData as a non-null value flag
            readonly get => _nounProps.ExtraData == 1 ? _nounProps : null;
            set
            {
                _nounProps = value.GetValueOrDefault();
                _nounProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }
        public bool IsReflexiveAdjective
        {
            readonly get => _nounProps.ExtraData == 2;
            set => _nounProps.ExtraData = value ? 2 : 0;
        }

        public readonly bool IsZero => StemType == 0;

        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            ValidateStemType(type, stemType);
            stress.Normalize(type, nameof(stress));

            _types = (byte)(stemType | ((int)type << 4));
            _stress = stress;
            _flags = (byte)flags;
        }

        private static void ValidateStemType(
            RussianDeclensionType type, int stemType,
            [CallerArgumentExpression(nameof(stemType))] string? paramName = null
        )
        {
            // Stem type 8 is exclusive to nouns
            if ((uint)stemType > (type == RussianDeclensionType.Noun ? 8 : 7))
                Throw(stemType, type, paramName);

            static void Throw(int stemType, RussianDeclensionType type, string? paramName)
            {
                string msg = stemType == 8
                    ? "Stem type 8 is only valid for noun declensions."
                    : $"Stem type {stemType} is not valid for {type} declension.";
                throw new ArgumentOutOfRangeException(paramName, stemType, msg);
            }
        }

        [Pure] public readonly string ExtractStem(string word, out bool isAdjReflexive)
            => ExtractStem(word.AsSpan(), out isAdjReflexive).ToString();
        [Pure] public readonly ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word, out bool isAdjReflexive)
        {
            isAdjReflexive = false;
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Noun:
                    // Remove the last vowel/'й'/'ь' to get the stem
                    return word.Length > 1 && RussianLowerCase.IsTrimNounStemChar(word[^1]) ? word[..^1] : word;

                case RussianDeclensionType.Adjective:
                    // If adjective ends with 'ся', remove the last four letters
                    if (word.Length > 4 && word[^2] == 'с' && word[^1] == 'я')
                    {
                        isAdjReflexive = true;
                        return word[..^4];
                    }
                    // Otherwise, remove just the last two letters
                    return word.Length > 2 ? word[..^2] : word;

                default:
                    throw new NotImplementedException();
            }
        }

        internal void RemovePluraleTantum()
            => _nounProps.IsPluraleTantum = false;

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
