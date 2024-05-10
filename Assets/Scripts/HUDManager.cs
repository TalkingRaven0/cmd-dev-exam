using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CoinDisplay;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject deathScreen;

    [SerializeField] GameObject helpScreen;
    [SerializeField] GameObject gameHUD;

    IEnumerator InitializeUI()
    {
        yield return new WaitUntil(()=>PlayerStats.instance != null);
        CoinDisplay.text = PlayerStats.instance.Coins.ToString();
        healthBar.UpdateValue(PlayerStats.instance.Health);

        SetupListeners();
    }

    void SetupListeners()
    {
        PlayerStats.instance.CoinsChanged.Subscribe(newCoins => OnChangeCoinValue(newCoins)).AddTo(this);
        PlayerStats.instance.HealthChanged.Subscribe(newHealth => healthBar.UpdateValue(newHealth)).AddTo(this);
        PlayerStats.instance.PlayerDied.Subscribe(isPlayerDead => deathScreen.SetActive(isPlayerDead));
    }

    void OnChangeCoinValue(float newValue)
    {
        CoinDisplay.text = newValue.ToString();
    }

    void Start()
    {
        StartCoroutine(InitializeUI());
    }

    public void OnHelp()
    {
        helpScreen.SetActive(!helpScreen.activeSelf);
        gameHUD.SetActive(!gameHUD.activeSelf);
    }

}
