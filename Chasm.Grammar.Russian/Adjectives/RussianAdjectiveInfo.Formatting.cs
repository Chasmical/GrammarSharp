using System.Text;
using JetBrains.Annotations;

namespace Chasm.Grammar.Russian
{
    public partial struct RussianAdjectiveInfo
    {
        [Pure] public readonly override string ToString()
        {
            StringBuilder sb = new();

            RussianDeclension decl = Declension;
            bool specialDeclensionProps = decl.Type != RussianDeclensionType.Adjective;

            if (specialDeclensionProps) sb.Append('п').Append(' ').Append('<');
            sb.Append(decl);
            if (specialDeclensionProps) sb.Append('>');

            return sb.ToString();
        }

    }
}
