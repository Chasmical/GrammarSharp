using JetBrains.Annotations;

namespace Chasm.Grammar.English
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
