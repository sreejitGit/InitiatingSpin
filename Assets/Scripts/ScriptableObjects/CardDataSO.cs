using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public string name;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "CardDataSO", menuName = "ScriptableObjects/CardDataSO", order = 1)]
public class CardDataSO : ScriptableObject
{
    public CardData myCardData;

    private void OnValidate()
    {
        if (myCardData.sprite)
        {
            myCardData.name = myCardData.sprite.name;
        }
    }
}
