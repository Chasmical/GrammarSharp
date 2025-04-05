using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianNounProperties
    {
        /// <summary>
        ///   <para>Returns a string representation of these noun properties.</para>
        /// </summary>
        /// <returns>The string representation of these noun properties.</returns>
        [Pure] public readonly override string ToString()
        {
            if (IsPluraleTantum) return "мн.";

            bool isAnimate = IsAnimate;
            return Gender switch
            {
                RussianGender.Neuter => isAnimate ? "со" : "с",
                RussianGender.Masculine => isAnimate ? "мо" : "м",
                RussianGender.Feminine => isAnimate ? "жо" : "ж",
                _ => isAnimate ? "мо-жо" : "м-ж",
            };
        }

    }
}
