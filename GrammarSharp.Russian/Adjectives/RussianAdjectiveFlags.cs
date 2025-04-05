using System;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines some Russian adjective properties.</para>
    /// </summary>
    [Flags]
    public enum RussianAdjectiveFlags
    {
        /// <summary>
        ///   <para>Specifies no adjective properties.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Specifies that the adjective does not have a comparative form.</para>
        /// </summary>
        NoComparativeForm = 0b_00_0001,
        /// <summary>
        ///   <para>Specifies that the adjective has difficulty forming a masculine short form.</para>
        /// </summary>
        Minus             = 0b_00_0010,
        /// <summary>
        ///   <para>Specifies that the adjective has difficulty forming short forms of all genders.</para>
        /// </summary>
        Cross             = 0b_00_0100,
        /// <summary>
        ///   <para>Specifies that the adjective does not have a masculine short form, and has difficulty forming short forms of other genders.</para>
        /// </summary>
        BoxedCross        = 0b_00_0110,
        /// <summary>
        ///   <para>Specifies that the adjective is a numeral adjective.</para>
        /// </summary>
        IsNumeral         = 0b_10_0000,
        /// <summary>
        ///   <para>Specifies that the adjective is a pronoun adjective.</para>
        /// </summary>
        IsPronoun         = 0b_11_0000,
    }
}
