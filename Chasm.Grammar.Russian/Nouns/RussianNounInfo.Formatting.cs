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

            RussianDeclension decl = Declension;

            bool specialDeclensionProps = decl.Type != RussianDeclensionType.Noun || decl.SpecialNounProperties.HasValue;

            if (specialDeclensionProps) sb.Append('<');
            decl.RemovePluraleTantum();
            sb.Append(decl);
            if (specialDeclensionProps) sb.Append('>');

            return sb.ToString();
        }

    }
}
