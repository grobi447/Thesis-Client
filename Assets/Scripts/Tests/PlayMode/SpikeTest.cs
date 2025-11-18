using NUnit.Framework;
using UnityEngine;
using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;

[TestFixture]
public class SpikeTest
{
    private GameObject trapObj;
    private Spike spike;
    private Animator animator;
    private BoxCollider2D collider;

    [SetUp]
    public void Setup()
    {
        trapObj = new GameObject("TestSpike");
        animator = trapObj.AddComponent<Animator>();
        collider = trapObj.AddComponent<BoxCollider2D>();
        spike = trapObj.AddComponent<Spike>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(trapObj);
    }

    [TestFixture]
    public class Inheritance : SpikeTest
    {
        [Test]
        public void ShouldInheritFromTrap()
        {
            Assert.IsTrue(spike is Trap);
            Assert.IsInstanceOf<Trap>(spike);
        }

        [Test]
        public void ShouldHaveAccessToTrapProperties()
        {
            Assert.IsNotNull(spike.trapType);
            Assert.AreEqual(TrapType.Spike, spike.trapType);
        }

        [Test]
        public void ShouldInheritFromTile()
        {
            Assert.IsTrue(spike is Tile);
            Assert.IsInstanceOf<Tile>(spike);
        }

        [Test]
        public void ShouldHaveAccessToTileProperties()
        {
            spike.SpriteData = new SpriteData();
            Assert.IsNotNull(spike.SpriteData);
        }
    }

    [TestFixture]
    public class PlayAnimationSequence : SpikeTest
    {
        [UnityTest]
        public IEnumerator ShouldEnableCollider_WhenSpikeIsOn()
        {
            spike.settings["startTime"] = 0f;
            spike.settings["onTime"] = 0.5f;
            spike.settings["offTime"] = 0.5f;

            spike.RestartTimer();

            yield return new WaitForSeconds(0.1f);
            Assert.IsTrue(collider.enabled);

            yield return new WaitForSeconds(0.5f);
            Assert.IsFalse(collider.enabled);
        }

        [UnityTest]
        public IEnumerator ShouldDisableCollider_WhenSpikeIsOff()
        {
            spike.settings["startTime"] = 0f;
            spike.settings["onTime"] = 0.5f;
            spike.settings["offTime"] = 0.5f;

            spike.RestartTimer();

            yield return new WaitForSeconds(0.6f);
            Assert.IsFalse(collider.enabled);

            yield return new WaitForSeconds(0.5f);
            Assert.IsTrue(collider.enabled);
        }

        [UnityTest]
        public IEnumerator ShouldRestartTimer_Correctly()
        {
            spike.settings["startTime"] = 0f;
            spike.settings["onTime"] = 1f;
            spike.settings["offTime"] = 1f;

            spike.RestartTimer();

            yield return new WaitForSeconds(0.5f);
            spike.RestartTimer();

            yield return new WaitForSeconds(0.6f);
            Assert.IsTrue(collider.enabled);

            yield return new WaitForSeconds(1f);
            Assert.IsFalse(collider.enabled);
        }
    }
}
