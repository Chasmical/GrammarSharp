using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GrammarSharp.Russian
{
    internal static class RussianDeclensionUnsafeExtensions
    {
        [Pure] public static ref readonly RussianNounDeclension AsNounUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianNounDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref readonly RussianAdjectiveDeclension AsAdjectiveUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianAdjectiveDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref readonly RussianPronounDeclension AsPronounUnsafeRef(ref readonly this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianPronounDeclension>(ref Unsafe.AsRef(in decl));
        
        [Pure] public static ref RussianNounDeclension AsNounUnsafeRefMutable(ref this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianNounDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref RussianAdjectiveDeclension AsAdjectiveUnsafeRefMutable(ref this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianAdjectiveDeclension>(ref Unsafe.AsRef(in decl));
        [Pure] public static ref RussianPronounDeclension AsPronounUnsafeRefMutable(ref this RussianDeclension decl)
            => ref Unsafe.As<RussianDeclension, RussianPronounDeclension>(ref Unsafe.AsRef(in decl));

    }
}
