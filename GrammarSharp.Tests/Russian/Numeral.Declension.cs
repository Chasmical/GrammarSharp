using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chasm.Collections;
using GrammarSharp.Russian;
using Xunit;

namespace GrammarSharp.Tests
{
    public partial class RussianNumeralTests
    {
        [Theory, MemberData(nameof(CreateDeclensionFixtures))]
        public void Declension(DeclensionFixture fixture)
        {
            int number = fixture.Number;
            var info = RussianNounInfo.Parse(fixture.NounInfo);
            RussianNoun noun = new RussianNoun(fixture.Noun, info);

            Output.WriteLine($"{fixture.Number} — {noun.Decline(RussianCase.Nominative, false)}\n");

            List<string> expectedList = [];
            List<string> actualList = [];

            foreach (var (@case, expected) in fixture.TestCases)
            {
                string actual = RussianNumeral.CreateCardinal(number, @case, noun);
                Output.WriteLine($"{@case.ToString().ToUpper()[..3]}: {actual}");

                expectedList.Add(expected);
                actualList.Add(actual);
            }

            Assert.Equal(expectedList.Join(" // "), actualList.Join(" // "));
        }
    }
}
