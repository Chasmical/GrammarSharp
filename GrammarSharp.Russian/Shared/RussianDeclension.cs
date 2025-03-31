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

            // This setter is only used in RussianEndings.GetAdjectiveEndingIndices(), so there's no validation
            internal set => _types = (byte)((_types & 0xF0) | value);
        }
        public RussianDeclensionType Type
        {
            readonly get => (RussianDeclensionType)(_types >> 4);

            // This setter is only used in internal Normalize() method, so there's no validation
            private set => _types = (byte)((_types & 0x0F) | ((int)value << 4));
        }

        public readonly RussianStressPattern StressPattern => _stress;
        public readonly RussianDeclensionFlags Flags => (RussianDeclensionFlags)_flags;

        public RussianNounProperties? SpecialNounProperties
        {
            readonly get => _specialProps.ExtraData == 1 ? _specialProps.WithoutExtraData() : null;
            set
            {
                if (value.HasValue == (_specialProps.ExtraData == 1)) return;

                if (Type is not RussianDeclensionType.Unknown and not RussianDeclensionType.Noun)
                    throw new InvalidOperationException("Cannot set adjective/pronoun declension's noun properties.");

                _specialProps = value.GetValueOrDefault();
                _specialProps.ExtraData = value.HasValue ? 1 : 0;
            }
        }
        public bool IsReflexiveAdjective
        {
            readonly get => _specialProps.ExtraData == 2;
            set
            {
                if (value == IsReflexiveAdjective) return;

                if (Type is not RussianDeclensionType.Unknown and not RussianDeclensionType.Adjective)
                    throw new InvalidOperationException("Cannot set noun/pronoun declension's adjective properties.");

                _specialProps = default;
                _specialProps.ExtraData = value ? 2 : 0;
            }
        }

        public readonly bool IsZero => StemType == 0;

        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stress, RussianDeclensionFlags flags)
        {
            if ((uint)type > (uint)RussianDeclensionType.Pronoun)
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(RussianDeclensionType));

            if (stemType == 0)
            {
                if (!stress.IsZero) throw new ArgumentException("Declension 0 cannot have a non-zero stress pattern.", nameof(stress));
                if (flags != 0) throw new ArgumentException("Declension 0 cannot have declension flags.", nameof(stress));
                _types = (byte)((int)type << 4);
            }
            else
            {
                ValidateStemType(stemType, type);
                _types = (byte)(stemType | ((int)type << 4));
                _stress = stress;
                _flags = (byte)flags;
            }
        }

        private static void ValidateStemType(
            int stemType, RussianDeclensionType type,
            [CallerArgumentExpression(nameof(stemType))] string? paramName = null
        )
        {
            // Stem type 8 is exclusive to nouns
            if ((uint)stemType > (type == RussianDeclensionType.Noun ? 8 : 7))
                Throw(stemType, type, paramName);

            static void Throw(int stemType, RussianDeclensionType type, string? paramName)
            {
                string msg = $"{stemType} is not a valid stem type for {type}.";
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
                    return RussianNoun.ExtractStem(word);

                case RussianDeclensionType.Adjective:
                    return RussianAdjective.ExtractStem(word, out isAdjReflexive);

                default:
                    throw new InvalidOperationException("Cannot extract stem using an unknown declension.");
            }
        }

        internal void RemovePluraleTantum()
            => _specialProps.IsPluraleTantum = false;

        internal void Normalize(RussianDeclensionType defaultType, string paramName)
        {
            if (IsZero)
            {
                Type = defaultType;
                return;
            }

            RussianDeclensionType type = Type;
            if (type == RussianDeclensionType.Unknown)
            {
                Type = type = defaultType;
                ValidateStemType(StemType, type, paramName);
            }
            _stress.NormalizeMut(type, paramName);
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
