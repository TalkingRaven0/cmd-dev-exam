using System;
using UniRx;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxhealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float coins = 10f;
    [SerializeField] private bool isDead = false;

    private readonly Subject<float> healthSubject = new Subject<float>();
    public IObservable<float> HealthChanged => healthSubject;


    private readonly Subject<float> coinsSubject = new Subject<float>();
    public IObservable<float> CoinsChanged => coinsSubject;

    private readonly Subject<bool> PlayerIsDeadSubject = new Subject<bool>();
    public IObservable<bool> PlayerDied => PlayerIsDeadSubject;

    public static PlayerStats instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;

        currentHealth = maxhealth;
    }

    public float Health
    {
        get => currentHealth;
        set
        {
            currentHealth = (value > maxhealth) ? maxhealth : value;
            healthSubject.OnNext(Health); // Notify observers of HP change

            if(currentHealth <= 0)
            {
                IsDead = true;
            }
        }
    }

    public float Coins
    {
        get => coins;
        set
        {
            coins = value;
            coinsSubject.OnNext(coins); // Notify observers of Coins change
        }
    }

    public bool IsDead
    {
        get => isDead;
        set 
        {
            isDead = value;
            PlayerIsDeadSubject.OnNext(isDead);
        }
    }

    public float MaxHealth
    {
        get => maxhealth;
    }


    public void Respawn()
    {
        IsDead = false;
        Health = maxhealth;
    }
}
