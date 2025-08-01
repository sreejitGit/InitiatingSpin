using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutHorizontal : MonoBehaviour
{
    [SerializeField] HorizontalLayoutData horizontalLayoutData;
    [SerializeField] Transform parentForCards;
    [SerializeField] Card cardPrefab;
    public List<Card> InsCards => insCards;
    List<Card> insCards = new List<Card>();
    List<Card> emptyCards = new List<Card>();
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
            else
            {
                Card newCard = Instantiate(cardPrefab, parentForCards);
                newCard.transform.name = "empty";
                newCard.gameObject.SetActive(true);
                newCard.SetAsEmpty();
                emptyCards.Add(newCard);
            }
        }
    }

    public IEnumerator InitUI()
    {
        foreach (var x in insCards)
        {
            x.InitUI();
            yield return new WaitForSeconds(0.025f);
        }
        foreach (var x in emptyCards)
        {
            x.InitUI();
        }
    }
}
