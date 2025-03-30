using System.ComponentModel;
using JetBrains.Annotations;

namespace GrammarSharp.English
{
    public sealed class EnglishVerb
    {
        public string Infinitive { get; }
        public EnglishVerbInfo Info { get; }

        public EnglishVerb(string infinitive)
            : this(infinitive, null, null, null, default) { }
        public EnglishVerb(string infinitive, EnglishVerbInfo info)
            : this(infinitive, null, null, null, info) { }
        public EnglishVerb(string infinitive, string? simplePast, string? pastParticiple)
            : this(infinitive, simplePast, pastParticiple, null, default) { }
        public EnglishVerb(string infinitive, string? simplePast, string? pastParticiple, EnglishVerbInfo info)
            : this(infinitive, simplePast, pastParticiple, null, info) { }
        public EnglishVerb(string infinitive, string? simplePast, string? pastParticiple, string? presentParticiple, EnglishVerbInfo info)
        {
            Infinitive = infinitive;
            verbSimplePast = simplePast;
            verbPastParticiple = pastParticiple;
            verbPresentParticiple = presentParticiple;
            Info = info;
        }

        private string? verbPresentSg3;
        private string? verbSimplePast;
        private string? verbPastParticiple;
        private string? verbPresentParticiple;

        private string PresentSg3 => verbPresentSg3 ??= InflectPresentSg3Core();
        private string SimplePast => verbSimplePast ??= InflectSimplePastCore();
        private string PastParticiple => verbPastParticiple ??= SimplePast;
        private string PresentParticiple => verbPresentParticiple ??= InflectPresentParticipleCore();

        [Pure] public string Inflect(EnglishTense tense, EnglishAspect aspect, EnglishPerson person, EnglishConjugationType type)
        {
            return tense switch
            {
                EnglishTense.Present
                    => person == EnglishPerson.Third && type == EnglishConjugationType.SingularIs ? PresentSg3 : Infinitive,
                EnglishTense.Past => SimplePast,
                EnglishTense.Future => "will " + Infinitive,
                _ => throw new InvalidEnumArgumentException(nameof(tense), (int)tense, typeof(EnglishTense)),
            };
        }

        [Pure] private string InflectPresentSg3Core()
        {
            string word = Infinitive;

            switch (word[^1])
            {
                case 's' or 'z':
                    return word + "es";

                case 'h':
                    if (word is [.., 'c' or 's', _])
                        return word + "es";
                    break;

                case 'o':
                    if (word.Length > 1 && EnglishLowerCase.IsConsonant(word[^2]))
                        return word + "es";
                    break;

                case 'y':
                    if (word.Length > 1 && EnglishLowerCase.IsConsonant(word[^2]))
                        return word[..^1] + "ies";
                    break;
            }

            return word + "s";
        }

        [Pure] private string InflectSimplePastCore()
        {
            string word = Infinitive;

            switch (word[^1])
            {
                case 'e':
                    return word + "d";

                case 'y':
                    if (word.Length > 1)
                    {
                        if (word[^2] == 'a')
                            return word[..^1] + "id";
                        if (EnglishLowerCase.IsConsonant(word[^2]))
                            return word[..^1] + "ied";
                    }
                    break;

                case 'c':
                    return word + "ked";
            }

            if (word.Length > 1 && EnglishLowerCase.IsDoublableConsonant(word[^1]) && EnglishLowerCase.IsVowel(word[^2]))
                return word + word[^1] + "ed";

            return word + "ed";
        }

        [Pure] private string InflectPresentParticipleCore()
        {
            string word = Infinitive;

            switch (word[^1])
            {
                case 'e':
                    if (word is [.., 'i', _])
                        // ReSharper disable once StringLiteralTypo
                        return word[..^2] + "ying";

                    return word[..^1] + "ing";

                case 'c':
                    // ReSharper disable once StringLiteralTypo
                    return word + "cking";
            }

            if (word.Length > 1 && EnglishLowerCase.IsDoublableConsonant(word[^1]) && EnglishLowerCase.IsVowel(word[^2]))
                return word + word[^1] + "ing";

            return word + "ing";
        }

    }
}
