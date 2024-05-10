using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite halfSprite;
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private float HealthPerContainer = 20f;
    [SerializeField] private float spriteSize = 20f;

    private string containerContent = "2 2 2";
    private float highestValue;

    private List<GameObject> spritePool = new List<GameObject>();
    private Vector2 lastPosition;

    void Start()
    {
        lastPosition = new Vector2(spriteSize/ 2, -spriteSize/2);
    }

    public void UpdateValue(float health)
    {
        highestValue = (highestValue > health) ? highestValue : health;

        string newHeartCode = "";
        float healthCalculation = health;

        for (int i = 1; i*HealthPerContainer <= highestValue; i++)
        {
            if(MathF.Round(healthCalculation / HealthPerContainer) >= 1 )
            {
                newHeartCode += " 2";
            } else if (MathF.Round(healthCalculation / (HealthPerContainer/2)) >= 1)
            {
                newHeartCode += " 1";
            } else
            {
                if (i == 1 && health > 0) newHeartCode += "1";
                newHeartCode += " 0";
            }

            healthCalculation -= HealthPerContainer;
        }



        containerContent = newHeartCode;

        UpdateSprites();
    }

    void UpdateSprites()
    {
        string[] hearts = containerContent.Trim().Split(' ');

        for(int index = 0;  index < hearts.Length; index++)
        {
            bool hasEnoughSprite = false;
            while (!hasEnoughSprite)
            {
                try
                {
                    SetSpriteOnObject(spritePool[index], hearts[index]);
                    hasEnoughSprite = true;
                }
                catch
                {
                    GameObject newSprite = new GameObject(spritePool.Count.ToString());
                    newSprite.transform.parent = this.transform;
                    newSprite.AddComponent<Image>();
                    newSprite.transform.localScale = Vector3.one;
                    newSprite.GetComponent<RectTransform>().localPosition = lastPosition;
                    newSprite.GetComponent<RectTransform>().sizeDelta = Vector2.one * spriteSize;
                    lastPosition.x += spriteSize;
                    spritePool.Add(newSprite);
                }
            }
        }
    }

    void SetSpriteOnObject(GameObject Object, string HeartCode)
    {
        switch (HeartCode)
        {
            case "0":
                Object.GetComponent<Image>().sprite = emptySprite;
                break;
            case "1":
                Object.GetComponent<Image>().sprite = halfSprite;
                break;
            case "2":
                Object.GetComponent<Image>().sprite = fullSprite;
                break;
        }
    }
}
