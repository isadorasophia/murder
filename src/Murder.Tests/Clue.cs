using Microsoft.VisualStudio.TestTools.UnitTesting;
using Murder.Core.Geometry;
using Murder.Services;
using System.Numerics;

namespace Murder.Tests;

[TestClass]
public class Clue
{
    [TestMethod]
    public void TestCircleVsCircle()
    {
        CircleShape c1 = new(2, new(0, 0));
        CircleShape c2 = new(2, new(1, 1));

        bool collides = PhysicsServices.CollidesWith(c1, Point.Zero, Vector2.One, c2, Point.Zero, Vector2.One);
        Assert.AreEqual(true, collides);
    }

    [TestMethod]
    public void TestPolygonVsPolygon1()
    {
        // Polygon inside another
        PolygonShape p1 = new(new([new(0, -5), new(7, 1), new(0, 6), new(-7, 1)]));
        PolygonShape p2 = new(new([new(15, -92), new(94, -116), new(171, -91), new(173, 34), new(15, 37)]));

        bool collides = PhysicsServices.CollidesWith(
            p1, position1: new(294, 230), Vector2.One, 
            p2, position2: new(181, 265), Vector2.One);
        Assert.AreEqual(true, collides);
    }
}