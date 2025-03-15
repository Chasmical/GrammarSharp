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

            // Append the noun's properties
            sb.Append(Properties).Append(' ');

            RussianDeclension decl = Declension;

            bool isSpecialDeclension = decl.Type != RussianDeclensionType.Noun || decl.SpecialNounProperties.HasValue;

            // If it's a special declension or has special declension properties, put it in braces
            if (isSpecialDeclension) sb.Append('<');

            // Append the actual declension, without tantums (and special declension properties too)
            decl.RemovePluraleTantum();
            sb.Append(decl);

            // Append the singulare tantum indicator
            if (Properties.IsSingulareTantum)
                sb.Append('—');

            // Close the declension braces
            if (isSpecialDeclension) sb.Append('>');

            return sb.ToString();
        }

    }
}
