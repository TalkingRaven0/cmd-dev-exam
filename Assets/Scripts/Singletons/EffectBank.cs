using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBank : MonoBehaviour
{

    public static EffectBank instance;

    [SerializeField] public ParticleSystem BombOnInteract;
    [SerializeField] public ParticleSystem CoinOnInteract;
    [SerializeField] public ParticleSystem HealthOnInteract;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }
}
