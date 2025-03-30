namespace GrammarSharp.Russian
{
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
}
