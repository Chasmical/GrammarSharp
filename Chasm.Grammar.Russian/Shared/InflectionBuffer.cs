using System;

namespace Chasm.Grammar.Russian
{
    internal ref struct InflectionBuffer
    {
        public readonly Span<char> Buffer;
        public int StemLength;
        private int ResultLength;

        public InflectionBuffer(Span<char> buffer)
            => Buffer = buffer;

        public readonly Span<char> Stem => Buffer[..StemLength];
        public readonly Span<char> Ending => Buffer[StemLength..ResultLength];
        public readonly Span<char> Result => Buffer[..ResultLength];

        public void WriteInitialParts(ReadOnlySpan<char> stem, ReadOnlySpan<char> ending)
        {
            StemLength = stem.Length;
            ResultLength = StemLength + ending.Length;

            stem.CopyTo(Buffer);
            ending.CopyTo(Buffer[stem.Length..]);
        }

        public void RemoveStemCharAt(int index)
        {
            for (int i = index + 1; i < ResultLength; i++)
                Buffer[i - 1] = Buffer[i];
            StemLength--;
            ResultLength--;
        }
        public void ShrinkStemBy(int offset)
        {
            StemLength -= offset;
            ResultLength -= offset;
            for (int i = StemLength; i < ResultLength; i++)
                Buffer[i] = Buffer[i + offset];
        }
        public void AppendToStem(char a, char b)
        {
            for (int i = ResultLength - 1; i >= StemLength; i--)
                Buffer[i + 2] = Buffer[i];
            Buffer[StemLength] = a;
            Buffer[StemLength + 1] = b;
            StemLength += 2;
            ResultLength += 2;
        }
        public void AppendToEnding(char a, char b)
        {
            Buffer[ResultLength++] = a;
            Buffer[ResultLength++] = b;
        }

        public void RemoveEnding()
            => ResultLength = StemLength;
        public void ReplaceEnding(char a)
        {
            Buffer[StemLength] = a;
            ResultLength = StemLength + 1;
        }
        public void ReplaceEnding(char a, char b)
        {
            Buffer[StemLength] = a;
            Buffer[StemLength + 1] = b;
            ResultLength = StemLength + 2;
        }

        public void InsertBetweenTwoLastStemChars(char ch)
        {
            for (int i = ResultLength; i >= StemLength; i--)
                Buffer[i] = Buffer[i - 1];

            Buffer[StemLength - 1] = ch;

            StemLength++;
            ResultLength++;
        }
    }
}
