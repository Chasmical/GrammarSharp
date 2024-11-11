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



            New("кукл", "ж", "1*a")
                .Returns("кукла, куклы, кукле, куклу, куклой, кукле")
                .Returns("куклы, кукол, куклам, куклы, куклами, куклах");



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
