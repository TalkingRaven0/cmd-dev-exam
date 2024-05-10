using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody2D))]

public class PlayerVisuals : MonoBehaviour
{

    #region Component References
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    #endregion

    #region Animator Variables
    private const string IS_DEAD_BOOL = "isDead";
    private const string IS_GROUNDED_BOOL = "isGrounded";
    private const string MOVEMENT_FLOAT = "Movement";
    #endregion

    private float currentFacing = 1;
    [SerializeField] private bool DisableTurning = false;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float knownHP = 0;
    private bool isDead = false;
    private Coroutine damagedEffectRoutine;

    IEnumerator InitializeStateListener()
    {
        yield return new WaitUntil(() => PlayerStats.instance != null);
        PlayerStats.instance.HealthChanged.Subscribe(newHealth => HealthChangeReaction(newHealth));
        PlayerStats.instance.PlayerDied.Subscribe(isPlayerDead => PlayerLife(isPlayerDead));
    }


    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(InitializeStateListener());
    }

    // Update is called once per frame
    void Update()
    {
        ManageLookDirection();

        // Manage Movement State
        animator.SetFloat(MOVEMENT_FLOAT,Mathf.Abs(rigidBody2D.velocity.x));

        // Manage Grounded State
        animator.SetBool(IS_GROUNDED_BOOL, (Mathf.Abs(rigidBody2D.velocity.y) > 0.3f) ? false : true);
    }

    void ManageLookDirection()
    {
        if (isDead || DisableTurning) return;
        if (currentFacing*-1 != transform.localScale.x) transform.localScale = new Vector2(currentFacing*-1, transform.localScale.y);
    }

    #region Input Handlers

    public void OnMove(InputValue value)
    {

        // Checking if value is Null
        Vector2 assigner;
        assigner = (value == null) ? Vector2.zero : value.Get<Vector2>();
        if (assigner.x == 0) return;

        currentFacing = Mathf.Sign(assigner.x);
    }

    public void OnRetry()
    {
        if (!isDead) return;
        animator.SetBool(IS_DEAD_BOOL, false);
    }

    #endregion

    public float getCurrentFacing()
    {
        return currentFacing;
    }

    private void HealthChangeReaction(float newHealth)
    {
        if (newHealth >= knownHP)
        {
            knownHP = newHealth;
            return;
        }

        knownHP = newHealth;

        if(damagedEffectRoutine != null) StopCoroutine(damagedEffectRoutine);
        damagedEffectRoutine = StartCoroutine(damagedEffect());
    }

    IEnumerator damagedEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void PlayerLife(bool PlayerIsDead)
    {
        if (PlayerIsDead)
        {
            StopCoroutine(damagedEffectRoutine);
            animator.SetBool(IS_DEAD_BOOL, true);
        }
        isDead = PlayerIsDead;
    }
}
