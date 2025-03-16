using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    internal static partial class RussianEndings
    {
        [Pure] public static (byte, byte) GetNounEndingIndices(RussianDeclension declension, RussianNounProperties props)
        {
            ReadOnlySpan<byte> lookup = NounLookup;

            // Get indices of both unstressed and stressed forms of endings (usually they're the same)
            int lookupIndex = ComposeNounEndingIndex(declension, props, props.Case);
            byte unStrIndex = lookup[lookupIndex];

            // Accusative case usually uses either genitive's or nominative's ending, depending on animacy.
            // In such case, the lookup yields index 0. Don't confuse with "" (encoded as 0x01: pos=1, length=0).
            if (unStrIndex == 0)
            {
                lookupIndex = ComposeNounEndingIndex(declension, props, props.IsAnimate ? RussianCase.Genitive : RussianCase.Nominative);
                unStrIndex = lookup[lookupIndex];
            }
            // Stressed ending index is right next to the unstressed one's
            byte strIndex = lookup[lookupIndex + 1];

            return (unStrIndex, strIndex);
        }

        [Pure] private static int ComposeNounEndingIndex(RussianDeclension declension, RussianNounProperties props, RussianCase @case)
        {
            // Context-dependent variables are more significant and come first, noun-dependent variables come next,
            // And finally, unstressed and stressed forms are next to each other to make stress-checking simpler.

            // Composite index: [case:6] [plural:2] [gender:3] [stem type:8] [stress:2]

            int index = (int)@case;
            index = index * 2 + (props.IsPlural ? 1 : 0);
            index = index * 3 + (int)props.Gender;
            index = index * 8 + (declension.StemType - 1);
            index *= 2; // stress takes up the least significant bit

            return index;
        }

        [Pure] public static (byte, byte) GetAdjectiveEndingIndices(RussianDeclension declension, RussianNounProperties props)
        {
            ReadOnlySpan<byte> lookup = AdjectiveLookup;

            // Get indices of both unstressed and stressed forms of endings (usually they're the same)
            int lookupIndex = ComposeAdjectiveEndingIndex(declension, props, props.Case);
            byte unStrIndex = lookup[lookupIndex];

            // Accusative case usually uses either genitive's or nominative's ending, depending on animacy.
            // In such case, the lookup yields index 0. Don't confuse with "" (encoded as 0x01: pos=1, length=0).
            if (unStrIndex == 0)
            {
                lookupIndex = ComposeAdjectiveEndingIndex(declension, props, props.IsAnimate ? RussianCase.Genitive : RussianCase.Nominative);
                unStrIndex = lookup[lookupIndex];
            }
            // Stressed ending index is right next to the unstressed one's
            byte strIndex = lookup[lookupIndex + 1];

            return (unStrIndex, strIndex);
        }

        [Pure] private static int ComposeAdjectiveEndingIndex(RussianDeclension declension, RussianNounProperties props, RussianCase @case)
        {
            // Context-dependent variables are more significant and come first, subject-driven variables come next,
            // And finally, unstressed and stressed forms are next to each other to make stress-checking simpler.

            // Composite index: [short & case:7] [plural & gender:4] [stem type:7] [stress:2]

            int index = (int)@case;
            index = index * 4 + (int)props.Gender;
            index = index * 7 + (declension.StemType - 1);
            index *= 2; // stress takes up the least significant bit

            return index;
        }

    }
}
