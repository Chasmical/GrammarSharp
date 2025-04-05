using System.Text;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    public partial struct RussianAdjectiveInfo
    {
        /// <summary>
        ///   <para>Returns a string representation of this adjective info.</para>
        /// </summary>
        /// <returns>The string representation of this adjective info.</returns>
        [Pure] public readonly override string ToString()
        {
            // TODO: use a span buffer to format RussianAdjectiveInfo
            StringBuilder sb = new();

            RussianDeclension decl = Declension;
            RussianAdjectiveFlags flags = Flags;
            RussianDeclensionType defaultDeclensionType = RussianDeclensionType.Adjective;

            const RussianAdjectiveFlags typeFlags = RussianAdjectiveFlags.IsNumeral | RussianAdjectiveFlags.IsPronoun;
            if ((flags & typeFlags) != 0)
            {
                if ((flags & typeFlags) == RussianAdjectiveFlags.IsNumeral)
                {
                    sb.Append("числ.-п");
                    // Always specify numeral adjectives' declension as irregular
                    defaultDeclensionType = (RussianDeclensionType)(-1);
                }
                else
                {
                    sb.Append("мс-п");
                    defaultDeclensionType = RussianDeclensionType.Pronoun;
                }
            }
            else
            {
                sb.Append('п');
            }
            sb.Append(' ');

            bool irregularDeclensionType = decl.Type != defaultDeclensionType;

            // Append " <", if the adjective uses an irregular declension
            if (irregularDeclensionType) sb.Append('<');

            // Append the actual declension
            // TODO: format RussianDeclension directly into span buffer
            sb.Append(decl.ToString(irregularDeclensionType));

            if (irregularDeclensionType)
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
