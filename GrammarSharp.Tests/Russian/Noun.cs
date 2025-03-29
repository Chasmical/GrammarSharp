using Xunit.Abstractions;

namespace GrammarSharp.Tests
{
    public partial class RussianNounTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

    }
}
