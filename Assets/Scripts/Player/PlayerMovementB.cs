using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovementB : MonoBehaviour
{
    #region Default Movement Variables
    [Header("Movement Variables")]
    [SerializeField] private float playerSpeed = 9;
    [SerializeField] private float accelerationValue = 9;
    [SerializeField] private float deccelerationValue = 9;
    [SerializeField] private float accelMulti = 1.2f;
    [SerializeField] private float airAccelMult = 0.2f;
    private Vector2 inputDir = Vector2.zero;
    #endregion

    #region Applied Movement Variables
    private float AplayerSpeed;
    private float AaccelerationValue;
    private float AdeccelerationValue;
    private float AaccelMulti;
    private float AairAccelMult;
    #endregion

    #region Attack Overrides
    public float AttackingPlayerSpeed {get; set;}
    public float AttackingAccelerationValue {get; set;}
    public float AttackingDeccelerationValue {get; set;}
    public float AttackingAccelMulti {get; set;}
    public float AttackingAirAccelMult {get; set;}
    public bool isAttacking { get; set; }
    #endregion

    #region Interface Fluff
    public float defaultPlayerSpeed { get; set; }
    public float defaultAccelerationValue { get; set; }
    public float defaultDeccelerationValue { get; set; }
    public float defaultAccelMulti { get; set; }
    public float defaultAirAccelMult { get; set; }
    #endregion

    [Space]

    #region Jump Variables
    [Header("Jump Variables")]
    [SerializeField] private float jumpStrength = 10;
    [SerializeField] private float jumpCutMultiplier = 0.4f;
    [SerializeField] private float cayoteTime = 0.2f;
    [SerializeField] private float gravityMultiplier = 2.1f;
    #endregion

    #region Miscellaneous
    [Header("Miscellaneous")]
    [SerializeField] private LayerMask TerrainCheck;
    #endregion

    #region Component References
    private Rigidbody2D rigidBody2D { get; set; }
    private BoxCollider2D boxCollider;
    #endregion

    #region State Variables
    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isDead = false;
    private float lastGroundedTime = 0;
    [SerializeField] private bool AnimateDisableMovement = false;
    #endregion

    float originalGravity = 1;

    IEnumerator InitializeStateListener()
    {
        yield return new WaitUntil(() => PlayerStats.instance != null);
        PlayerStats.instance.PlayerDied.Subscribe(isPlayerDead => PlayerLife(isPlayerDead)).AddTo(this);
    }

    void Start()
    {
        // Initialize References
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalGravity = rigidBody2D.gravityScale;

        defaultPlayerSpeed = playerSpeed;
        defaultAccelerationValue = accelerationValue;
        defaultDeccelerationValue = deccelerationValue;
        defaultAccelMulti = accelMulti;
        defaultAirAccelMult = airAccelMult;

        StartCoroutine(InitializeStateListener());
    }

    // Physics Update
    void FixedUpdate()
    {
        if (isDead || AnimateDisableMovement) return;

        MovementOverrides();

        isGrounded = GroundCheck();

        SmoothMovement((int)Mathf.Round(inputDir.x),AaccelerationValue, AdeccelerationValue, AplayerSpeed, AaccelMulti);

        JumpCut(jumpCutMultiplier);

        UpdateLastGroundedTime();
    }

    void UpdateLastGroundedTime()
    {
        // Cayote Time Tracker
        if (isJumping) lastGroundedTime = cayoteTime + 1;
        else lastGroundedTime = isGrounded ? 0 : lastGroundedTime + Time.deltaTime;
    }
    public void MovementOverrides()
    {
        // Set Defaults
        AplayerSpeed = playerSpeed;
        AaccelerationValue = accelerationValue;
        AdeccelerationValue = deccelerationValue;
        AaccelMulti = accelMulti;
        AairAccelMult = airAccelMult;

        // Air Overrides
        if (!isGrounded)
        {
            AaccelerationValue *= AairAccelMult;
            AdeccelerationValue *= AairAccelMult;
        }

    }

    #region Input Handlers

    public void OnMove(InputValue value)
    {
        if (isDead || AnimateDisableMovement) return;

        // Checking if value is Null
        Vector2 assigner;
        assigner = (value == null) ? Vector2.zero : value.Get<Vector2>();

        inputDir = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isDead || AnimateDisableMovement) return;

        bool cayoteValidation = lastGroundedTime < cayoteTime;

        // Evaluate Input as Boolean
        isJumping = value.Get() != null ? true : false;

        GravityChange(isJumping);

        if (!cayoteValidation) return;

        DoJump(isJumping);
    }

    #endregion

    #region Movement Functions

    private void SmoothMovement(int moveDirection,float acceleration, float decceleration, float speed, float forceMultiplier)
    {
        float targetSpeed = Mathf.Round(moveDirection) * speed;
        float speedDifference = targetSpeed - rigidBody2D.velocity.x;
        float speedChangeRate = (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : decceleration);
        float finalForce = Mathf.Pow(Mathf.Abs(speedDifference) * speedChangeRate, forceMultiplier) * Mathf.Sign(speedDifference);
        rigidBody2D.AddForce(finalForce * Vector2.right);
    }


    private void GravityChange(bool isLowGravity)
    {
        if (isJumping)
        {
            rigidBody2D.gravityScale = originalGravity;
        }
        else
        {
            // Set Original Gravity
            // Capture changes made at runtime
            originalGravity = rigidBody2D.gravityScale;
            rigidBody2D.gravityScale *= gravityMultiplier;
        }
    }

    private void DoJump(bool isJumping)
    {
        if (!isJumping) return; 
        rigidBody2D.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
        // Remove Cayotetime
        lastGroundedTime = cayoteTime + 1;
    }

    private void JumpCut(float jumpCutMultiplier)
    {
        if (isJumping && rigidBody2D.velocity.y > 0)
        {
            rigidBody2D.AddForce(Vector2.up * rigidBody2D.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Force);
        }
    }
    #endregion

    private bool GroundCheck() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (boxCollider.size.y / 2) + 0.1f, TerrainCheck);
        return hit.collider != null;
    }

    private void PlayerLife(bool isDeadSignal)
    {
        if (isDeadSignal)
        {
            isDead = true;
            rigidBody2D.velocity = Vector2.zero;
        } else
        {
            isDead = false;
        }
    }
}
