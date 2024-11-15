using System;
#if NET8_0_OR_GREATER
using System.Buffers;
#endif

namespace Chasm.Grammar.Russian
{
    internal static class RussianLowerCase
    {
        private const string Vowels = "аеёиоуыэюя";
        private const string Consonants = "бвгджзйклмнпрстфхцчшщ";
        private const string HissingConsonants = "шжчщ";
        private const string SibilantConsonants = "шжчщц";
        private const string NonSibilantConsonants = "бвгдзйклмнпрстфх";

#if NET8_0_OR_GREATER
        private static readonly SearchValues<char> VowelsSearch = SearchValues.Create(Vowels);
        private static readonly SearchValues<char> ConsonantsSearch = SearchValues.Create(Consonants);
        private static readonly SearchValues<char> HissingConsonantsSearch = SearchValues.Create(HissingConsonants);
        private static readonly SearchValues<char> SibilantConsonantsSearch = SearchValues.Create(SibilantConsonants);
        private static readonly SearchValues<char> NonSibilantConsonantsSearch = SearchValues.Create(NonSibilantConsonants);
#endif

        public static int LastIndexOfVowel(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.LastIndexOfAny(VowelsSearch);
#else
            return text.LastIndexOfAny(Vowels);
#endif
        }
        public static int IndexOfVowel(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.IndexOfAny(VowelsSearch);
#else
            return text.IndexOfAny(Vowels);
#endif
        }
        public static int LastIndexOfConsonant(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.LastIndexOfAny(ConsonantsSearch);
#else
            return text.LastIndexOfAny(Consonants);
#endif
        }

        public static bool IsVowel(char ch)
        {
#if NET8_0_OR_GREATER
            return VowelsSearch.Contains(ch);
#else
            return Vowels.Contains(ch);
#endif
        }
        public static bool IsHissingConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return HissingConsonantsSearch.Contains(ch);
#else
            return HissingConsonants.Contains(ch);
#endif
        }
        public static bool IsSibilantConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return SibilantConsonantsSearch.Contains(ch);
#else
            return SibilantConsonants.Contains(ch);
#endif
        }
        public static bool IsNonSibilantConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return NonSibilantConsonantsSearch.Contains(ch);
#else
            return NonSibilantConsonants.Contains(ch);
#endif
        }

    }
}
