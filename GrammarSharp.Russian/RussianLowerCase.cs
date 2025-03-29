using System;
using JetBrains.Annotations;
#if NET8_0_OR_GREATER
using System.Buffers;
#endif

namespace GrammarSharp.Russian
{
    internal static class RussianLowerCase
    {
        private const string Vowels = "аеиоуыэюяё";
        private const string Consonants = "бвгджзйклмнпрстфхцчшщ";
        private const string HissingConsonants = "жчшщ";
        private const string SibilantConsonants = "жцчшщ";
        private const string NonSibilantConsonants = "бвгдзйклмнпрстфх";
        private const string TrimNounStemChars = "аеийоуыьэюяё";

#if NET8_0_OR_GREATER
        private static readonly SearchValues<char> VowelsSearch = SearchValues.Create(Vowels);
        private static readonly SearchValues<char> ConsonantsSearch = SearchValues.Create(Consonants);
        private static readonly SearchValues<char> HissingConsonantsSearch = SearchValues.Create(HissingConsonants);
        private static readonly SearchValues<char> SibilantConsonantsSearch = SearchValues.Create(SibilantConsonants);
        private static readonly SearchValues<char> NonSibilantConsonantsSearch = SearchValues.Create(NonSibilantConsonants);
        private static readonly SearchValues<char> TrimNounStemCharsSearch = SearchValues.Create(TrimNounStemChars);
#endif

        [Pure] public static int LastIndexOfVowel(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.LastIndexOfAny(VowelsSearch);
#else
            return text.LastIndexOfAny(Vowels);
#endif
        }
        [Pure] public static int IndexOfVowel(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.IndexOfAny(VowelsSearch);
#else
            return text.IndexOfAny(Vowels);
#endif
        }
        [Pure] public static int LastIndexOfConsonant(ReadOnlySpan<char> text)
        {
#if NET8_0_OR_GREATER
            return text.LastIndexOfAny(ConsonantsSearch);
#else
            return text.LastIndexOfAny(Consonants);
#endif
        }

        [Pure] public static bool IsVowel(char ch)
        {
#if NET8_0_OR_GREATER
            return VowelsSearch.Contains(ch);
#else
            return Vowels.Contains(ch);
#endif
        }
        [Pure] public static bool IsHissingConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return HissingConsonantsSearch.Contains(ch);
#else
            return HissingConsonants.Contains(ch);
#endif
        }
        [Pure] public static bool IsSibilantConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return SibilantConsonantsSearch.Contains(ch);
#else
            return SibilantConsonants.Contains(ch);
#endif
        }
        [Pure] public static bool IsNonSibilantConsonant(char ch)
        {
#if NET8_0_OR_GREATER
            return NonSibilantConsonantsSearch.Contains(ch);
#else
            return NonSibilantConsonants.Contains(ch);
#endif
        }
        [Pure] public static bool IsTrimNounStemChar(char ch)
        {
#if NET8_0_OR_GREATER
            return TrimNounStemCharsSearch.Contains(ch);
#else
            return TrimNounStemChars.Contains(ch);
#endif
        }

    }
}
