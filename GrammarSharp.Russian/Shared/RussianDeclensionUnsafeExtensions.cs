using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    internal static class RussianDeclensionUnsafeExtensions
    {
        [Pure] public static ref RussianNounDeclension AsNounUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianNounDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref RussianAdjectiveDeclension AsAdjectiveUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianAdjectiveDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref RussianPronounDeclension AsPronounUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianPronounDeclension>(ref Unsafe.AsRef(in decl));

    }
}
