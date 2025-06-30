using Microsoft.VisualStudio.TestTools.UnitTesting;
using Murder.Core.Geometry;
using Murder.Services;

namespace Murder.Tests;

[TestClass]
public class Clue
{
    [TestMethod]
    public void TestCircleVsCircle()
    {
        CircleShape c1 = new(2, new(0, 0));
        CircleShape c2 = new(2, new(1, 1));

        bool collides = PhysicsServices.CollidesWith(c1, Point.Zero, c2, Point.Zero);
        Assert.AreEqual(true, collides);
    }
}