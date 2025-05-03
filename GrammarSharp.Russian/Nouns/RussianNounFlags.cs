using System;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Defines some Russian noun properties.</para>
    /// </summary>
    [Flags]
    public enum RussianNounFlags
    {
        /// <summary>
        ///   <para>Specifies no noun properties.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Specifies that the noun is a paired object, that in count forms uses the auxiliary word "пара".<br/>This enum flag only has an effect on nouns with a plurale tantum.<br/>("ботинки" — "три пары ботинок" instead of "три ботинка")</para>
        /// </summary>
        IsPaired = 0b_0001,
    }
}
