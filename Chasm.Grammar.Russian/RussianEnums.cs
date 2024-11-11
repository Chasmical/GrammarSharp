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
    public enum RussianGender
    {
        Neuter    = 0b_00,
        Masculine = 0b_01,
        Feminine  = 0b_10,
        Common    = 0b_11,
    }
    [Flags]
    public enum RussianDeclensionAccent
    {
        // 0 is reserved for 0 declension
        Zero = 0,

        // Regular accent letters start from 1
        A = 0b_0001, // nouns, adjectives, pronouns
        B = 0b_0010, // nouns, adjectives, pronouns
        C = 0b_0011, // nouns, adjectives
        D = 0b_0100, // nouns
        E = 0b_0101, // nouns
        F = 0b_0110, // nouns
        // (0b_0111 is unused)

        // Single- and double-prime accent letters (a′, f″) start from 0
        Ap = 0b_1000, // adjectives
        Bp = 0b_1001, // nouns, adjectives
        Cp = 0b_1010, // adjectives
        Dp = 0b_1011, // nouns
        Ep = 0b_1100, //
        Fp = 0b_1101, // nouns

        Cpp = 0b_1110, // adjectives
        Fpp = 0b_1111, // nouns
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
}
