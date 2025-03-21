using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.English
{
    public sealed class EnglishVerb
    {
        public string Infinitive { get; }
        public EnglishVerbInfo Info { get; }

        private string? verbSimplePast;
        private string? verbPastParticiple;
        private string? verbPresentSg3;

        public EnglishVerb(string infinitive)
            : this(infinitive, null, null, default) { }
        public EnglishVerb(string infinitive, EnglishVerbInfo info)
            : this(infinitive, null, null, info) { }
        public EnglishVerb(string infinitive, string? simplePast, string? pastParticiple)
            : this(infinitive, simplePast, pastParticiple, default) { }
        public EnglishVerb(string infinitive, string? simplePast, string? pastParticiple, EnglishVerbInfo info)
        {
            Infinitive = infinitive;
            verbSimplePast = simplePast;
            verbPastParticiple = pastParticiple;
            Info = info;
        }

        [Pure] public string Inflect(EnglishTense tense, EnglishPerson person, bool plural)
        {
            if (tense <= EnglishTense.Past)
            {
                string result
                    = tense == EnglishTense.Past
                        ? verbSimplePast ??= InflectSimplePastCore()
                        : !plural && person == EnglishPerson.Third
                            ? verbPresentSg3 ??= InflectPresentSg3Core()
                            : Infinitive;

                return result;
            }
            throw new NotImplementedException();
        }
        [Pure] public string InflectParticiple(EnglishTense tense)
        {
            if (tense <= EnglishTense.Past)
            {
                return tense == EnglishTense.Present
                    ? InflectPresentParticipleCore()
                    : verbPastParticiple ??= InflectSimplePastCore();
            }
            throw new NotImplementedException();
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

        [Pure] private string InflectPresentParticipleCore()
        {
            string word = Infinitive;

            switch (word[^1])
            {
                case 'e':
                    if (word.Length > 1 && word[^2] == 'i')
                        return word[..^2] + "ying";

                    return word[..^1] + "ing";

                case 'c':
                    return word + "cking";
            }

            if (word.Length > 1 && EnglishLowerCase.IsDoublableConsonant(word[^1]) && EnglishLowerCase.IsVowel(word[^2]))
                return word + word[^1] + "ing";

            return word + "ing";
        }

    }
}
