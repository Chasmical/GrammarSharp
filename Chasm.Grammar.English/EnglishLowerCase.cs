using JetBrains.Annotations;
#if NET8_0_OR_GREATER
using System.Buffers;
#endif

namespace Chasm.Grammar.English
{
    internal static class EnglishLowerCase
    {
        private const string Vowels = "aeiou";
        private const string DoublableConsonants = "aehiotuwxy";

#if NET8_0_OR_GREATER
        private static readonly SearchValues<char> VowelsSearch = SearchValues.Create(Vowels);
        private static readonly SearchValues<char> DoublableConsonantsSearch = SearchValues.Create(DoublableConsonants);
#endif

        [Pure] public static bool IsVowel(char ch)
        {
#if NET8_0_OR_GREATER
            return VowelsSearch.Contains(ch);
#else
            return Vowels.Contains(ch);
#endif
        }
        [Pure] public static bool IsConsonant(char ch)
            => !IsVowel(ch);

        [Pure] public static bool IsDoublableConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return DoublableConsonantsSearch.Contains(ch);
#else
            return DoublableConsonants.Contains(ch);
#endif
        }

    }
}
