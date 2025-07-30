using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayoutData
{
    public List<HorizontalLayoutData> horizontalLayoutDatas = new List<HorizontalLayoutData>();
}

[System.Serializable]
public class HorizontalLayoutData
{
    public List<CardDataSO> cardSOs = new List<CardDataSO>();
}

[CreateAssetMenu(fileName = "LayoutSO", menuName = "ScriptableObjects/LayoutSO", order = 1)]
public class LayoutSO : ScriptableObject
{
    public string layoutID = "";
    /// <summary>
    /// list of card data SOs that will be used as refCardDataSOs during random grid layout generation
    /// </summary>
    public List<CardDataSO> libraryCardDataSOs = new List<CardDataSO>();
   
    [Header("grid layout data")]
    public LayoutData layoutData;

    [Header("scoring data")]
    [SerializeField] int scorePerCard = 5;
    public int ScorePerCard => scorePerCard;

    [Header("RandomGenerateLayout")]
    [SerializeField][Range(2,3)] int numOfCopiesInGrid = 2;
    public int NumOfCopiesInGrid => numOfCopiesInGrid;

    public const int maxWidth = 7;
    public const int maxHeight = 7;
    [SerializeField] [Range(2,maxWidth)] int width = 2;
    [SerializeField] [Range(2,maxHeight)] int height = 2;
    List<CardDataSO> refCardDataSOs = new List<CardDataSO>();
    [SerializeField] List<CardDataSO> gridCardDataSOs = new List<CardDataSO>();

    private void OnValidate()
    {
        if (layoutID.Length < 3)
        {
            GenerateLayoutID();
        }
    }

    public void GenerateLayoutID()
    {
        layoutID = Utils.GenerateRandomString(20);
    }

    void RandomizeLibraryCardDataSOs()
    {
        refCardDataSOs = new List<CardDataSO>();
        foreach (var x in libraryCardDataSOs)
        {
            if (x != null)
            {
                refCardDataSOs.Add(x);
            }
        }
        refCardDataSOs.Randomize();
    }

    public void RandomGenerateLayout()
    {
        int maxElementsInGrid = width * height;
        int maxElementsInLibrary = Mathf.FloorToInt((maxElementsInGrid/ numOfCopiesInGrid));
        RandomizeLibraryCardDataSOs();
        refCardDataSOs.RemoveRange(maxElementsInLibrary, (refCardDataSOs.Count - maxElementsInLibrary));

        layoutData = new LayoutData();
        gridCardDataSOs = new List<CardDataSO>();
        for (int i = 0; i < numOfCopiesInGrid; i++)
        {
            gridCardDataSOs.AddRange(refCardDataSOs);
        }

        while (gridCardDataSOs.Count < maxElementsInGrid)
        {
            gridCardDataSOs.Add(null);
        }

        gridCardDataSOs.Randomize();

        int index = 0;
        for (int i = 0; i < height; i++)
        {
            HorizontalLayoutData h = new HorizontalLayoutData();
            for (int j = 0; j < width; j++)
            {
                h.cardSOs.Add(gridCardDataSOs[index]);
                index++;
            }
            layoutData.horizontalLayoutDatas.Add(h);
        }
    }
}
