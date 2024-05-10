using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : PooledObject, IInteractable
{
    [SerializeField] private float damageValue = 15f;
    public void OnInteract()
    {
        Debug.Log("Bomb Activated!!!");

        if (EffectBank.instance.BombOnInteract != null)
        {
            EffectBank.instance.BombOnInteract.transform.position = transform.position;
            EffectBank.instance.BombOnInteract.Play();
        }

        PlayerStats.instance.Health -= damageValue;
        PoolDestroy();
    }
}
