using NUnit.Framework;

namespace Crida.Shared.Math
{
    public class StoryTests
    {
        [Test]
        public void VectorRotationTest()
        {
            Assert.That(new EulerXyz(Angle.FromDegrees(90), Angle.Zero, Angle.FromDegrees(90)).ToTransform() *
                        new Vector3D(0, 0, 1, 1), Is.EqualTo(new Vector3D(1, 0, 0, 1)));
        }
    }
}
