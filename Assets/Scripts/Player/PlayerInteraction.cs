using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerVisuals))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask interactibleMask;
    [SerializeField] private float interactibleRange;

    private bool isDead = false;

    private PlayerVisuals PlayerVisuals;

    IEnumerator InitializeStateListener()
    {
        yield return new WaitUntil(() => PlayerStats.instance != null);
        PlayerStats.instance.PlayerDied.Subscribe(isPlayerDead => isDead = isPlayerDead);
    }

    private void Start()
    {
        PlayerVisuals = GetComponent<PlayerVisuals>();
        StartCoroutine(InitializeStateListener());
    }



    public void OnInteract()
    {
        if (isDead) return;
        float lookDirection = PlayerVisuals.getCurrentFacing();
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.right * lookDirection, interactibleRange, interactibleMask);
        if (!hit) return;
        hit.collider.GetComponent<IInteractable>().OnInteract();

    }
}
