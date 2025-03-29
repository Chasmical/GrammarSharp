using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GrammarSharp.English
{
    public sealed class EnglishPronoun : IEquatable<EnglishPronoun>
    {
        public string Subjective { get; }
        public string Objective { get; }
        public string Reflexive { get; }
        public string PossessiveDeterminer { get; }
        public string PossessivePronoun { get; }

        public EnglishConjugationType ConjugationType { get; }
        public bool IsPlural => ConjugationType == EnglishConjugationType.PluralAre;

        public EnglishPronoun(
            string subjective, string objective, string reflexive,
            string possessiveDeterminer, string possessivePronoun, EnglishConjugationType conjugationType
        )
        {
            Subjective = subjective;
            Objective = objective;
            Reflexive = reflexive;
            PossessiveDeterminer = possessiveDeterminer;
            PossessivePronoun = possessivePronoun;
            ConjugationType = conjugationType;
        }

        public static EnglishPronoun FirstSingular { get; }
            = new("I", "me", "myself", "my", "mine", EnglishConjugationType.SingularAm);
        public static EnglishPronoun FirstPlural { get; }
            = new("we", "us", "ourselves", "our", "ours", EnglishConjugationType.PluralAre);

        public static EnglishPronoun SecondSingular { get; }
            = new("you", "you", "yourself", "your", "yours", EnglishConjugationType.SingularAre);
        public static EnglishPronoun SecondPlural { get; }
            = new("you", "you", "yourselves", "your", "yours", EnglishConjugationType.PluralAre);

        public static EnglishPronoun ThirdSingular { get; }
            = new("they", "them", "themself", "their", "theirs", EnglishConjugationType.SingularAre);
        public static EnglishPronoun ThirdPlural { get; }
            = new("they", "them", "themselves", "their", "theirs", EnglishConjugationType.PluralAre);
        public static EnglishPronoun ThirdNeuter { get; }
            = new("it", "it", "itself", "its", "its", EnglishConjugationType.SingularIs);
        public static EnglishPronoun ThirdMasculine { get; }
            = new("he", "him", "himself", "his", "his", EnglishConjugationType.SingularIs);
        public static EnglishPronoun ThirdFeminine { get; }
            = new("she", "her", "herself", "her", "hers", EnglishConjugationType.SingularIs);

        [Pure] public string Decline(EnglishCase @case)
        {
            return @case switch
            {
                EnglishCase.Subjective => Subjective,
                EnglishCase.Objective => Objective,
                EnglishCase.PossessiveDeterminer => PossessiveDeterminer,
                EnglishCase.PossessivePronoun => PossessivePronoun,
                _ => throw new InvalidEnumArgumentException(nameof(@case), (int)@case, typeof(EnglishCase)),
            };
        }

        [Pure] public override string ToString()
            => $"{Subjective}/{Objective}";
        [Pure] public string ToStringFull()
        {
            string type = ConjugationType switch
            {
                EnglishConjugationType.SingularIs => "is",
                EnglishConjugationType.SingularAre => "sg. are",
                EnglishConjugationType.SingularAm => "am",
                _ /* EnglishConjugationType.PluralAre */ => "pl. are",
            };
            return $"{Subjective}, {Objective}, {Reflexive}, {PossessiveDeterminer}, {PossessivePronoun} ({type})";
        }

        [Pure] public bool Equals([NotNullWhen(true)] EnglishPronoun? other)
        {
            if (ReferenceEquals(this, other)) return true;

            return other is not null &&
                   Subjective == other.Subjective &&
                   Objective == other.Objective &&
                   Reflexive == other.Reflexive &&
                   PossessiveDeterminer == other.PossessiveDeterminer &&
                   PossessivePronoun == other.PossessivePronoun;
        }
        [Pure] public override bool Equals([NotNullWhen(true)] object? obj)
            => Equals(obj as EnglishPronoun);
        [Pure] public override int GetHashCode()
            => HashCode.Combine(Subjective, Objective, Reflexive, PossessiveDeterminer, PossessivePronoun);

        [Pure] public static bool operator ==(EnglishPronoun? left, EnglishPronoun? right)
            => left is null ? right is null : left.Equals(right);
        [Pure] public static bool operator !=(EnglishPronoun? left, EnglishPronoun? right)
            => !(left == right);

    }
}
