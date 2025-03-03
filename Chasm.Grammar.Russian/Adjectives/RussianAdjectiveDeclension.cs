namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianAdjectiveDeclension
    {
        // Representation (_stemType field):
        //   xxxx_1111 - stem type      (0000 - 0, 1000 - 8)
        //
        // Representation (_stresses field):
        //   xxxx_1111 - full form stress pattern  (see RussianStressPattern enum)
        //   1111_xxxx - short form stress pattern (see RussianStressPattern enum)
        //
        private readonly byte _stemType;
        private readonly byte _stresses;
        private readonly byte _flags;

        public int StemType => _stemType;
        public RussianStressPattern FullFormStress => (RussianStressPattern)(_stresses & 0x0F);
        public RussianStressPattern ShortFormStress => (RussianStressPattern)(_stresses >> 4);
        public RussianDeclensionFlags Flags => (RussianDeclensionFlags)_flags;

    }
}
