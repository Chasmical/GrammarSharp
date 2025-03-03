using System;

namespace Chasm.Grammar.Russian
{
    // Note: Don't derive these enums from byte, since they're always packed in structs anyway
    public enum RussianCase
    {
        Nominative,
        Genitive,
        Dative,
        Accusative,
        Instrumental,
        Prepositional,
    }
    public enum RussianTense
    {
        Past,
        Present,
        Future,
    }
    public enum RussianGender
    {
        Neuter    = 0b_00,
        Masculine = 0b_01,
        Feminine  = 0b_10,
        Common    = 0b_11,
    }
    [Flags]
    public enum RussianStressPattern
    {
        // 0 is reserved for stem type 0, that doesn't have a stress pattern
        Zero = 0,

        // Regular stress patterns start from 1
        A = 0b_0001, // nouns, adjectives, pronouns
        B = 0b_0010, // nouns, adjectives, pronouns
        C = 0b_0011, // nouns, adjectives
        D = 0b_0100, // nouns
        E = 0b_0101, // nouns
        F = 0b_0110, // nouns
        // (0b_0111 is unused)

        // Single- and double-prime stress patterns (a′, f″) start from 0
        Ap = 0b_1000, // adjectives
        Bp = 0b_1001, // nouns, adjectives
        Cp = 0b_1010, // adjectives
        Dp = 0b_1011, // nouns
        Ep = 0b_1100, //
        Fp = 0b_1101, // nouns

        Cpp = 0b_1110, // adjectives
        Fpp = 0b_1111, // nouns
    }
    public enum RussianDeclensionType
    {
        Noun,
        Adjective,
        Pronoun,
    }
    public enum RussianNounFlags
    {
        None = 0,

        IsSingulareTantum = 0b_10,
        IsPluraleTantum   = 0b_11,
    }
    [Flags]
    public enum RussianDeclensionFlags
    {
        None = 0,

        Star          = 1 << 0,
        Circle        = 1 << 1,
        CircledOne    = 1 << 2,
        CircledTwo    = 1 << 3,
        CircledThree  = 1 << 4,
        AlternatingYo = 1 << 5,
    }
    [Flags]
    public enum RussianConjugationFlags
    {
        None = 0,

        Star          = 1 << 0,
        Circle        = 1 << 1,
        CircledOne    = 1 << 2,
        CircledTwo    = 1 << 3,
        CircledThree  = 1 << 4,
        CircledFour   = 1 << 5,
        CircledFive   = 1 << 6,
        CircledSix    = 1 << 7,
        CircledSeven  = 1 << 8,
        CircledEight  = 1 << 9,
        CircledNine   = 1 << 10,
        AlternatingYo = 1 << 11,
        AlternatingO  = 1 << 12,
    }
    [Flags]
    public enum RussianVerbFlags
    {
        Perfective    = 1 << 0,
        Imperfective  = 1 << 1,
        Intransitive  = 1 << 2,
        Reflexive     = 1 << 3, // TODO: is it really needed though? should it be processed automatically in ctor?
        Impersonal    = 1 << 4,
        Frequentative = 1 << 5,

    }
}
