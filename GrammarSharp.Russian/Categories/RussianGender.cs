namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines grammatical genders used in Russian language.</para>
    /// </summary>
    public enum RussianGender
    {
        /// <summary>
        ///   <para>Specifies a masculine grammatical gender.</para>
        /// </summary>
        Masculine = 0b_00,
        /// <summary>
        ///   <para>Specifies a neuter grammatical gender.</para>
        /// </summary>
        Neuter    = 0b_01,
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
