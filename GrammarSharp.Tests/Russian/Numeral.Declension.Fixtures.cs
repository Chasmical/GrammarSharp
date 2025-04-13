using System;
using System.Collections.Generic;
using GrammarSharp.Russian;
using JetBrains.Annotations;

namespace GrammarSharp.Tests
{
    public partial class RussianNumeralTests
    {
        [Pure] public static FixtureAdapter<DeclensionFixture> CreateDeclensionFixtures()
        {
            FixtureAdapter<DeclensionFixture> adapter = [];

            DeclensionFixture New(int number, string noun, string nounInfo)
                => adapter.Add(new DeclensionFixture(number, noun, nounInfo));



            #region 0-123456, собака жо 3a

            New(0, "собака", "жо 3a")
                .Nom("ноль собак").Gen("нуля собак").Dat("нулю собак")
                .Acc("ноль собак").Ins("нулём собак").Prp("нуле собак");
            New(1, "собака", "жо 3a")
                .Nom("одна собака").Gen("одной собаки").Dat("одной собаке")
                .Acc("одну собаку").Ins("одной собакой").Prp("одной собаке");
            New(2, "собака", "жо 3a")
                .Nom("две собаки").Gen("двух собак").Dat("двум собакам")
                .Acc("двух собак").Ins("двумя собаками").Prp("двух собаках");
            New(3, "собака", "жо 3a")
                .Nom("три собаки").Gen("трёх собак").Dat("трём собакам")
                .Acc("трёх собак").Ins("тремя собаками").Prp("трёх собаках");
            New(5, "собака", "жо 3a")
                .Nom("пять собак").Gen("пяти собак").Dat("пяти собакам")
                .Acc("пять собак").Ins("пятью собаками").Prp("пяти собаках");

            New(13, "собака", "жо 3a")
                .Nom("тринадцать собак").Gen("тринадцати собак").Dat("тринадцати собакам")
                .Acc("тринадцать собак").Ins("тринадцатью собаками").Prp("тринадцати собаках");
            New(21, "собака", "жо 3a")
                .Nom("двадцать одна собака").Gen("двадцати одной собаки").Dat("двадцати одной собаке")
                .Acc("двадцать одну собаку").Ins("двадцатью одной собакой").Prp("двадцати одной собаке");
            New(22, "собака", "жо 3a")
                .Nom("двадцать две собаки").Gen("двадцати двух собак").Dat("двадцати двум собакам")
                .Acc("двадцать две собаки").Ins("двадцатью двумя собаками").Prp("двадцати двух собаках");
            New(24, "собака", "жо 3a")
                .Nom("двадцать четыре собаки").Gen("двадцати четырёх собак").Dat("двадцати четырём собакам")
                .Acc("двадцать четыре собаки").Ins("двадцатью четырьмя собаками").Prp("двадцати четырёх собаках");

            New(100, "собака", "жо 3a")
                .Nom("сто собак").Gen("ста собак").Dat("ста собакам")
                .Acc("сто собак").Ins("ста собаками").Prp("ста собаках");
            New(101, "собака", "жо 3a")
                .Nom("сто одна собака").Gen("ста одной собаки").Dat("ста одной собаке")
                .Acc("сто одну собаку").Ins("ста одной собакой").Prp("ста одной собаке");
            New(103, "собака", "жо 3a")
                .Nom("сто три собаки").Gen("ста трёх собак").Dat("ста трём собакам")
                .Acc("сто три собаки").Ins("ста тремя собаками").Prp("ста трёх собаках");
            New(215, "собака", "жо 3a")
                .Nom("двести пятнадцать собак").Gen("двухсот пятнадцати собак").Dat("двумстам пятнадцати собакам")
                .Acc("двести пятнадцать собак").Ins("двумястами пятнадцатью собаками").Prp("двухстах пятнадцати собаках");
            New(338, "собака", "жо 3a")
                .Nom("триста тридцать восемь собак").Gen("трёхсот тридцати восьми собак").Dat("трёмстам тридцати восьми собакам")
                .Acc("триста тридцать восемь собак").Ins("тремястами тридцатью восьмью собаками").Prp("трёхстах тридцати восьми собаках");
            New(447, "собака", "жо 3a")
                .Nom("четыреста сорок семь собак").Gen("четырёхсот сорока семи собак").Dat("четырёмстам сорока семи собакам")
                .Acc("четыреста сорок семь собак").Ins("четырьмястами сорока семью собаками").Prp("четырёхстах сорока семи собаках");
            New(809, "собака", "жо 3a")
                .Nom("восемьсот девять собак").Gen("восьмисот девяти собак").Dat("восьмистам девяти собакам")
                .Acc("восемьсот девять собак").Ins("восьмьюстами девятью собаками").Prp("восьмистах девяти собаках");

            New(1000, "собака", "жо 3a")
                .Nom("одна тысяча собак").Gen("одной тысячи собак").Dat("одной тысяче собак")
                .Acc("одну тысячу собак").Ins("одной тысячью собак").Prp("одной тысяче собак");
            New(1001, "собака", "жо 3a")
                .Nom("одна тысяча одна собака").Gen("одной тысячи одной собаки").Dat("одной тысяче одной собаке")
                .Acc("одну тысячу одну собаку").Ins("одной тысячью одной собакой").Prp("одной тысяче одной собаке");
            New(1004, "собака", "жо 3a")
                .Nom("одна тысяча четыре собаки").Gen("одной тысячи четырёх собак").Dat("одной тысяче четырём собакам")
                .Acc("одну тысячу четыре собаки").Ins("одной тысячью четырьмя собаками").Prp("одной тысяче четырёх собаках");
            New(4004, "собака", "жо 3a")
                .Nom("четыре тысячи четыре собаки").Gen("четырёх тысяч четырёх собак").Dat("четырём тысячам четырём собакам")
                .Acc("четыре тысячи четыре собаки").Ins("четырьмя тысячами четырьмя собаками").Prp("четырёх тысячах четырёх собаках");
            New(11000, "собака", "жо 3a")
                .Nom("одиннадцать тысяч собак").Gen("одиннадцати тысяч собак").Dat("одиннадцати тысячам собак")
                .Acc("одиннадцать тысяч собак").Ins("одиннадцатью тысячами собак").Prp("одиннадцати тысячах собак");
            New(23900, "собака", "жо 3a")
                .Nom("двадцать три тысячи девятьсот собак").Gen("двадцати трёх тысяч девятисот собак")
                .Dat("двадцати трём тысячам девятистам собакам").Acc("двадцать три тысячи девятьсот собак")
                .Ins("двадцатью тремя тысячами девятьюстами собаками").Prp("двадцати трёх тысячах девятистах собаках");

            New(123456, "собака", "жо 3a")
                .Nom("сто двадцать три тысячи четыреста пятьдесят шесть собак")
                .Gen("ста двадцати трёх тысяч четырёхсот пятидесяти шести собак")
                .Dat("ста двадцати трём тысячам четырёмстам пятидесяти шести собакам")
                .Acc("сто двадцать три тысячи четыреста пятьдесят шесть собак")
                .Ins("ста двадцатью тремя тысячами четырьмястами пятьюдесятью шестью собаками")
                .Prp("ста двадцати трёх тысячах четырёхстах пятидесяти шести собаках");

            #endregion



            return adapter;
        }

