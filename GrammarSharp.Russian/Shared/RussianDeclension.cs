using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
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
        // Representation (_specialProps field):
        //   111_11111 - see RussianNounProperties struct
        //
        //   xxx_11111 - special noun gender, animacy and tantums
        //   001_xxxxx - 'is noun and has special declension properties' flag
        //   010_xxxxx - 'is adjective and is reflexive' flag
        //
        private byte _types;
        private RussianStressPattern _stress;
        private byte _flags;
        private RussianNounProperties _specialProps;

        public int StemType
        {
            readonly get => _types & 0x0F;
            set
            {
                ValidateStemType(Type, value);
                _types = (byte)((_types & 0xF0) | value);
            }
        }
        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_types >> 4);
            // Set only in internal Normalize() method, so there's no validation
            private set => _types = (byte)((_types & 0x0F) | ((int)value << 4));
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
            readonly get => _specialProps.ExtraData == 1 ? _specialProps.WithoutExtraData() : null;
            set
            {
                _specialProps = value.GetValueOrDefault();
                if (value.HasValue && Type > RussianDeclensionType.Noun)
                    throw new InvalidOperationException("Cannot set adjective/pronoun declension's noun properties.");
                _specialProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }
        public bool IsReflexiveAdjective
        {
            readonly get => _specialProps.ExtraData == 2;
            set
            {
                if (value && Type is not RussianDeclensionType.Unknown and not RussianDeclensionType.Adjective)
                    throw new InvalidOperationException("Cannot set noun/pronoun declension's adjective properties.");
                _specialProps = default;
                _specialProps.ExtraData = value ? 2 : 0;
            }
        }

        public readonly bool IsZero => StemType == 0;

        private RussianDeclension(RussianDeclensionType type)
            => _types = (byte)((int)type << 4);

        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            if ((uint)type > (uint)RussianDeclensionType.Pronoun)
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(RussianDeclensionType));

            ValidateStemType(type, stemType);

            _types = (byte)(stemType | ((int)type << 4));

            if (stemType == 0)
            {
                if (!stress.IsZero) throw new ArgumentException("0 declension cannot have a non-zero stress pattern.", nameof(stress));
                if (flags != 0) throw new ArgumentException("0 declension cannot have declension flags.", nameof(stress));
            }
            else
            {
                _stress = stress;
                _flags = (byte)flags;
            }
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
        {
            if (IsZero)
            {
                isAdjReflexive = false;
                return word;
            }
            return ExtractStem(word.AsSpan(), out isAdjReflexive).ToString();
        }
        [Pure] public readonly ReadOnlySpan<char> ExtractStem(ReadOnlySpan<char> word, out bool isAdjReflexive)
        {
            isAdjReflexive = false;
            if (IsZero) return word;

            switch (Type)
            {
                case RussianDeclensionType.Noun or RussianDeclensionType.Pronoun:
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
            => _specialProps.IsPluraleTantum = false;

        internal void Normalize(RussianDeclensionType defaultType)
        {
            if (IsZero)
            {
                Type = defaultType;
                return;
            }
            if (Type == RussianDeclensionType.Unknown) Type = defaultType;
            _stress.Normalize(Type, nameof(defaultType));
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
