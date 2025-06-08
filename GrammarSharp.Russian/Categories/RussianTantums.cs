namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines Russian tantums.</para>
    /// </summary>
    public enum RussianTantums
    {
        /// <summary>
        ///   <para>Specifies no tantums.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Specifies a singulare tantum, meaning that the word appears only in singular form.</para>
        /// </summary>
        IsSingulareTantum = 0b_10,
        /// <summary>
        ///   <para>Specifies a plurale tantum, meaning that the word appears only in plural form.</para>
        /// </summary>
        IsPluraleTantum   = 0b_11,
    }
}
