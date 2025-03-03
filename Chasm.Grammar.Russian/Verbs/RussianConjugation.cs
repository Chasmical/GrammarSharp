using System;

namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianConjugation
    {
        // Representation (_conjugationType field):
        //   1111_1111 - conjugation type
        //
        // Representation (_stresses field):
        //   xxxx_1111 - present tense stress pattern (see RussianStressPattern enum)
        //   1111_xxxx - past tense stress pattern    (see RussianStressPattern enum)
        //
        private readonly byte _conjugationType;
        private readonly byte _stresses;
        private readonly ushort _flags;

        public int ConjugationType => _conjugationType;
        public RussianStressPattern PresentTenseStress => (RussianStressPattern)(_stresses & 0x0F);
        public RussianStressPattern PastTenseStress => (RussianStressPattern)(_stresses >> 4);
        public RussianConjugationFlags Flags => (RussianConjugationFlags)_flags;

    }
}
