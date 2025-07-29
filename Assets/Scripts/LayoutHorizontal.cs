using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutHorizontal : MonoBehaviour
{
    [SerializeField] HorizontalLayoutData horizontalLayoutData;
    [SerializeField] Transform parentForCards;
    [SerializeField] Card cardPrefab;
    List<Card> insCards = new List<Card>();
    int heightInGrid = -1;
    
    public void SpawnCards(int height, HorizontalLayoutData horizontalLayoutData)
    {
        transform.name = height + " " + transform.name;
        List<CardDataSO> cardSOs = horizontalLayoutData.cardSOs;
        heightInGrid = height;
        for (int i = 0; i < cardSOs.Count; i++)
        {
            if (cardSOs[i] != null)
            {
                Card newCard = Instantiate(cardPrefab, parentForCards);
                newCard.transform.name = i.ToString();
                newCard.gameObject.SetActive(true);
                insCards.Add(newCard);
                newCard.InitData(cardSOs[i].myCardData);
            }
        }
    }

    public void InitUI()
    {
        foreach (var x in insCards)
        {
            x.InitUI();
        }
    }
}
