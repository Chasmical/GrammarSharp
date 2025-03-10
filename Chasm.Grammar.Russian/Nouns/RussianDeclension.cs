namespace Chasm.Grammar.Russian
{
    public readonly partial struct RussianDeclension
    {
        // Representation (_types field):
        //   xxxx_1111 - stem type       (0000 - 0, 1000 - 8)
        //   1111_xxxx - declension type (see RussianDeclensionType enum)
        //
        // Representation (_stressPattern field):
        //   1111_1111 - see RussianStressPattern struct
        //
        // Representation (_flags field):
        //   1111_1111_1111 - see RussianDeclensionFlags enum
        //
        private readonly byte _types;
        private readonly RussianStressPattern _stressPattern;
        private readonly byte _flags;

        public int StemType => _types & 0x0F;
        public RussianDeclensionType Type => (RussianDeclensionType)(_types >> 4);
        public RussianStressPattern StressPattern => _stressPattern;
        public RussianDeclensionFlags Flags => (RussianDeclensionFlags)_flags;

        public RussianDeclension(RussianDeclensionType type, int stemType, RussianStressPattern stressPattern, RussianDeclensionFlags flags)
        {
            _types = (byte)(stemType | ((int)type << 4));
            _stressPattern = stressPattern;
            _flags = (byte)flags;
        }

        public bool IsZero => StemType == 0;

    }
}
