using System.Text;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianNounInfo
    {
        [Pure] public readonly override string ToString()
        {
            // Longest form (28 chars): мо-жо <мо-жо 1*°f″/f″①②③, ё>
            StringBuilder sb = new(32);

            sb.Append(Properties).Append(' ');

            bool specialDeclensionProps = Declension.SpecialNounProperties.HasValue;

            if (specialDeclensionProps) sb.Append('<');
            sb.Append(Declension);
            if (specialDeclensionProps) sb.Append('>');

            return sb.ToString();
        }

    }
}
