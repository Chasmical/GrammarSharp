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

            New("садовый", "п 1a")
                .Returns("садовое, садового, садовому, садового, садовым, садовом, садово")
                .Returns("садовый, садового, садовому, садового, садовым, садовом, садов")
                .Returns("садовая, садовой, садовой, садовую, садовой, садовой, садова")
                .Returns("садовые, садовых, садовым, садовых, садовыми, садовых, садовы");

            New("синий", "п 2a/c")
                .Returns("синее, синего, синему, синего, синим, синем, сине")
                .Returns("синий, синего, синему, синего, синим, синем, синь")
                .Returns("синяя, синей, синей, синюю, синей, синей, синя")
                .Returns("синие, синих, синим, синих, синими, синих, сини");

            New("строгий", "п 3a/c'")
                .Returns("строгое, строгого, строгому, строгого, строгим, строгом, строго")
                .Returns("строгий, строгого, строгому, строгого, строгим, строгом, строг")
                .Returns("строгая, строгой, строгой, строгую, строгой, строгой, строга")
                .Returns("строгие, строгих, строгим, строгих, строгими, строгих, строги");

            New("летучий", "п 4a")
                .Returns("летучее, летучего, летучему, летучего, летучим, летучем, летуче")
                .Returns("летучий, летучего, летучему, летучего, летучим, летучем, летуч")
                .Returns("летучая, летучей, летучей, летучую, летучей, летучей, летуча")
                .Returns("летучие, летучих, летучим, летучих, летучими, летучих, летучи");

            New("двулицый", "п 5a")
                .Returns("двулицее, двулицего, двулицему, двулицего, двулицым, двулицем, двулице")
                .Returns("двулицый, двулицего, двулицему, двулицего, двулицым, двулицем, двулиц")
                .Returns("двулицая, двулицей, двулицей, двулицую, двулицей, двулицей, двулица")
                .Returns("двулицые, двулицых, двулицым, двулицых, двулицыми, двулицых, двулицы");

            New("голошеий", "п 6a")
                .Returns("голошеее, голошеего, голошеему, голошеего, голошеим, голошеем, голошее")
                .Returns("голошеий, голошеего, голошеему, голошеего, голошеим, голошеем, голошей")
                .Returns("голошеяя, голошеей, голошеей, голошеюю, голошеей, голошеей, голошея")
                .Returns("голошеие, голошеих, голошеим, голошеих, голошеими, голошеих, голошеи");

            // I couldn't find any 7th type adjectives, so I came up with one
            New("виий", "п 7a")
                .Returns("виее, виего, виему, виего, виим, вием, вие")
                .Returns("виий, виего, виему, виего, виим, вием, вий")
                .Returns("вияя, вией, вией, виюю, вией, вией, вия")
                .Returns("виие, виих, виим, виих, виими, виих, вии");

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
                => $"{base.ToString()} {Stem}, {(RussianAdjectiveInfo.TryParse(Info, out var info) ? info : Info)}";

        }

    }
}
