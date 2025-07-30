using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutSpawner : MonoBehaviour
{
    [SerializeField] Transform verticalParentTransform;
    [SerializeField] LayoutHorizontal layoutHorizontalPrefab;


    public List<LayoutHorizontal> InsLayoutHorizontals =>insLayoutHorizontals;
    List<LayoutHorizontal> insLayoutHorizontals = new List<LayoutHorizontal>();


    public void SpawnLayout(LayoutSO layoutSO)
    {
        ResetLayout();
        StartCoroutine(Spawn(layoutSO));
    }

    IEnumerator Spawn(LayoutSO layoutSO)
    {
        for (int j = 0; j < layoutSO.layoutData.horizontalLayoutDatas.Count; j++)
        {
            LayoutHorizontal newHori = Instantiate(layoutHorizontalPrefab, verticalParentTransform);
            newHori.gameObject.SetActive(true);
            insLayoutHorizontals.Add(newHori);
            newHori.SpawnCards(j, layoutSO.layoutData.horizontalLayoutDatas[j]);
        }
        yield return new WaitForEndOfFrame();
        SFXManager.instance.PlaySFXOnce(SFXManager.GameplaySFXType.LayoutOpen);
        foreach (var x in insLayoutHorizontals)
        {
            yield return x.InitUI();
        }
        yield return new WaitForSeconds(0.125f);
        GameEvents.LayoutSetupDone();
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
        StopAllCoroutines();
    }
}
