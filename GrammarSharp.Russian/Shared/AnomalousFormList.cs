using System;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    internal struct AnomalousFormList
    {
        private (int Index, string Form)[]? _anomalies;

        [Pure] private static int FindIndex((int Index, string Form)[] array, int index)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Index == index)
                    return i;
            return -1;
        }

        [Pure] public readonly string? Get(int value)
        {
            if (_anomalies is { } anomalies)
            {
                int index = FindIndex(anomalies, value);
                return index >= 0 ? anomalies[index].Form : null;
            }
            return null;
        }
        public void Set(int value, string? form)
        {
            var newAnomalies = _anomalies;
            if (newAnomalies is null)
            {
                if (form is null) return;
                newAnomalies = [(value, form)];
            }
            else
            {
                int existingIndex = FindIndex(newAnomalies, value);
                if (existingIndex >= 0)
                {
                    if (form is not null)
                    {
                        newAnomalies[existingIndex].Form = form;
                        return;
                    }
                    var old = newAnomalies;
                    newAnomalies = new (int, string)[old.Length - 1];
                    Array.Copy(old, 0, newAnomalies, 0, existingIndex);
                    Array.Copy(old, existingIndex + 1, newAnomalies, existingIndex, old.Length - existingIndex - 1);
                }
                else
                {
                    if (form is null) return;
                    Array.Resize(ref newAnomalies, newAnomalies.Length + 1);
                    newAnomalies[^1] = (value, form);
                }
            }
            _anomalies = newAnomalies;
        }

    }
}
