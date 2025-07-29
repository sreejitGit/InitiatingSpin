using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutSpawner : MonoBehaviour
{
    [SerializeField] Transform verticalParentTransform;
    [SerializeField] LayoutHorizontal layoutHorizontalPrefab;
    List<LayoutHorizontal> insLayoutHorizontals = new List<LayoutHorizontal>();
    [SerializeField]  LayoutSO tempLayoutSO;

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        StartCoroutine(Spawn(tempLayoutSO));
    }

    public IEnumerator Spawn(LayoutSO layoutSO)
    {
        ResetLayout();
        for (int j = 0; j < layoutSO.layoutData.horizontalLayoutDatas.Count; j++)
        {
            LayoutHorizontal newHori = Instantiate(layoutHorizontalPrefab, verticalParentTransform);
            newHori.gameObject.SetActive(true);
            insLayoutHorizontals.Add(newHori);
            newHori.SpawnCards(j, layoutSO.layoutData.horizontalLayoutDatas[j]);
        }
        yield return new WaitForEndOfFrame();
        foreach (var x in insLayoutHorizontals)
        {
            x.InitUI();
        }
    }

    void ResetLayout()
    {
        foreach (var x in insLayoutHorizontals)
        {
            if (x != null)
            {
                Destroy(x.gameObject);
            }
        }
        insLayoutHorizontals = new List<LayoutHorizontal>();
    }
}
