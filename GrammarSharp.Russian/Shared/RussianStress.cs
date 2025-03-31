namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>
    ///     Defines stress schemas in Russian words, according to Zaliznyak's classification.<br/>
    ///     Values: a, b, c, d, e, f, a′, b′, c′, d′, e′, f′, c″, f″.
    ///   </para>
    /// </summary>
    public enum RussianStress
    {
        /// <summary>
        ///   <para>Specifies no stress. Used for uninflectable words.</para>
        /// </summary>
        Zero = 0,

        /// <summary>
        ///   <para>Stress schema a. The stress is always on the stem. Used by all inflectable words.</para>
        /// </summary>
        A = 0b_0001,
        /// <summary>
        ///   <para>Stress schema b. The stress is always on the ending. Used by all inflectable words.</para>
        /// </summary>
        B = 0b_0010,
        /// <summary>
        ///   <para>
        ///     Stress schema c.<br/>
        ///     Nouns: singular - stress on stem, plural - stress on ending.<br/>
        ///     Adjectives (short form only): feminine - stress on ending, all other - stress on stem.<br/>
        ///     Verbs (non-past tense): first person, and imperative - stress on ending, all other - stress on stem.<br/>
        ///     Verbs (past tense): feminine - stress on ending, all other - stress on stem.
        ///   </para>
        /// </summary>
        C = 0b_0011,
        /// <summary>
        ///   <para>
        ///     Stress schema d.<br/>
        ///     Nouns: singular - stress on ending, plural - stress on stem.
        ///   </para>
        /// </summary>
        D = 0b_0100,
        /// <summary>
        ///   <para>
        ///     Stress schema e.<br/>
        ///     Nouns: singular, and plural nominative - stress on stem, plural of other cases - stress on ending.
        ///   </para>
        /// </summary>
        E = 0b_0101,
        /// <summary>
        ///   <para>
        ///     Stress schema f.<br/>
        ///     Nouns and pronouns: plural nominative - stress on stem, all other - stress on ending.
        ///   </para>
        /// </summary>
        F = 0b_0110,

        /// <summary>
        ///   <para>
        ///     Stress schema a′ (a with single prime).<br/>
        ///     Adjectives (short form only): feminine - both??? (resolved as on stem), all other - stress on stem.
        ///   </para>
        /// </summary>
        Ap = 0b_0111,
        /// <summary>
        ///   <para>
        ///     Stress schema b′ (b with single prime).<br/>
        ///     Nouns: singular instrumental - stress on stem, all other - stress on ending.<br/>
        ///     Adjectives (short form only): plural - both??? (resolved as on ending), all other - stress on ending.
        ///   </para>
        /// </summary>
        Bp = 0b_1000,
        /// <summary>
        ///   <para>
        ///     Stress schema c′ (c with single prime).<br/>
        ///     Adjectives (short form only): feminine - stress on ending, neuter - stress on stem, plural - both??? (TODO).<br/>
        ///     Verbs (non-past tense): first person, imperative, and plural - stress on ending, all other - stress on stem.<br/>
        ///     Verbs (past tense): feminine - stress on ending, neuter - both??? (TODO), all other - stress on stem.
        ///   </para>
        /// </summary>
        Cp = 0b_1001,
        /// <summary>
        ///   <para>
        ///     Stress schema d′ (d with single prime).<br/>
        ///     Nouns: singular accusative, and plural - stress on stem, singular of other cases - stress on ending.
        ///   </para>
        /// </summary>
        Dp = 0b_1010,
        /// <summary>
        ///   <para>
        ///     Stress schema e′ (e with single prime).<br/>
        ///     Unused??? TODO
        ///   </para>
        /// </summary>
        Ep = 0b_1011,
        /// <summary>
        ///   <para>
        ///     Stress schema f′ (f with single prime).<br/>
        ///     Nouns: singular accusative, and plural nominative - stress on stem, all other - stress on ending.
        ///   </para>
        /// </summary>
        Fp = 0b_1100,

        /// <summary>
        ///   <para>
        ///     Stress schema c″ (c with double prime).<br/>
        ///     Adjectives (short form only): feminine - stress on ending, all other - both??? (resolved as on ending).<br/>
        ///     Verbs (past tense reflexive only): masculine - stress on stem, feminine - stress on ending, neuter and plural - both??? (TODO).
        ///   </para>
        /// </summary>
        Cpp = 0b_1101,
        /// <summary>
        ///   <para>
        ///     Stress schema f″ (f with double prime).<br/>
        ///     Nouns: singular instrumental, and plural nominative - stress on stem, all other - stress on ending.
        ///   </para>
        /// </summary>
        Fpp = 0b_1110,
    }
}
