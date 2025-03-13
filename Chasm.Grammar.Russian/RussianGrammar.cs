using System;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public static class RussianGrammar
    {
        [Pure] public static RussianGender ParseGender(char gender)
        {
            return gender switch
            {
                'n' or 'с' => RussianGender.Neuter,
                'f' or 'ж' => RussianGender.Feminine,
                'm' or 'м' => RussianGender.Masculine,
                'c' or 'о' => RussianGender.Common,
                _ => ThrowOutOfRange(gender),
            };

            static RussianGender ThrowOutOfRange(char gender)
            {
                string msg = $"{nameof(gender)} ({gender}) is not a valid Russian grammatical gender.";
                throw new ArgumentOutOfRangeException(nameof(gender), gender, msg);
            }
        }

    }
}
