using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    /// <summary>
    ///   <para>Represents a Russian adjective.</para>
    /// </summary>
    public sealed partial class RussianAdjective
    {
        /// <summary>
        ///   <para>Gets the Russian adjective's stem.</para>
        /// </summary>
        public string Stem { get; }
        /// <summary>
        ///   <para>Gets the Russian adjective's info.</para>
        /// </summary>
        public RussianAdjectiveInfo Info { get; }

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjective"/> class with the specified <paramref name="word"/> and <paramref name="info"/>.</para>
        /// </summary>
        /// <param name="word">The Russian adjective's regular form.</param>
        /// <param name="info">The Russian adjective's info.</param>
        public RussianAdjective(ReadOnlySpan<char> word, RussianAdjectiveInfo info)
        {
            Stem = info._declension.ExtractStem(word).ToString();
            Info = info;
        }
        /// <inheritdoc cref="RussianAdjective(ReadOnlySpan{char}, RussianAdjectiveInfo)"/>
        /// <exception cref="ArgumentNullException"><paramref name="word"/> is <see langword="null"/>.</exception>
        public RussianAdjective(string word, RussianAdjectiveInfo info)
        {
            Guard.ThrowIfNull(word);
            Stem = info._declension.ExtractStem(word);
            Info = info;
        }

        // ReSharper disable once UnusedParameter.Local
        private RussianAdjective(string stem, RussianAdjectiveInfo info, bool _)
        {
            Stem = stem;
            Info = info;
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <seealso cref="RussianAdjective"/> class with the specified <paramref name="stem"/> and <paramref name="info"/>.</para>
        /// </summary>
        /// <param name="stem">The Russian adjective's stem.</param>
        /// <param name="info">The Russian adjective's info.</param>
        /// <returns>A new <seealso cref="RussianAdjective"/> instance with the specified <paramref name="stem"/> and <paramref name="info"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stem"/> is <see langword="null"/>.</exception>
        [Pure] public static RussianAdjective FromStem(string stem, RussianAdjectiveInfo info)
        {
            Guard.ThrowIfNull(stem);
            return new RussianAdjective(stem, info, false);
        }

    }
}
