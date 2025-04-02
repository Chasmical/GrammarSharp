using System.Text;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianDeclension
    {
        /// <summary>
        ///   <para>Returns a string representation of this Russian declension.</para>
        /// </summary>
        /// <returns>The string representation of this Russian declension.</returns>
        [Pure] public readonly override string ToString()
        {
            // Longest form (20 chars): мо-жо 1*°f″/f″①②③, ё
            StringBuilder sb = new(32);

            switch (Type)
            {
                case RussianDeclensionType.Noun:
                    if (SpecialNounProperties is { } props)
                        sb.Append(props).Append(' ');
                    break;
                case RussianDeclensionType.Adjective:
                    sb.Append('п').Append(' ');
                    break;
                case RussianDeclensionType.Pronoun:
                    sb.Append('м').Append('с').Append(' ');
                    break;
            }

            // Append the stem type
            sb.Append((char)(StemType + '0'));

            // Append the star and the circle
            RussianDeclensionFlags flags = Flags;
            if ((flags & (RussianDeclensionFlags.Star | RussianDeclensionFlags.Circle)) != 0)
            {
                if ((flags & RussianDeclensionFlags.Star) != 0)
                    sb.Append('*');
                if ((flags & RussianDeclensionFlags.Circle) != 0)
                    sb.Append('°');
            }

            // Append the stress pattern
            sb.Append(StressPattern.ToString(Type));

            const RussianDeclensionFlags trailingFlags
                = RussianDeclensionFlags.CircledOne
                | RussianDeclensionFlags.CircledTwo
                | RussianDeclensionFlags.CircledThree
                | RussianDeclensionFlags.AlternatingYo;

            if ((flags & trailingFlags) != 0)
            {
                // Append the numbers in circles
                if ((flags & RussianDeclensionFlags.CircledOne) != 0)
                    sb.Append('①');
                if ((flags & RussianDeclensionFlags.CircledTwo) != 0)
                    sb.Append('②');
                if ((flags & RussianDeclensionFlags.CircledThree) != 0)
                    sb.Append('③');

                // Append the ё mark
                if ((flags & RussianDeclensionFlags.AlternatingYo) != 0)
                {
                    sb.Append(',');
                    sb.Append(' ');
                    sb.Append('ё');
                }
            }

            // Build and return the string
            return sb.ToString();
        }

    }
}
