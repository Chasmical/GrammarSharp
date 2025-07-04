using System.Collections.Generic;
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
            RussianNoun? noun = fixture.Noun is null ? null : new RussianNoun(fixture.Noun, info);

            Output.WriteLine($"{fixture.Number} — {noun?.Decline(RussianCase.Nominative, false)}\n");

            List<string> expectedList = [];
            List<string> actualList = [];

            foreach (var (@case, expected) in fixture.TestCases)
            {
                StringBuilder sb = new();
                RussianCase case2 = @case;
                bool plural = RussianGrammar.ValidateAndNormalizeCase(ref case2);

                if (noun?.Info.Properties is { } props)
                {
                    props.PrepareForDeclensionGendersAndPlural(@case, plural);
                    var agreement = RussianCardinal.DeclineInt32(sb, props, number);
                    sb.Append(' ').Append(noun.Decline(@case, agreement));
                }
                else
                {
                    var props2 = info.Properties;
                    props2.PrepareForDeclensionGendersAndPlural(@case, false);
                    RussianOrdinal.DeclineInt32(sb, props2, number);
                }

                string actual = sb.ToString();
                Output.WriteLine($"{@case.ToString().ToUpper()[..3]}: {actual}");

                expectedList.Add(expected);
                actualList.Add(actual);
            }

            Assert.Equal(expectedList.Join(" // "), actualList.Join(" // "));
        }
    }
}
