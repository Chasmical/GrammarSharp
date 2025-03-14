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
    public enum RussianStress
    {
        // 0 is for when the stress is not specified
        Zero = 0, // nouns, adjectives, pronouns

        // Regular stress patterns start from 1
        A = 0b_0001, // nouns, adjectives, pronouns, verbs
        B = 0b_0010, // nouns, adjectives, pronouns, verbs
        C = 0b_0011, // nouns, adjectives, verbs
        D = 0b_0100, // nouns
        E = 0b_0101, // nouns
        F = 0b_0110, // nouns, pronouns

        // Single- and double-prime stress patterns (a′, f″) start from 0
        Ap = 0b_0111, // adjectives
        Bp = 0b_1000, // nouns, adjectives
        Cp = 0b_1001, // adjectives, verbs
        Dp = 0b_1010, // nouns
        Ep = 0b_1011, //
        Fp = 0b_1100, // nouns

        Cpp = 0b_1101, // adjectives, verbs
        Fpp = 0b_1110, // nouns
        //   (0b_1111 is only used for debugging, to display `a/` or `/b`)
    }
    public enum RussianDeclensionType
    {
        Unknown,
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
