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
            New("лиц", "с", "5d")
                .Returns("лицо, лица, лицу, лицо, лицом, лице")
                .Returns("лица, лиц, лицам, лица, лицами, лицах");

            // Simple declension 6
            New("бо", "м", "6c")
                .Returns("бой, боя, бою, бой, боем, бое")
                .Returns("бои, боёв, боям, бои, боями, боях");
            New("ше", "ж", "6a")
                .Returns("шея, шеи, шее, шею, шеей, шее")
                .Returns("шеи, шей, шеям, шеи, шеями, шеях");
            // Note: it appears that all neuter 6 have vowel alternations

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
            // Note: it appears that all neuter 8 have unique stem alternations

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

            // Undocumented exceptions to the rule (2*b, 2*f, 2*②, and neuter ②)
            New("лыжн", "ж", "2*b")
                .Returns("лыжня, лыжни, лыжне, лыжню, лыжнёй, лыжне")
                .Returns("лыжни, лыжней, лыжням, лыжни, лыжнями, лыжнях");
            New("сходн", "ж", "2*a(2)")
                .Returns("сходня, сходни, сходне, сходню, сходней, сходне")
                .Returns("сходни, сходней, сходням, сходни, сходнями, сходнях");
            New("плать", "с", "6*a(2)")
                .Returns("платье, платья, платью, платье, платьем, платье")
                .Returns("платья, платьев, платьям, платья, платьями, платьях");
            New("облачк", "с", "3*c(2)")
                .Returns("облачко, облачка, облачку, облачко, облачком, облачке")
                .Returns("облачка, облачков, облачкам, облачка, облачками, облачках");
            New("жальц", "с", "5*a")
                .Returns("жальце, жальца, жальцу, жальце, жальцем, жальце")
                .Returns("жальца, жалец, жальцам, жальца, жальцами, жальцах");
            New("жальц", "с", "5*a(2)")
                .Returns("жальце, жальца, жальцу, жальце, жальцем, жальце")
                .Returns("жальца, жальцев, жальцам, жальца, жальцами, жальцах");

            #endregion

            #region Circled numbers, ①②③

            // Circled one, ①
            New("жвал", "с", "1a(1)")
                .Returns("жвало, жвала, жвалу, жвало, жвалом, жвале")
                .Returns("жвалы, жвал, жвалам, жвалы, жвалами, жвалах");
            New("горлышк", "с", "3*a(1)")
                .Returns("горлышко, горлышка, горлышку, горлышко, горлышком, горлышке")
                .Returns("горлышки, горлышек, горлышкам, горлышки, горлышками, горлышках");
            New("ветер", "м", "1*c(1)")
                .Returns("ветер, ветра, ветру, ветер, ветром, ветре")
                .Returns("ветра, ветров, ветрам, ветра, ветрами, ветрах");
            New("кра", "м", "6c(1)")
                .Returns("край, края, краю, край, краем, крае")
                .Returns("края, краёв, краям, края, краями, краях");

            // Circled two, ②
            New("облак", "с", "3c(2)")
                .Returns("облако, облака, облаку, облако, облаком, облаке")
                .Returns("облака, облаков, облакам, облака, облаками, облаках");
            New("оконц", "с", "5*a(2)")
                .Returns("оконце, оконца, оконцу, оконце, оконцем, оконце")
                .Returns("оконца, оконцев, оконцам, оконца, оконцами, оконцах");
            New("мясц", "с", "5*b(2)")
                .Returns("мясцо, мясца, мясцу, мясцо, мясцом, мясце")
                .Returns("мясца, мясцов, мясцам, мясца, мясцами, мясцах");
            New("подполь", "с", "6*a(2)")
                .Returns("подполье, подполья, подполью, подполье, подпольем, подполье")
                .Returns("подполья, подпольев, подпольям, подполья, подпольями, подпольях");
            New("жнивь", "с", "6*b(2)")
                .Returns("жнивьё, жнивья, жнивью, жнивьё, жнивьём, жнивье")
                .Returns("жнивья, жнивьёв, жнивьям, жнивья, жнивьями, жнивьях");
            New("байт", "м", "1a(2)")
                .Returns("байт, байта, байту, байт, байтом, байте")
                .Returns("байты, байт, байтам, байты, байтами, байтах");
            // Note: there are no masculine nouns with ②, of type 2/6/7/8?
            New("ноздр", "ж", "2f(2)")
                .Returns("ноздря, ноздри, ноздре, ноздрю, ноздрёй, ноздре")
                .Returns("ноздри, ноздрей, ноздрям, ноздри, ноздрями, ноздрях");
            New("сходн", "ж", "2*a(2)")
                .Returns("сходня, сходни, сходне, сходню, сходней, сходне")
                .Returns("сходни, сходней, сходням, сходни, сходнями, сходнях");

            // Circled one and two, ①②
            New("глаз", "м", "1c(1)(2)")
                .Returns("глаз, глаза, глазу, глаз, глазом, глазе")
                .Returns("глаза, глаз, глазам, глаза, глазами, глазах");
            New("очк", "с", "3*b(1)(2)")
                .Returns("очко, очка, очку, очко, очком, очке")
                .Returns("очки, очков, очкам, очки, очками, очках");

            // Circled three, ③
            New("чи", "м", "7a(3)")
                .Returns("чий, чия, чию, чий, чием, чие")
                .Returns("чии, чиев, чиям, чии, чиями, чиях");

            #endregion

            #region Unique alternations, °

            New("крестьянин", "мо", "1°a")
                .Returns("крестьянин, крестьянина, крестьянину, крестьянина, крестьянином, крестьянине")
                .Returns("крестьяне, крестьян, крестьянам, крестьян, крестьянами, крестьянах");
            New("марсианин", "мо", "1°a")
                .Returns("марсианин, марсианина, марсианину, марсианина, марсианином, марсианине")
                .Returns("марсиане, марсиан, марсианам, марсиан, марсианами, марсианах");
            New("боярин", "мо", "1°a")
                .Returns("боярин, боярина, боярину, боярина, боярином, боярине")
                .Returns("бояре, бояр, боярам, бояр, боярами, боярах");
            New("господин", "мо", "1°c(1)")
                .Returns("господин, господина, господину, господина, господином, господине")
                .Returns("господа, господ, господам, господ, господами, господах");

            New("утёнок", "мо", "3°a")
                .Returns("утёнок, утёнка, утёнку, утёнка, утёнком, утёнке")
                .Returns("утята, утят, утятам, утят, утятами, утятах");
            New("мышонок", "мо", "3°a")
                .Returns("мышонок, мышонка, мышонку, мышонка, мышонком, мышонке")
                .Returns("мышата, мышат, мышатам, мышат, мышатами, мышатах");

            New("щенок", "мо", "3°d")
                .Returns("щенок, щенка, щенку, щенка, щенком, щенке")
                .Returns("щенята, щенят, щенятам, щенят, щенятами, щенятах");
            New("внучок", "мо", "3°b")
                .Returns("внучок, внучка, внучку, внучка, внучком, внучке")
                .Returns("внучата, внучат, внучатам, внучат, внучатами, внучатах");

            New("поросёночек", "мо", "3°a")
                .Returns("поросёночек, поросёночка, поросёночку, поросёночка, поросёночком, поросёночке")
                .Returns("поросятки, поросяток, поросяткам, поросяток, поросятками, поросятках");

            New("врем", "с", "8°c, ё")
                .Returns("время, времени, времени, время, временем, времени")
                .Returns("времена, времён, временам, времена, временами, временах");
            New("им", "с", "8°c, ё")
                .Returns("имя, имени, имени, имя, именем, имени")
                .Returns("имена, имён, именам, имена, именами, именах");

            #endregion

            #region Alternating ё

            // Alternation with ё in the stem
            New("ёж", "мо", "4b, ё")
                .Returns("ёж, ежа, ежу, ежа, ежом, еже")
                .Returns("ежи, ежей, ежам, ежей, ежами, ежах");
            New("делёж", "мо", "4b, ё")
                .Returns("делёж, дележа, дележу, дележа, дележом, дележе")
                .Returns("дележи, дележей, дележам, дележей, дележами, дележах");
            New("черёд", "м", "1b, ё")
                .Returns("черёд, череда, череду, черёд, чередом, череде")
                .Returns("череды, чередов, чередам, череды, чередами, чередах");
            New("шёлк", "м", "3c(1), ё")
                .Returns("шёлк, шёлка, шёлку, шёлк, шёлком, шёлке")
                .Returns("шелка, шелков, шелкам, шелка, шелками, шелках");
            New("жёлуд", "м", "2e, ё")
                .Returns("жёлудь, жёлудя, жёлудю, жёлудь, жёлудем, жёлуде")
                .Returns("жёлуди, желудей, желудям, жёлуди, желудями, желудях");
            New("щёлоч", "ж", "8e, ё")
                .Returns("щёлочь, щёлочи, щёлочи, щёлочь, щёлочью, щёлочи")
                .Returns("щёлочи, щелочей, щелочам, щёлочи, щелочами, щелочах");

            // Alternation without ё in the stem
            New("стег", "ж", "3b, ё")
                .Returns("стега, стеги, стеге, стегу, стегой, стеге")
                .Returns("стеги, стёг, стегам, стеги, стегами, стегах");
            New("серед", "ж", "1f', ё")
                .Returns("середа, середы, середе, середу, середой, середе")
                .Returns("середы, серёд, середам, середы, середами, середах");
            New("черед", "ж", "1b, ё")
                .Returns("череда, череды, череде, череду, чередой, череде")
                .Returns("череды, черёд, чередам, череды, чередами, чередах");
            New("сел", "с", "1d, ё")
                .Returns("село, села, селу, село, селом, селе")
                .Returns("сёла, сёл, сёлам, сёла, сёлами, сёлах");
            New("веретен", "с", "1d, ё")
                .Returns("веретено, веретена, веретену, веретено, веретеном, веретене")
                .Returns("веретёна, веретён, веретёнам, веретёна, веретёнами, веретёнах");
            New("жен", "жо", "1d, ё")
                .Returns("жена, жены, жене, жену, женой, жене")
                .Returns("жёны, жён, жёнам, жён, жёнами, жёнах");
            New("слез", "ж", "1f, ё")
                .Returns("слеза, слезы, слезе, слезу, слезой, слезе")
                .Returns("слёзы, слёз, слезам, слёзы, слезами, слезах");
            New("желез", "ж", "1f, ё")
                .Returns("железа, железы, железе, железу, железой, железе")
                .Returns("железы, желёз, железам, железы, железами, железах");

            // Alternation without ё in the stem, and also with alternating vowels
            New("стекл", "с", "1*d, ё")
                .Returns("стекло, стекла, стеклу, стекло, стеклом, стекле")
                .Returns("стёкла, стёкол, стёклам, стёкла, стёклами, стёклах");
            New("бронестекл", "с", "1*d, ё")
                .Returns("бронестекло, бронестекла, бронестеклу, бронестекло, бронестеклом, бронестекле")
                .Returns("бронестёкла, бронестёкол, бронестёклам, бронестёкла, бронестёклами, бронестёклах");
            New("метл", "ж", "1*d, ё")
                .Returns("метла, метлы, метле, метлу, метлой, метле")
                .Returns("мётлы, мётел, мётлам, мётлы, мётлами, мётлах");
            New("сестр", "жо", "1*d, ё") // Genitive Plural form is anomalous (сестёр), but we'll ignore that here
                .Returns("сестра, сестры, сестре, сестру, сестрой, сестре")
                .Returns("сёстры, сёстер, сёстрам, сёстер, сёстрами, сёстрах");

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
