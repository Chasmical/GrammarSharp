namespace Chasm.Grammar.English
{
    public sealed class EnglishAdjective
    {
        public string Stem { get; }

        public EnglishAdjective(string stem)
            => Stem = stem;

        public string Decline()
            => Stem;

    }
}
