namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Specifies some Russian noun properties.</para>
    /// </summary>
    public enum RussianNounFlags
    {
        /// <summary>
        ///   <para>Specifies no noun properties.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Specifies that the noun is a singulare tantum, that is, appears only in singular form.</para>
        /// </summary>
        IsSingulareTantum = 0b_10,
        /// <summary>
        ///   <para>Specifies that the noun is a plurale tantum, that is, appears only in plural form.</para>
        /// </summary>
        IsPluraleTantum   = 0b_11,
    }
}
