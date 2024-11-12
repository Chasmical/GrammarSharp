﻿using System;
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

            #region Simple declensions 0 through 8

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
            New("бреш", "ж", "8a")
                .Returns("брешь, бреши, бреши, брешь, брешью, бреши")
                .Returns("бреши, брешей, брешам, бреши, брешами, брешах");

            #endregion

            #region Alternating vowels, *

            // A) vowel alternation type (masc any / fem 8*)
            New("сон", "м", "1*b")
                .Returns("сон, сна, сну, сон, сном, сне")
                .Returns("сны, снов, снам, сны, снами, снах");
            New("любов", "ж", "8*b'")
                .Returns("любовь, любви, любви, любовь, любовью, любви")
                .Returns("любви, любвей, любвям, любви, любвями, любвях");
            New("ветв", "ж", "8e")
                .Returns("ветвь, ветви, ветви, ветвь, ветвью, ветви")
                .Returns("ветви, ветвей, ветвям, ветви, ветвями, ветвях");
            New("боец", "мо", "5*b")
                .Returns("боец, бойца, бойцу, бойца, бойцом, бойце")
                .Returns("бойцы, бойцов, бойцам, бойцов, бойцами, бойцах");
            New("паёк", "м", "3*b")
                .Returns("паёк, пайка, пайку, паёк, пайком, пайке")
                .Returns("пайки, пайков, пайкам, пайки, пайками, пайках");
            New("уле", "м", "6*a")
                .Returns("улей, улья, улью, улей, ульем, улье")
                .Returns("ульи, ульев, ульям, ульи, ульями, ульях");
            New("зверёк", "мо", "3*b")
                .Returns("зверёк, зверька, зверьку, зверька, зверьком, зверьке")
                .Returns("зверьки, зверьков, зверькам, зверьков, зверьками, зверьках");
            New("лёд", "м", "1*b")
                .Returns("лёд, льда, льду, лёд, льдом, льде")
                .Returns("льды, льдов, льдам, льды, льдами, льдах");
            New("палец", "м", "5*a")
                .Returns("палец, пальца, пальцу, палец, пальцем, пальце")
                .Returns("пальцы, пальцев, пальцам, пальцы, пальцами, пальцах");
            New("орёл", "мо", "1*b")
                .Returns("орёл, орла, орлу, орла, орлом, орле")
                .Returns("орлы, орлов, орлам, орлов, орлами, орлах");
            New("кашел", "м", "2*a")
                .Returns("кашель, кашля, кашлю, кашель, кашлем, кашле")
                .Returns("кашли, кашлей, кашлям, кашли, кашлями, кашлях");
            New("конец", "м", "5*b")
                .Returns("конец, конца, концу, конец, концом, конце")
                .Returns("концы, концов, концам, концы, концами, концах");

            // B) vowel alternation type (neuter any / fem any, except fem 8*)
            New("гость", "жо", "6*a")
                .Returns("гостья, гостьи, гостье, гостью, гостьей, гостье")
                .Returns("гостьи, гостий, гостьям, гостий, гостьями, гостьях");
            New("ущель", "с", "6*a")
                .Returns("ущелье, ущелья, ущелью, ущелье, ущельем, ущелье")
                .Returns("ущелья, ущелий, ущельям, ущелья, ущельями, ущельях");
            New("стать", "ж", "6*b")
                .Returns("статья, статьи, статье, статью, статьёй, статье")
                .Returns("статьи, статей, статьям, статьи, статьями, статьях");
            New("пить", "с", "6*b")
                .Returns("питьё, питья, питью, питьё, питьём, питье")
                .Returns("питья, питей, питьям, питья, питьями, питьях");
            New("шпильк", "ж", "3*a")
                .Returns("шпилька, шпильки, шпильке, шпильку, шпилькой, шпильке")
                .Returns("шпильки, шпилек, шпилькам, шпильки, шпильками, шпильках");
            New("письм", "с", "1*d")
                .Returns("письмо, письма, письму, письмо, письмом, письме")
                .Returns("письма, писем, письмам, письма, письмами, письмах");
            New("чайк", "жо", "3*a")
                .Returns("чайка, чайки, чайке, чайку, чайкой, чайке")
                .Returns("чайки, чаек, чайкам, чаек, чайками, чайках");
            New("серьг", "ж", "3*f")
                .Returns("серьга, серьги, серьге, серьгу, серьгой, серьге")
                .Returns("серьги, серёг, серьгам, серьги, серьгами, серьгах");
            New("кайм", "ж", "1*b")
                .Returns("кайма, каймы, кайме, кайму, каймой, кайме")
                .Returns("каймы, каём, каймам, каймы, каймами, каймах");
            New("кольц", "с", "5*d") // Note: anomalous accent in Genitive Plural form
                .Returns("кольцо, кольца, кольцу, кольцо, кольцом, кольце")
                .Returns("кольца, колец, кольцам, кольца, кольцами, кольцах");
            New("кукл", "ж", "1*a")
                .Returns("кукла, куклы, кукле, куклу, куклой, кукле")
                .Returns("куклы, кукол, куклам, куклы, куклами, куклах");
            New("окн", "с", "1*d")
                .Returns("окно, окна, окну, окно, окном, окне")
                .Returns("окна, окон, окнам, окна, окнами, окнах");
            New("сказк", "ж", "3*a")
                .Returns("сказка, сказки, сказке, сказку, сказкой, сказке")
                .Returns("сказки, сказок, сказкам, сказки, сказками, сказках");
            New("сосн", "ж", "1*d")
                .Returns("сосна, сосны, сосне, сосну, сосной, сосне")
                .Returns("сосны, сосен, соснам, сосны, соснами, соснах");
            New("числ", "с", "1*d")
                .Returns("число, числа, числу, число, числом, числе")
                .Returns("числа, чисел, числам, числа, числами, числах");
            New("ножн", "ж", "1*a")
                .Returns("ножна, ножны, ножне, ножну, ножной, ножне")
                .Returns("ножны, ножен, ножнам, ножны, ножнами, ножнах");
            New("кишк", "ж", "3*b")
                .Returns("кишка, кишки, кишке, кишку, кишкой, кишке")
                .Returns("кишки, кишок, кишкам, кишки, кишками, кишках");
            New("овц", "жо", "5*d") // Note: anomalous accent in Genitive/Accusative Plural form
                .Returns("овца, овцы, овце, овцу, овцой, овце")
                .Returns("овцы, овец, овцам, овец, овцами, овцах");

            New("кошк", "жо", "3*a")
                .Returns("кошка, кошки, кошке, кошку, кошкой, кошке")
                .Returns("кошки, кошек, кошкам, кошек, кошками, кошках");
            New("платок", "м", "3*b")
                .Returns("платок, платка, платку, платок, платком, платке")
                .Returns("платки, платков, платкам, платки, платками, платках");
            New("сердц", "с", "5*c")
                .Returns("сердце, сердца, сердцу, сердце, сердцем, сердце")
                .Returns("сердца, сердец, сердцам, сердца, сердцами, сердцах");

            // Exception for feminine 2*a nouns, ending with 'ня'
            New("башн", "ж", "2*a")
                .Returns("башня, башни, башне, башню, башней, башне")
                .Returns("башни, башен, башням, башни, башнями, башнях");
            New("пекарн", "ж", "2*a")
                .Returns("пекарня, пекарни, пекарне, пекарню, пекарней, пекарне")
                .Returns("пекарни, пекарен, пекарням, пекарни, пекарнями, пекарнях");

            #endregion

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
