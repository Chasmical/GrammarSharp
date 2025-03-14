using System;
using System.Linq;
using Chasm.Grammar.Russian;
using JetBrains.Annotations;
using Xunit;

namespace Chasm.Grammar.Tests
{
    public partial class RussianAdjectiveTests
    {
        [Pure] public static FixtureAdapter<DeclensionFixture> CreateDeclensionFixtures()
        {
            FixtureAdapter<DeclensionFixture> adapter = [];

            DeclensionFixture New(string stem, string info)
                => adapter.Add(new DeclensionFixture(stem, info));

            #region Simple stem types 0 through 7

            New("хаки", "п 0")
                .Returns("хаки, хаки, хаки, хаки, хаки, хаки, хаки")
                .Returns("хаки, хаки, хаки, хаки, хаки, хаки, хаки")
                .Returns("хаки, хаки, хаки, хаки, хаки, хаки, хаки")
                .Returns("хаки, хаки, хаки, хаки, хаки, хаки, хаки");

            #endregion

            #region Alternating vowels, *

            #endregion

            #region Circled numbers, ①②

            #endregion

            #region Alternating ё

            #endregion

            return adapter;
        }

        public sealed class DeclensionFixture(string stem, string info) : FuncFixture<string>
        {
            [Obsolete(TestUtil.DeserCtor, true)] public DeclensionFixture() : this(null!, null!) { }

            public string Stem { get; } = stem;
            public string Info { get; } = info;

            public string? Expected { get; private set; }

            public DeclensionFixture Returns(string expected)
            {
                Expected = Expected is null ? expected : Expected + " // " + expected;
                if (Expected?.Count(ch => ch == '/') == 6) MarkAsComplete();
                return this;
            }
            public override void AssertResult(string? result)
            {
                Assert.NotNull(result);
                Assert.Equal(Expected, result);
            }
            public override string ToString()
                => $"{base.ToString()} {Stem}, {RussianAdjectiveInfo.Parse(Info)}";

        }

    }
}
