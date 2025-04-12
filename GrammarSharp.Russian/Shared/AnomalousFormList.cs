using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    internal struct AnomalousFormList
    {
        private (int Index, string Form)[]? _anomalies;

        [Pure] private readonly string? Get(int index)
        {
            if (_anomalies is { } anomalies)
            {
                for (int i = 0; i < anomalies.Length; i++)
                    if (anomalies[i].Index == index)
                        return anomalies[i].Form;
            }
            return null;
        }
        private void Set(int value, string? form)
        {
            if (_anomalies is not null)
                Set2(value, form);
            else if (form is not null)
                _anomalies = [(value, form)];
        }
        private void Set2(int index, string? form)
        {
            var newAnomalies = _anomalies!;

            for (int i = 0; i < newAnomalies.Length; i++)
                if (newAnomalies[i].Index == index)
                {
                    if (form is not null)
                    {
                        newAnomalies[i].Form = form;
                        return;
                    }
                    var old = newAnomalies;
                    newAnomalies = new (int, string)[old.Length - 1];
                    Array.Copy(old, 0, newAnomalies, 0, i);
                    Array.Copy(old, i + 1, newAnomalies, i, old.Length - i - 1);
                    _anomalies = newAnomalies;
                    break;
                }

            if (form is null) return;
            Array.Resize(ref newAnomalies, newAnomalies.Length + 1);
            newAnomalies[^1] = (index, form);
            _anomalies = newAnomalies;
        }

        // Form indices for nouns:
        //  9*0 + 0-8 - all 9 cases, singular
        //  9*1 + 0-8 - all 9 cases, plural
        //  9*2 + 0-1 - count form, paucal and plural

        [Pure] public readonly string? GetForNoun(RussianCase @case, bool plural)
            => Get((plural ? 9 : 0) + (int)@case);
        public void SetForNoun(RussianCase @case, bool plural, string? form)
            => Set((plural ? 9 : 0) + (int)@case, form);

        [Pure] public readonly string? GetCountFormForNoun(bool plural)
            => Get(9 * 2 + (plural ? 1 : 0));
        public void SetCountFormForNoun(bool plural, string? form)
            => Set(9 * 2 + (plural ? 1 : 0), form);

        // Form indices for adjectives:
        //  6*0 + 0-5 - neuter, main 6 cases
        //  6*1 + 0-5 - masculine, main 6 cases
        //  6*2 + 0-5 - feminine, main 6 cases
        //  6*3 + 0-5 - plural, main 6 cases
        //  6*4 + 0-3 - short form, genders/count
        //  6*4 +  4  - comparative form

        [Pure] public readonly string? GetForAdjective(RussianCase @case, RussianGender gender)
            => Get((int)gender * 6 + (int)@case);
        public void Set(RussianCase @case, RussianGender gender, string? form)
            => Set((int)gender * 6 + (int)@case, form);

        [Pure] public readonly string? GetShortFormForAdjective(RussianGender gender)
            => Get(4 * 6 + (int)gender);
        public void SetShortFormForAdjective(RussianGender gender, string? form)
            => Set(4 * 6 + (int)gender, form);
        [Pure] public readonly string? GetComparativeForAdjective()
            => Get(4 * 6 + 4);
        public void SetComparativeForAdjective(string? form)
            => Set(4 * 6 + 4, form);

    }
}
