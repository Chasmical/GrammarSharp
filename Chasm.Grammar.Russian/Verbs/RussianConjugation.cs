using System;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianConjugation
    {
        private readonly byte _conjugationType;
        private readonly byte _stresses;
        private readonly ushort _flags;

        public int ConjugationType => _conjugationType;
        public RussianStressPattern StressPattern => new(_stresses);
        public RussianConjugationFlags Flags => (RussianConjugationFlags)_flags;

    }
}
