using System.Linq;
using System.Text;
using Chasm.Collections;
using GrammarSharp.Russian;
using Xunit;

namespace GrammarSharp.Tests
{
    public partial class RussianAdjectiveTests
    {
        [Theory, MemberData(nameof(CreateDeclensionFixtures))]
        public void Declension(DeclensionFixture fixture)
        {
            var info = RussianAdjectiveInfo.Parse(fixture.Info);
            var adjective = new RussianAdjective(fixture.Stem, info);

            Output.WriteLine($"{adjective.Stem}, {info}\n");

            StringBuilder sb = new();

            string? Decline(int num, RussianGender gender, bool plural = false)
            {
                if (num == 6) return adjective.DeclineShort(plural, new(gender, true));
                return adjective.Decline((RussianCase)num, plural, new(gender, true));
            }

            // Decline for every gender, and then for plural
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, RussianGender.Neuter)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, RussianGender.Masculine)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, RussianGender.Feminine)));
            sb.Append(" // ");
            sb.AppendJoin(", ", Enumerable.Range(0, 7).Select(i => Decline(i, RussianGender.Neuter, true)));

            string result = sb.ToString();

            Output.WriteLine(result.Split(" // ").Join("\n"));

            fixture.AssertResult(result);
        }

        [Fact]
        public void DifficultShortForms()
        {
            var info = RussianAdjectiveInfo.Parse("п 1a—");
            var adjective = new RussianAdjective("углекислый", info);

            Assert.Equal("углекисла", adjective.DeclineShort(false, new("жо")));
            Assert.Equal("углекисла", adjective.DeclineShort(false, new("жо"), true));
            Assert.Null(adjective.DeclineShort(false, new("мо")));
            Assert.Equal("углекисл", adjective.DeclineShort(false, new("мо"), true));

            info = RussianAdjectiveInfo.Parse("п 4a✕");
            adjective = new RussianAdjective("лучший", info);

            Assert.Null(adjective.DeclineShort(false, new("жо")));
            Assert.Equal("лучша", adjective.DeclineShort(false, new("жо"), true));
            Assert.Null(adjective.DeclineShort(false, new("мо")));
            Assert.Equal("лучш", adjective.DeclineShort(false, new("мо"), true));

            info = RussianAdjectiveInfo.Parse("п 1b⌧");
            adjective = new RussianAdjective("золотой", info);

            Assert.Null(adjective.DeclineShort(false, new("жо")));
            Assert.Equal("золота", adjective.DeclineShort(false, new("жо"), true));
            Assert.Null(adjective.DeclineShort(false, new("мо")));
            Assert.Null(adjective.DeclineShort(false, new("мо"), true));

        }

        [Fact]
        public void ComparativeForms()
        {
            var info = RussianAdjectiveInfo.Parse("п 1a/c");
            var adjective = new RussianAdjective("живой", info);
            Assert.Equal("живее", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 3a/c'");
            adjective = new RussianAdjective("мягкий", info);
            Assert.Equal("мягче", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 3a/c'");
            adjective = new RussianAdjective("строгий", info);
            Assert.Equal("строже", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 3a/c'");
            adjective = new RussianAdjective("тихий", info);
            Assert.Equal("тише", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 3*a/c, ё");
            adjective = new RussianAdjective("жёсткий", info);
            Assert.Equal("жёстче", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 1a/c\", ё");
            adjective = new RussianAdjective("мёртвый", info);
            Assert.Equal("мертвее", adjective.DeclineComparative());

            info = RussianAdjectiveInfo.Parse("п 3*a', ё");
            adjective = new RussianAdjective("чёткий", info);
            Assert.Equal("чётче", adjective.DeclineComparative());

        }

    }
}
