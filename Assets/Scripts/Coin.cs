using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : PooledObject, IInteractable
{
    [SerializeField] private float coinValue = 1f;
    public void OnInteract()
    {
        Debug.Log("Coin Picked Up!");

        if(EffectBank.instance.CoinOnInteract != null)
        {
            EffectBank.instance.CoinOnInteract.transform.position = transform.position;
            EffectBank.instance.CoinOnInteract.Play();
        }

        PlayerStats.instance.Coins += coinValue;
        PoolDestroy();
    }
}
