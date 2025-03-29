namespace GrammarSharp.Russian
{
    internal enum ParseCode : uint
    {
        Success,
        Leftovers,

        StemTypeNotFound,
        StressNotFound,
        GenderNotFound,

        InvalidStemType,
        InvalidStress,
        InvalidStressLetter,
        InvalidStressPrime,
        InvalidProperties,
        InvalidDeclension,
        InvalidTantums,

        UnclosedBraces,
    }
}
