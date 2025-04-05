using System;
using System.Diagnostics;
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
            => ToString(true);
        [Pure] internal readonly string ToString(bool includeType)
        {
            // Longest forms of types:
            // Noun      (17 chars): мо-жо 8*°f″①②③, ё
            // Adjective (14 chars): п 7*f″/f″①②, ё
            // Pronoun    (6 chars): мс 6*f
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
                    var forNoun = this.AsNounUnsafeRef();
                    stemType = forNoun.StemType;
                    stress = new(forNoun.Stress);
                    flags = forNoun.Flags;

                    // Append special declension properties
                    if (includeType && forNoun.SpecialProperties is { } specialProps)
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
                    var forAdjective = this.AsAdjectiveUnsafeRef();
                    stemType = forAdjective.StemType;
                    stress = forAdjective.StressPattern;
                    flags = forAdjective.Flags;

                    // Append the declension type identifier
                    if (includeType)
                    {
                        buffer[offset++] = 'п';
                        buffer[offset++] = ' ';
                    }
                    break;
                }
                case RussianDeclensionType.Pronoun:
                {
                    var forAdjective = this.AsPronounUnsafeRef();
                    stemType = forAdjective.StemType;
                    stress = new(forAdjective.Stress);
                    flags = forAdjective.Flags;

                    // Append the declension type identifier
                    if (includeType)
                    {
                        buffer[offset++] = 'м';
                        buffer[offset++] = 'с';
                        buffer[offset++] = ' ';
                    }
                    break;
                }
                default:
                    throw new InvalidOperationException();
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
            bool success = stress.TryFormat(buffer[offset..], out int stressStrLength, Type);
            Debug.Assert(success);
            offset += stressStrLength;

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
