using JetBrains.Annotations;

namespace GrammarSharp.English
{
    public sealed class EnglishAdjective
    {
        public string Stem { get; }

        public EnglishAdjective(string stem)
            => Stem = stem;

        [Pure] public string Decline()
            => Stem;

    }
}
