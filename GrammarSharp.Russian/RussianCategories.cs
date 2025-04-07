namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines grammatical cases used in Russian language.</para>
    /// </summary>
    public enum RussianCase
    {
        /// <summary>
        ///   <para>Specifies a nominative grammatical case.</para>
        /// </summary>
        Nominative,
        /// <summary>
        ///   <para>Specifies a genitive grammatical case.</para>
        /// </summary>
        Genitive,
        /// <summary>
        ///   <para>Specifies a dative grammatical case.</para>
        /// </summary>
        Dative,
        /// <summary>
        ///   <para>Specifies an accusative grammatical case.</para>
        /// </summary>
        Accusative,
        /// <summary>
        ///   <para>Specifies an instrumental grammatical case.</para>
        /// </summary>
        Instrumental,
        /// <summary>
        ///   <para>Specifies a prepositional grammatical case.</para>
        /// </summary>
        Prepositional,

        /// <summary>
        ///   <para>Specifies an optional partitive (2nd genitive) grammatical case.</para>
        /// </summary>
        Partitive,
        /// <summary>
        ///   <para>Specifies an optional translative (2nd accusative; though defaults to plural nominative) grammatical case.</para>
        /// </summary>
        Translative,
        /// <summary>
        ///   <para>Specifies an optional locative (2nd prepositional) grammatical case.</para>
        /// </summary>
        Locative,
    }
    /// <summary>
    ///   <para>Defines grammatical tenses used in Russian language.</para>
    /// </summary>
    public enum RussianTense
    {
        /// <summary>
        ///   <para>Specifies a past grammatical tense.</para>
        /// </summary>
        Past,
        /// <summary>
        ///   <para>Specifies a present grammatical tense.</para>
        /// </summary>
        Present,
        /// <summary>
        ///   <para>Specifies a future grammatical tense.</para>
        /// </summary>
        Future,
    }
    /// <summary>
    ///   <para>Defines grammatical genders used in Russian language.</para>
    /// </summary>
    public enum RussianGender
    {
        /// <summary>
        ///   <para>Specifies a neuter grammatical gender.</para>
        /// </summary>
        Neuter    = 0b_00,
        /// <summary>
        ///   <para>Specifies a masculine grammatical gender.</para>
        /// </summary>
        Masculine = 0b_01,
        /// <summary>
        ///   <para>Specifies a feminine grammatical gender.</para>
        /// </summary>
        Feminine  = 0b_10,
        /// <summary>
        ///   <para>Specifies a common grammatical gender.</para>
        /// </summary>
        Common    = 0b_11,
    }
}
