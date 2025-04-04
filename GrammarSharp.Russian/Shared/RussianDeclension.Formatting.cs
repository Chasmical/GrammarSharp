using System;
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
            // Longest form (noun): мо-жо 8*°f″①②③, ё (17 chars)
            // Longest form (adj) : п 7*f″/f″①②, ё (14 chars)
            // Longest form (pro) : TODO
            Span<char> buffer = stackalloc char[32];
            int offset = 0;

            int stemType;
            RussianStressPattern stress;
            RussianDeclensionFlags flags;

            // Retrieve common declension values depending on the type
            switch (Type)
            {
                case RussianDeclensionType.Noun:
                {
                    var forNoun = ForNounUnsafe();
                    stemType = forNoun.StemType;
                    stress = new(forNoun.Stress);
                    flags = forNoun.Flags;

                    // Append special declension properties
                    if (forNoun.SpecialProperties is { } specialProps)
                    {
                        string specialPropsStr = specialProps.ToString();
                        specialPropsStr
#if !NET6_0_OR_GREATER
                            .AsSpan()
#endif
                            .CopyTo(buffer);
                        offset += specialPropsStr.Length;

                        buffer[offset++] = ' ';
                    }
                    break;
                }
                case RussianDeclensionType.Adjective:
                {
                    var forAdjective = ForAdjectiveUnsafe();
                    stemType = forAdjective.StemType;
                    stress = forAdjective.StressPattern;
                    flags = forAdjective.Flags;

                    // Append the declension type identifier
                    buffer[offset++] = 'п';
                    buffer[offset++] = ' ';
                    break;
                }
                case RussianDeclensionType.Pronoun:
                {
                    var forAdjective = ForAdjectiveUnsafe();
                    stemType = forAdjective.StemType;
                    stress = forAdjective.StressPattern;
                    flags = forAdjective.Flags;

                    // Append the declension type identifier
                    buffer[offset++] = 'м';
                    buffer[offset++] = 'с';
                    buffer[offset++] = ' ';
                    break;
                }
                default:
                    throw new NotImplementedException("Pro/pro-adj declension: formatting");
            }

            // Append the stem type
            buffer[offset++] = (char)('0' + stemType);

            // Append the star and the circle
            if ((flags & (RussianDeclensionFlags.Star | RussianDeclensionFlags.Circle)) != 0)
            {
                if ((flags & RussianDeclensionFlags.Star) != 0)
                    buffer[offset++] = '*';
                if ((flags & RussianDeclensionFlags.Circle) != 0)
                    buffer[offset++] = '°';
            }

            // Append the stress pattern
            // TODO: format stress pattern directly to span buffer
            string stressStr = stress.ToString();
            stressStr
#if !NET6_0_OR_GREATER
                .AsSpan()
#endif
                .CopyTo(buffer[offset..]);
            offset += stressStr.Length;

            const RussianDeclensionFlags trailingFlags
                = RussianDeclensionFlags.CircledOne
                | RussianDeclensionFlags.CircledTwo
                | RussianDeclensionFlags.CircledThree
                | RussianDeclensionFlags.AlternatingYo;

            if ((flags & trailingFlags) != 0)
            {
                // Append the numbers in circles
                if ((flags & RussianDeclensionFlags.CircledOne) != 0)
                    buffer[offset++] = '①';
                if ((flags & RussianDeclensionFlags.CircledTwo) != 0)
                    buffer[offset++] = '②';
                if ((flags & RussianDeclensionFlags.CircledThree) != 0)
                    buffer[offset++] = '③';

                // Append the ё mark
                if ((flags & RussianDeclensionFlags.AlternatingYo) != 0)
                {
                    buffer[offset++] = ',';
                    buffer[offset++] = ' ';
                    buffer[offset++] = 'ё';
                }
            }

            // Build and return the string
            return new string(buffer[..offset]);
        }

    }
}
