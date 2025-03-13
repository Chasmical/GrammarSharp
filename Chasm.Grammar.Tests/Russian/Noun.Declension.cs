using System.Linq;
using System.Text;
using Chasm.Collections;
using Chasm.Grammar.Russian;
using Xunit;

namespace Chasm.Grammar.Tests
{
    public partial class RussianNounTests
    {
        [Theory, MemberData(nameof(CreateDeclensionFixtures))]
        public void Declension(DeclensionFixture fixture)
        {
            var info = RussianNounInfo.Parse(fixture.Info);
            RussianNoun noun = new RussianNoun(fixture.Stem, info);

            Output.WriteLine($"{noun.Stem}, {info}\n");

            StringBuilder sb = new();
            sb.AppendJoin(", ", Enumerable.Range(0, 6).Select(i => noun.Decline((RussianCase)i, false)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 6).Select(i => noun.Decline((RussianCase)i, true)));
            string result = sb.ToString();

            Output.WriteLine(result.Split(" // ").Join("\n"));

            fixture.AssertResult(result);
        }

    }
}
