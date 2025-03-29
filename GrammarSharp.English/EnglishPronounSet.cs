using JetBrains.Annotations;

namespace GrammarSharp.English
{
    public struct EnglishPronounSet
    {
        // TODO: null pronouns on default???
        public EnglishPronoun FirstPerson;
        public EnglishPronoun SecondPerson;
        public EnglishPronoun ThirdPerson;

        public EnglishPronounSet(EnglishPronoun thirdPerson)
            : this(EnglishPronoun.FirstSingular, EnglishPronoun.SecondSingular, thirdPerson) { }
        public EnglishPronounSet(EnglishPronoun firstPerson, EnglishPronoun secondPerson, EnglishPronoun thirdPerson)
        {
            FirstPerson = firstPerson;
            SecondPerson = secondPerson;
            ThirdPerson = thirdPerson;
        }

        public static EnglishPronounSet Singular { get; } = new(EnglishPronoun.ThirdSingular);
        public static EnglishPronounSet Plural { get; }
            = new(EnglishPronoun.FirstPlural, EnglishPronoun.SecondPlural, EnglishPronoun.ThirdPlural);
        public static EnglishPronounSet Neuter { get; } = new(EnglishPronoun.ThirdNeuter);
        public static EnglishPronounSet Masculine { get; } = new(EnglishPronoun.ThirdMasculine);
        public static EnglishPronounSet Feminine { get; } = new(EnglishPronoun.ThirdFeminine);

        [Pure] public readonly override string ToString()
        {
            if (FirstPerson == EnglishPronoun.FirstSingular && SecondPerson == EnglishPronoun.SecondSingular)
                return ThirdPerson.ToString();
            if (FirstPerson == EnglishPronoun.FirstPlural && SecondPerson == EnglishPronoun.SecondPlural)
                return $"{ThirdPerson} (plural)";

            return $"{FirstPerson}, {SecondPerson}, {ThirdPerson}";
        }

    }
}
