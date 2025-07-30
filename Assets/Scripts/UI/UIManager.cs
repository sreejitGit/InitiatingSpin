using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ScreenName
{
    None,
    MainMenu,
    Gameplay
}
    
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] public UnityEvent<BaseScreenUI, BaseScreenUI> OnChangeScreen;

    BaseScreenUI currentScreen;
    public BaseScreenUI CurrentScreen => currentScreen;
    [SerializeField] BaseScreenUI initScreen;
    [SerializeField] List<BaseScreenUI> screens = new List<BaseScreenUI>();
    public List<BaseScreenUI> Screens => screens;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeScreen(ScreenName screenName)
    {
        BaseScreenUI previousScreen = null;
        if (currentScreen != null)
        {
            previousScreen = currentScreen;
            currentScreen.Hide();
        }

        currentScreen = GetScreen(screenName);
        if (currentScreen != null)
        {
            currentScreen.Show();
        }

        if (OnChangeScreen != null && currentScreen != null)
        {
            OnChangeScreen.Invoke(previousScreen, currentScreen);
        }
    }

    public BaseScreenUI GetScreen(ScreenName screenName)
    {
        return screens.Find(x => x.ScreenName == screenName);
    }
}
