using System;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines Russian declension flags, according to Zaliznyak's classification.</para>
    /// </summary>
    [Flags]
    public enum RussianDeclensionFlags
    {
        /// <summary>
        ///   <para>Specifies no declension flags.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Specifies a <c>*</c> flag, indicating a vowel alternation in the stem.</para>
        /// </summary>
        Star          = 1 << 0,
        /// <summary>
        ///   <para>Specifies a <c>°</c> flag, indicating a unique alternation in the stem.</para>
        /// </summary>
        Circle        = 1 << 1,
        /// <summary>
        ///   <para>
        ///     Specifies a <c>①</c> flag, indicating a type 1 deviation from common declension patterns.<br/>
        ///     Nouns (nominative plural): use endings a different gender.
        ///     Adjectives (short forms): remove extra trailing 'н' in masculine short form.
        ///   </para>
        /// </summary>
        CircledOne    = 1 << 2,
        /// <summary>
        ///   <para>
        ///     Specifies a <c>②</c> flag, indicating a type 2 deviation from common declension patterns.<br/>
        ///     Nouns (genitive plural): use endings a different gender.
        ///     Adjectives (short forms): remove extra trailing 'н' in all short forms.
        ///   </para>
        /// </summary>
        CircledTwo    = 1 << 3,
        /// <summary>
        ///   <para>
        ///     Specifies a <c>③</c> flag, indicating a type 3 deviation from common declension patterns.<br/>
        ///     Nouns (singular prepositional, or singular dative feminine): ending '-е' is used instead of '-и'.<br/>
        ///     Though '-и' is more grammatically correct, '-е' is used more often and is easier to comprehend.
        ///   </para>
        /// </summary>
        CircledThree  = 1 << 4,
        /// <summary>
        ///   <para>Specifies a <c>ё</c> flag, indicating an alternation between 'ё' and 'е' in the stem.</para>
        /// </summary>
        AlternatingYo = 1 << 5,
    }
}