        public sealed class DeclensionFixture(int number, string noun, string nounInfo) : Fixture
        {
            [Obsolete(TestUtil.DeserCtor, true)] public DeclensionFixture() : this(0, null!, null!) { }

            public int Number { get; } = number;
            public string Noun { get; } = noun;
            public string NounInfo { get; } = nounInfo;

            public List<(RussianCase Case, string Expected)> TestCases { get; } = [];

            public DeclensionFixture Nom(string expected) => Returns(RussianCase.Nominative, expected);
            public DeclensionFixture Gen(string expected) => Returns(RussianCase.Genitive, expected);
            public DeclensionFixture Dat(string expected) => Returns(RussianCase.Dative, expected);
            public DeclensionFixture Acc(string expected) => Returns(RussianCase.Accusative, expected);
            public DeclensionFixture Ins(string expected) => Returns(RussianCase.Instrumental, expected);
            public DeclensionFixture Prp(string expected) => Returns(RussianCase.Prepositional, expected);

            private DeclensionFixture Returns(RussianCase @case, string expected)
            {
                TestCases.Add((@case, expected));
                if (TestCases.Count == 1) MarkAsComplete();
                return this;
            }

            public override string ToString()
                => $"{base.ToString()} {Number} — {Noun}";

        }
    }
}
