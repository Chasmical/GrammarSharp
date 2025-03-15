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

            string? Decline(int num, char gender, bool plural = false)
            {
                if (num == 6) return adjective.DeclineShort(plural, new(gender, true));
                return adjective.Decline((RussianCase)num, plural, new(gender, true));
            }

            // Decline for every gender, and then for plural
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, 'n')));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, 'm')));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, 'f')));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, 'n', true)));

            string result = sb.ToString();

            Output.WriteLine(result.Split(" // ").Join("\n"));

            fixture.AssertResult(result);
        }

    }
}
