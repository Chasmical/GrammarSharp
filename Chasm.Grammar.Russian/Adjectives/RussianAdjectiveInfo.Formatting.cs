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
            bool isNonAdjectiveDeclension = decl.Type != RussianDeclensionType.Adjective;

            // Append "п <", if the adjective uses a non-adjective declension
            if (isNonAdjectiveDeclension) sb.Append('п').Append(' ').Append('<');
            
            // Append the actual declension
            sb.Append(decl);

            if (isNonAdjectiveDeclension)
            {
                // Close the declension braces
                sb.Append('>');
            }
            else
            {
                // Only "pure" adjectives have short form and comparative indicators

                // Append short form difficulty indicator, if present
                RussianAdjectiveFlags shortFormFlags = Flags & RussianAdjectiveFlags.BoxedCross;
                if (shortFormFlags != 0)
                    sb.Append(shortFormFlags switch
                    {
                        RussianAdjectiveFlags.Minus => '—',
                        RussianAdjectiveFlags.Cross => '✕',
                        _ => '⌧',
                    });

                // Append the "no comparative form" indicator
                if ((Flags & RussianAdjectiveFlags.NoComparativeForm) != 0)
                    sb.Append('~');
            }

            return sb.ToString();
        }

    }
}
