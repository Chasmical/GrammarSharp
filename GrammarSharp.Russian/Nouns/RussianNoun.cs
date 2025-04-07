using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian noun.</para>
    /// </summary>
    public sealed partial class RussianNoun
    {
        /// <summary>
        ///   <para>Gets the Russian noun's stem.</para>
        /// </summary>
        public string Stem { get; }
        /// <summary>
        ///   <para>Gets the Russian noun's info.</para>
        /// </summary>
        public RussianNounInfo Info { get; }

        private AnomalousFormList _anomalies;

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="RussianNoun"/> class with the specified <paramref name="word"/> and <paramref name="info"/>.</para>
        /// </summary>
        /// <param name="word">The Russian noun's regular form.</param>
        /// <param name="info">The Russian noun's info.</param>
        public RussianNoun(ReadOnlySpan<char> word, RussianNounInfo info)
        {
            Stem = info._declension.ExtractStem(word).ToString();
            Info = info;
        }
        /// <inheritdoc cref="RussianNoun(ReadOnlySpan{char}, RussianNounInfo)"/>
        /// <exception cref="ArgumentNullException"><paramref name="word"/> is <see langword="null"/>.</exception>
        public RussianNoun(string word, RussianNounInfo info)
        {
            Guard.ThrowIfNull(word);
            Stem = info._declension.ExtractStem(word);
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianNoun(string stem, RussianNounInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianNoun"/> class with the specified <paramref name="stem"/> and <paramref name="info"/>.</para>
        /// </summary>
        /// <param name="stem">The Russian noun's stem.</param>
        /// <param name="info">The Russian noun's info.</param>
        /// <returns>A new <seealso cref="RussianNoun"/> instance with the specified <paramref name="stem"/> and <paramref name="info"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stem"/> is <see langword="null"/>.</exception>
        [Pure] public static RussianNoun FromStem(string stem, RussianNounInfo info)
        {
            Guard.ThrowIfNull(stem);
            return new RussianNoun(stem, info, false);
        }

    }
}
