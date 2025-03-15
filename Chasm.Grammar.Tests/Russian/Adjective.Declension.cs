using System.Linq;
using System.Text;
using Chasm.Collections;
using Chasm.Grammar.Russian;
using Xunit;

namespace Chasm.Grammar.Tests
{
    public partial class RussianAdjectiveTests
    {
        [Theory, MemberData(nameof(CreateDeclensionFixtures))]
        public void Declension(DeclensionFixture fixture)
        {
            var info = RussianAdjectiveInfo.Parse(fixture.Info);
            RussianAdjective adjective = new RussianAdjective(fixture.Stem, info);

            Output.WriteLine($"{adjective.Stem}, {info}\n");

            StringBuilder sb = new();

            string Decline(RussianGender gender, int num)
                => adjective.Decline((RussianCase)num, gender == RussianGender.Common, new(gender, true));

            // Decline for every gender, and then for plural
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(RussianGender.Neuter, i)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(RussianGender.Masculine, i)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(RussianGender.Feminine, i)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(RussianGender.Common, i)));

            string result = sb.ToString();

            Output.WriteLine(result.Split(" // ").Join("\n"));

            fixture.AssertResult(result);
        }

    }
}
