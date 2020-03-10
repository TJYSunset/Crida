using System.Linq;
using NUnit.Framework;

namespace Crida.Shared.Utilities
{
    public class LinqExtensionsTests
    {
        [Test]
        public void OverflowSplitTest()
        {
            Assert.That(new[] {0, 128, 128, 0, 1, 2, 3, 4, 1, 2, 9, 9, 1, 0, 0, 128, 1}.OverflowSplit(x => x, 10),
                Is.EquivalentTo(new[]
                {
                    new[] {0, 128},
                    new[] {128, 0},
                    new[] {1, 2, 3, 4},
                    new[] {1, 2},
                    new[] {9},
                    new[] {9, 1, 0, 0},
                    new[] {128},
                    new[] {1}
                }));
            Assert.That(new int[0].OverflowSplit(x => x, 10).Count(),
                Is.Zero);
        }
    }
}
