using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TrapTest
{
    private GameObject trapObj;
    private Trap trap;
    private GameObject borderObj;

    [SetUp]
    public void Setup()
    {
        trapObj = new GameObject("TestTrap");
        trap = trapObj.AddComponent<Trap>();
        trap.TileRenderer = trapObj.AddComponent<SpriteRenderer>();
        trapObj.AddComponent<BoxCollider2D>();

        borderObj = new GameObject("Border");
        trap.border = borderObj;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(borderObj);
        Object.DestroyImmediate(trapObj);
    }

    [TestFixture]
    public class Inheritance : TrapTest
    {
        [Test]
        public void ShouldInheritFromTile()
        {
            Assert.IsTrue(trap is Tile);
            Assert.IsInstanceOf<Tile>(trap);
        }

        [Test]
        public void ShouldHaveAccessToTileProperties()
        {
            Assert.IsNotNull(trap.TileRenderer);
            Assert.AreEqual(trap.GetComponent<SpriteRenderer>(), trap.TileRenderer);
        }
    }

    [TestFixture]
    public class SetBorder : TrapTest
    {
        [Test]
        public void ShouldActivateBorder_WhenTrapIsActive()
        {
            trap.isActive = true;

            trap.SetBorder();

            Assert.IsTrue(trap.border.activeSelf);
        }

        [Test]
        public void ShouldDeactivateBorder_WhenTrapIsNotActive()
        {
            trap.isActive = false;

            trap.SetBorder();

            Assert.IsFalse(trap.border.activeSelf);
        }

        [Test]
        public void ShouldToggleBorder_WhenActiveStateChanges()
        {
            trap.isActive = false;
            trap.SetBorder();
            Assert.IsFalse(trap.border.activeSelf);

            trap.isActive = true;
            trap.SetBorder();
            Assert.IsTrue(trap.border.activeSelf);

            trap.isActive = false;
            trap.SetBorder();
            Assert.IsFalse(trap.border.activeSelf);
        }
    }
}
