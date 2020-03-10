using NUnit.Framework;
using static System.Math;

namespace Crida.Shared.Math
{
    public class AngleTests
    {
        [Test]
        public void ConstructorTest()
        {
            var expected = Angle.FromDegrees(30).Radians;
            const double error = 0.0001;
            Assert.AreEqual(expected, Angle.FromDegrees(360 * 42 + 30).Radians, error);
            Assert.AreEqual(expected, Angle.FromDegrees(-360 * 99 + 30).Radians, error);
            Assert.AreEqual(expected, Angle.FromRadians(1d / 6 * PI).Radians, error);
            Assert.AreEqual(expected, Angle.FromRadians((1d / 6 + 32) * PI).Radians, error);
            Assert.AreEqual(expected, Angle.FromRadians((1d / 6 - 18) * PI).Radians, error);
        }
    }
}
