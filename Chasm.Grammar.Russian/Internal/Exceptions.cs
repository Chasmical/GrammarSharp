using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    internal static class Exceptions
    {
        public const string Leftovers = "";

        public const string StemTypeNotFound = "";
        public const string StressNotFound = "";
        public const string GenderNotFound = "";

        public const string InvalidStemType = "";
        public const string InvalidStress = "";
        public const string InvalidStressLetter = "";
        public const string InvalidStressPrime = "";
        public const string InvalidProperties = "";
        public const string InvalidDeclension = "";

        [Pure] public static string GetMessage(this ParseCode code) => code switch
        {
            ParseCode.Leftovers => Leftovers,

            ParseCode.StemTypeNotFound => StemTypeNotFound,
            ParseCode.StressNotFound => StressNotFound,
            ParseCode.GenderNotFound => GenderNotFound,

            ParseCode.InvalidStemType => InvalidStemType,
            ParseCode.InvalidStress => InvalidStress,
            ParseCode.InvalidStressLetter => InvalidStressLetter,
            ParseCode.InvalidStressPrime => InvalidStressPrime,
            ParseCode.InvalidProperties => InvalidProperties,
            ParseCode.InvalidDeclension => InvalidDeclension,

            // dotcover disable next line
            _ => throw new ArgumentException(code + " error code is not supposed to have a message."),
        };

        [Pure, MustUseReturnValue, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TReturn ReturnOrThrow<TReturn>(this ParseCode code, TReturn? returnValue, [InvokerParameterName] string parameterName)
            => code is ParseCode.Success ? returnValue! : throw new ArgumentException(code.GetMessage(), parameterName);

    }
}
