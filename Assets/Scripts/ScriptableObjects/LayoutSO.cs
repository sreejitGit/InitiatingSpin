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
    /// <summary>
    /// list of card data SOs that will be used as refCardDataSOs during random grid layout generation
    /// </summary>
    public List<CardDataSO> libraryCardDataSOs = new List<CardDataSO>();
   
    [Header("grid layout data")]
    public LayoutData layoutData;

    [Header("RandomGenerateLayout")]
    [SerializeField][Range(1,4)] int numOfCopiesInGrid = 2;

    public const int maxWidth = 7;
    public const int maxHeight = 7;
    [SerializeField] [Range(2,maxWidth)] int width = 2;
    [SerializeField] [Range(2,maxHeight)] int height = 2;
    [SerializeField] List<CardDataSO> refCardDataSOs = new List<CardDataSO>();
    [SerializeField] List<CardDataSO> gridCardDataSOs = new List<CardDataSO>();


    public void RandomizeLibraryCardDataSOs()
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
        float maxElementsInLibrary = (float)(width * height) / numOfCopiesInGrid ;
        if (refCardDataSOs.Count < maxElementsInLibrary)
        {
            RandomizeLibraryCardDataSOs();
        }

        int maxPossibility = refCardDataSOs.Count * numOfCopiesInGrid;
        int limit = maxWidth * maxHeight;
        if (maxPossibility > limit)
        {
            maxPossibility = limit;
        }
        width = height = Mathf.FloorToInt(Mathf.Sqrt(maxPossibility));


        maxElementsInLibrary = (float)(width * height) / numOfCopiesInGrid ;
        bool addOneExtraElement = false;
        if ((float)(width * height) % 2 == 0)
        {
            addOneExtraElement = false;
        }
        else
        {
            maxElementsInLibrary = Mathf.FloorToInt(maxElementsInLibrary);
            addOneExtraElement = true;
        }

        refCardDataSOs.RemoveRange((int)maxElementsInLibrary, (int)(refCardDataSOs.Count - maxElementsInLibrary));

        layoutData = new LayoutData();
        gridCardDataSOs = new List<CardDataSO>();
        for (int i = 0; i < numOfCopiesInGrid; i++)
        {
            gridCardDataSOs.AddRange(refCardDataSOs);
        }

        if (addOneExtraElement)
        {
            gridCardDataSOs.Add(gridCardDataSOs[0]);
            gridCardDataSOs.Randomize();
        }
        else
        {
            gridCardDataSOs.Randomize();
        }
     
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
