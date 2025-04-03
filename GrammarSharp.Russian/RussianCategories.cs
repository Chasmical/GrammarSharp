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

        // TODO: add Ablative case, "от/из/с чего - из дому" (did it turn into just the Locative case?)
        // TODO: add Vocative case (not a case???)
        // TODO: add Locative case "в чём, где - в лесу" (usually same as prepositional, with stress on ending)
        // TODO: add Partitive case "чего - сахару" (usually same as dative)
        // TODO: add Waitive(???) case (seems to be just a context-dependent usage of genitive and accusative)
        // TODO: add Translative case (в что/кого - в солдаты) (usually same as plural nominative)
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
