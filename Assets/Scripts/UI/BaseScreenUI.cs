using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScreenUI : MonoBehaviour
{
    public UIManager UIMan => UIManager.Instance;
  
    [SerializeField] ScreenName screenName;
    public ScreenName ScreenName => screenName;

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
