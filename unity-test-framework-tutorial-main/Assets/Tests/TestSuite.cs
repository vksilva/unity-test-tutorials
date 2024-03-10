using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Tests
{
    public class TestSuite : InputTestFixture
    {
        private readonly Player playerPrefab = Resources.Load<Player>("Player");
        private Skeleton skeletonPrefab = Resources.Load<Skeleton>("ArmedSkeleton");
        private Mouse mouse;
        private Keyboard keyboard;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            SceneManager.LoadScene("Scenes/Sandbox");
            mouse = InputSystem.AddDevice<Mouse>();
            keyboard = InputSystem.AddDevice<Keyboard>();

            playerPrefab.activeCam = false;
        }

        [TearDown]
        public override void TearDown()
        {
            // Bug: Workaround Unity's ArgumentOutOfRangeException when tearing playmode tests down. 
            var inputs = Object.FindObjectsOfType<PlayerInput>();
            foreach (var input in inputs)
            {
                Object.DestroyImmediate(input);
            }
            base.TearDown();
        }

        [Test]
        public void TestPlayerDamage()
        {
            var playerPos = Vector3.zero;
            var playerDir = Quaternion.identity;
            var player = Object.Instantiate(playerPrefab, playerPos, playerDir);

            Assert.That(player.health, Is.EqualTo(100f));

            player.applyDamage(20f);

            Assert.That(player.health, Is.EqualTo(80f));
            Assert.AreEqual(player.health, 80f);
        }

        [UnityTest]
        public IEnumerator TestSlashDamagesSkeleton()
        {
            var playerPos = new Vector3(2f, 1f, -1f);
            var playerDir = Quaternion.identity;
            var skeletonPos = new Vector3(2f, 0f, 1f);
            var skeletonDir = Quaternion.identity;

            var player = Object.Instantiate(playerPrefab, playerPos, playerDir);
            var skeleton = Object.Instantiate(skeletonPrefab, skeletonPos, skeletonDir);

            skeleton.enabled = false;
            skeleton.player = player;

            Assert.AreEqual(skeleton.health, 100f);

            Press(mouse.leftButton);
            yield return new WaitForSeconds(0.1f);
            Release(mouse.leftButton);
            yield return new WaitForSeconds(3f);

            Assert.AreEqual(skeleton.health, 80f);
        }
    }
}