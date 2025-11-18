using NUnit.Framework;
using UnityEngine;
using InGame;
using UnityEngine.InputSystem;
using Objects;
[TestFixture]
public class PlayerTest
{
    private GameObject playerObj;
    private Player player;
    private Rigidbody2D rb;
    private GameObject groundCheck;
    private GameObject gameManagerObj;
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        playerObj = new GameObject("Player");
        rb = playerObj.AddComponent<Rigidbody2D>();
        player = playerObj.AddComponent<Player>();
        player.rb = rb;

        groundCheck = new GameObject("GroundCheck");
        player.groundCheck = groundCheck.transform;

        gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        player.gameManager = gameManager;

        player.animator = playerObj.AddComponent<Animator>();
        player.boxCollider = playerObj.AddComponent<BoxCollider2D>();
        player.capsuleCollider = playerObj.AddComponent<CapsuleCollider2D>();

         var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = new InputActionMap("Player");
        var jumpAction = map.AddAction("Jump", InputActionType.Button);
        asset.AddActionMap(map);
        player.jump = InputActionReference.Create(jumpAction);
        jumpAction.Enable();
    }

[TearDown]
public void Teardown()
{
    Object.DestroyImmediate(playerObj);
    Object.DestroyImmediate(groundCheck);
    Object.DestroyImmediate(gameManagerObj);
    if (player.jump != null)
        Object.DestroyImmediate(player.jump.asset);
}

    [Test]
    public void ShouldInitializeRequiredComponents()
    {
        Assert.NotNull(player.rb);
        Assert.NotNull(player.animator);
        Assert.NotNull(player.boxCollider);
        Assert.NotNull(player.capsuleCollider);
        Assert.NotNull(player.gameManager);
    }

    [Test]
    public void ShouldMoveHorizontally_WhenNotDead()
    {
        player.isDead = false;
        player.movement = new Vector2(1, 0);
        player.moveSpeed = 5f;

        player.FixedUpdate();

        Assert.AreEqual(5f, player.rb.velocity.x, 0.01f);
    }

    [Test]
    public void ShouldNotMove_WhenDead()
    {
        player.isDead = true;
        player.movement = new Vector2(1, 0);
        player.moveSpeed = 5f;

        player.FixedUpdate();

        Assert.AreEqual(0f, player.rb.velocity.x, 0.01f);
    }

[Test]
public void ShouldPerformJump_WhenGrounded()
{
    player.isGrounded = true;
    player.wasGrounded = true;
    player.isJumping = false;
    player.jumpBufferCounter = 0.1f;

    player.HandleJump();

    Assert.IsTrue(player.isJumping);
    Assert.AreEqual(player.jumpForce, player.rb.velocity.y, 0.01f);
}

    [Test]
    public void ShouldNotJump_WhenNotGrounded()
    {
        player.isGrounded = false;
        player.jumpBufferCounter = 0.1f;
        player.isJumping = false;

        player.HandleJump();

        Assert.IsFalse(player.isJumping);
    }

    [Test]
    public void ShouldApplyFallMultiplier_WhenFalling()
    {
        player.rb.velocity = new Vector2(0, -5);
        player.fallMultiplier = 4f;

        float originalYVel = player.rb.velocity.y;
        player.ApplyJumpPhysics();

        Assert.Less(player.rb.velocity.y, originalYVel);
    }

    [Test]
    public void ShouldNotBeGrounded_WhenNoBlockDetected()
    {
        player.CheckGrounded();
        Assert.IsFalse(player.isGrounded);
    }

    [Test]
    public void ShouldSwitchColliders_WhenCrouching()
    {
        player.isCrouching = true;
        player.HandleCrouchCollider();

        Assert.IsFalse(player.boxCollider.enabled);
        Assert.IsTrue(player.capsuleCollider.enabled);
    }

    [Test]
    public void ShouldSwitchColliders_WhenNotCrouching()
    {
        player.isCrouching = false;
        player.HandleCrouchCollider();

        Assert.IsTrue(player.boxCollider.enabled);
        Assert.IsFalse(player.capsuleCollider.enabled);
    }

    [Test]
    public void ShouldFlipCharacter_WhenMovingRight()
    {
        player.movement = new Vector2(1, 0);
        player.FlipCharacter();

        Assert.Greater(player.transform.localScale.x, 0);
    }

    [Test]
    public void ShouldFlipCharacter_WhenMovingLeft()
    {
        player.movement = new Vector2(-1, 0);
        player.FlipCharacter();

        Assert.Less(player.transform.localScale.x, 0);
    }
}
