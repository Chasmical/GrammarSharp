using Xunit.Abstractions;

namespace GrammarSharp.Tests
{
    public partial class RussianNumeralTests(ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; } = output;

    }
}
