using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Medpack : PooledObject, IInteractable
{

    [SerializeField] private float healValue = 10f;
    public void OnInteract()
    {
        Debug.Log("You have Been Healed");

        if (EffectBank.instance.HealthOnInteract != null)
        {
            EffectBank.instance.HealthOnInteract.transform.position = transform.position;
            EffectBank.instance.HealthOnInteract.Play();
        }

        PlayerStats.instance.Health += healValue;
        PoolDestroy();
    }
}
