using System;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.Grammar.Tests
{
    public partial class RussianNounTests
    {
        [Pure] public static FixtureAdapter<DeclensionFixture> CreateDeclensionFixtures()
        {
            FixtureAdapter<DeclensionFixture> adapter = [];

            DeclensionFixture New(string stem, string info, string declension)
                => adapter.Add(new DeclensionFixture(stem, info, declension));



            // Declension 0
            New("кофе", "с", "0")
                .Returns("кофе, кофе, кофе, кофе, кофе, кофе")
                .Returns("кофе, кофе, кофе, кофе, кофе, кофе");

            // Simple declension 1
            New("топор", "м", "1b")
                .Returns("топор, топора, топору, топор, топором, топоре")
                .Returns("топоры, топоров, топорам, топоры, топорами, топорах");
            New("кобр", "жо", "1a")
                .Returns("кобра, кобры, кобре, кобру, коброй, кобре")
                .Returns("кобры, кобр, кобрам, кобр, кобрами, кобрах");
            New("олов", "с", "1a")
                .Returns("олово, олова, олову, олово, оловом, олове")
                .Returns("олова, олов, оловам, олова, оловами, оловах");

            // Simple declension 2
            New("искател", "мо", "2a")
                .Returns("искатель, искателя, искателю, искателя, искателем, искателе")
                .Returns("искатели, искателей, искателям, искателей, искателями, искателях");
            New("бан", "ж", "2a")
                .Returns("баня, бани, бане, баню, баней, бане")
                .Returns("бани, бань, баням, бани, банями, банях");
            New("пол", "с", "2c")
                .Returns("поле, поля, полю, поле, полем, поле")
                .Returns("поля, полей, полям, поля, полями, полях");

            // Simple declension 3
            New("блинчик", "мо", "3a")
                .Returns("блинчик, блинчика, блинчику, блинчика, блинчиком, блинчике")
                .Returns("блинчики, блинчиков, блинчикам, блинчиков, блинчиками, блинчиках");
            New("коряг", "ж", "3a")
                .Returns("коряга, коряги, коряге, корягу, корягой, коряге")
                .Returns("коряги, коряг, корягам, коряги, корягами, корягах");
            New("войск", "с", "3c")
                .Returns("войско, войска, войску, войско, войском, войске")
                .Returns("войска, войск, войскам, войска, войсками, войсках");

            // Simple declension 4
            New("калач", "м", "4b")
                .Returns("калач, калача, калачу, калач, калачом, калаче")
                .Returns("калачи, калачей, калачам, калачи, калачами, калачах");
            New("галош", "ж", "4a")
                .Returns("галоша, галоши, галоше, галошу, галошей, галоше")
                .Returns("галоши, галош, галошам, галоши, галошами, галошах");
            New("жилищ", "с", "4a")
                .Returns("жилище, жилища, жилищу, жилище, жилищем, жилище")
                .Returns("жилища, жилищ, жилищам, жилища, жилищами, жилищах");

            // Simple declension 5
            New("кузнец", "мо", "5b")
                .Returns("кузнец, кузнеца, кузнецу, кузнеца, кузнецом, кузнеце")
                .Returns("кузнецы, кузнецов, кузнецам, кузнецов, кузнецами, кузнецах");
            New("девиц", "жо", "5a")
                .Returns("девица, девицы, девице, девицу, девицей, девице")
                .Returns("девицы, девиц, девицам, девиц, девицами, девицах");

            // Simple declension 6
            New("бо", "м", "6c")
                .Returns("бой, боя, бою, бой, боем, бое")
                .Returns("бои, боёв, боям, бои, боями, боях");
            New("ше", "ж", "6a")
                .Returns("шея, шеи, шее, шею, шеей, шее")
                .Returns("шеи, шей, шеям, шеи, шеями, шеях");

            // Simple declension 7
            New("полони", "м", "7a")
                .Returns("полоний, полония, полонию, полоний, полонием, полонии")
                .Returns("полонии, полониев, полониям, полонии, полониями, полониях");
            New("маги", "ж", "7a")
                .Returns("магия, магии, магии, магию, магией, магии")
                .Returns("магии, магий, магиям, магии, магиями, магиях");
            New("сложени", "с", "7a")
                .Returns("сложение, сложения, сложению, сложение, сложением, сложении")
                .Returns("сложения, сложений, сложениям, сложения, сложениями, сложениях");

            // Simple declension 8
            New("пут", "м", "8b")
                .Returns("путь, пути, пути, путь, путём, пути")
                .Returns("пути, путей, путям, пути, путями, путях");
            New("бол", "ж", "8a")
                .Returns("боль, боли, боли, боль, болью, боли")
                .Returns("боли, болей, болям, боли, болями, болях");



            // Alternating vowels, *

            New("кукл", "ж", "1*a")
                .Returns("кукла, куклы, кукле, куклу, куклой, кукле")
                .Returns("куклы, кукол, куклам, куклы, куклами, куклах");
            New("кошк", "жо", "3*a")
                .Returns("кошка, кошки, кошке, кошку, кошкой, кошке")
                .Returns("кошки, кошек, кошкам, кошек, кошками, кошках");
            New("платок", "м", "3*b")
                .Returns("платок, платка, платку, платок, платком, платке")
                .Returns("платки, платков, платкам, платки, платками, платках");
            New("сердц", "с", "5*c")
                .Returns("сердце, сердца, сердцу, сердце, сердцем, сердце")
                .Returns("сердца, сердец, сердцам, сердца, сердцами, сердцах");



            return adapter;
        }

        public sealed class DeclensionFixture(string stem, string info, string declension) : FuncFixture<string>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public DeclensionFixture() : this(null!, null!, null!) { }

            public string Stem { get; } = stem;
            public string Info { get; } = info;
            public string Declension { get; } = declension;

            public string? Expected { get; private set; }

            public DeclensionFixture Returns(string expected)
            {
                bool willBeComplete = Expected is not null;
                Expected = Expected is null ? expected : Expected + " // " + expected;
                if (willBeComplete) MarkAsComplete();
                return this;
            }
            public override void AssertResult(string? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Expected, result);
            }
            public override string ToString()
                => $"{base.ToString()} {Stem}, {Info}, {Declension}";

        }

    }
}
