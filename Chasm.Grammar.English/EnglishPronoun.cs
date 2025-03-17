using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.English
{
    public sealed class EnglishPronoun
    {
        public string Subjective { get; }
        public string Objective { get; }
        public string Reflexive { get; }
        public string PossessiveDeterminer { get; }
        public string PossessivePronoun { get; }

        // TODO: IsPlural (for conjugation)
        // TODO: PronounType? (to choose between is/are/am)

        public EnglishPronoun(string subjective, string objective, string reflexive, string possessiveDeterminer, string possessivePronoun)
        {
            Subjective = subjective;
            Objective = objective;
            Reflexive = reflexive;
            PossessiveDeterminer = possessiveDeterminer;
            PossessivePronoun = possessivePronoun;
        }

        public static EnglishPronoun FirstSingular { get; } = new("I", "me", "myself", "my", "mine");
        public static EnglishPronoun FirstPlural { get; } = new("we", "us", "ourselves", "our", "ours");

        public static EnglishPronoun SecondSingular { get; } = new("you", "you", "yourself", "your", "yours");
        public static EnglishPronoun SecondPlural { get; } = new("you", "you", "yourselves", "your", "yours");

        public static EnglishPronoun ThirdSingular { get; } = new("they", "them", "themself", "their", "theirs");
        public static EnglishPronoun ThirdPlural { get; } = new("they", "them", "themselves", "their", "theirs");
        public static EnglishPronoun ThirdNeuter { get; } = new("it", "it", "itself", "its", "its");
        public static EnglishPronoun ThirdMasculine { get; } = new("he", "him", "himself", "his", "his");
        public static EnglishPronoun ThirdFeminine { get; } = new("she", "her", "herself", "her", "hers");

        [Pure] public string Decline()
        {
            throw new NotImplementedException();
        }

        [Pure] public override string ToString()
            => $"{Subjective}/{Objective}";

    }
}
