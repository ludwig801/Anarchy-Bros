using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuUIManager : MonoBehaviour
{
    // Static vars
    public static MenuUIManager Instance;
    // Enums
    public enum Menus
    {
        Main = 0,
        Eras = 1,
        Levels = 2
    }
    // Public vars
    [Range(0f, 8f)]
    public float AnimationsSpeed;
    [ReadOnly]
    public Menus CurrentMenu;
    public MenuPanel MainMenu, ErasMenu, LevelsMenu;
    // Private vars
    ScrollerBehavior _scrollerEras, _scrollerLevels;
    LevelManager _levelManager;
    GameEra _lastSeenEra;

    void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }

    void Start()
    {
        _levelManager = LevelManager.Instance;

        _scrollerEras = ErasMenu.GetComponent<ScrollerBehavior>();
        for (int i = 0; i < _levelManager.Eras.Count; i++)
        {
            _scrollerEras.AddOption(_levelManager.Eras[i].Name.ToString(), _levelManager.Eras[i].Sprite, _levelManager.Eras[i].Unlocked);
        }

        _scrollerLevels = LevelsMenu.GetComponent<ScrollerBehavior>();

        ErasMenu.HideAbove(float.MaxValue);
        LevelsMenu.HideRight(float.MaxValue);
        MainMenu.HideBelow(float.MaxValue);
        MainMenu.Show(float.MaxValue);
        CurrentMenu = Menus.Main;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PreviousMenu();
        }

        switch (CurrentMenu)
        {
            case Menus.Eras:
                ErasMenu.Title.text = "Choose an Era";
                ErasMenu.Description.text = _levelManager.CurrentEra.Name.ToString();
                UpdateCurrentLevels();
                break;

            case Menus.Levels:
                LevelsMenu.Title.text = "Choose a level";
                LevelsMenu.Description.text = _levelManager.CurrentLevel.Title;
                break;
        }

        _levelManager.CurrentEra = _levelManager.Eras[_scrollerEras.CurrentOption];
        if (_levelManager.CurrentEra.Levels.Count > 0)
        {
            _levelManager.CurrentLevel = _levelManager.CurrentEra.Levels[_scrollerLevels.CurrentOption];
        }
    }

    public void NextMenu()
    {
        switch (CurrentMenu)
        {
            case Menus.Main:
                MainMenu.HideBelow(AnimationsSpeed);
                ErasMenu.Show(AnimationsSpeed);
                CurrentMenu = Menus.Eras;
                break;

            case Menus.Eras:
                ErasMenu.HideLeft(AnimationsSpeed);
                LevelsMenu.Show(AnimationsSpeed);  
                CurrentMenu = Menus.Levels;
                break;

            case Menus.Levels:
                CurrentMenu = Menus.Main;
                LoadScene("level");
                break;
        }
    }

    public void PreviousMenu()
    {
        switch (CurrentMenu)
        {
            case Menus.Main:
                Quit();
                break;

            case Menus.Eras:
                ErasMenu.HideAbove(AnimationsSpeed);
                MainMenu.Show(AnimationsSpeed);
                CurrentMenu = Menus.Main;
                break;

            case Menus.Levels:
                LevelsMenu.HideRight(AnimationsSpeed);
                ErasMenu.Show(AnimationsSpeed);
                CurrentMenu = Menus.Eras;
                break;
        }
    }

    public void UpdateCurrentLevels()
    {
        if (_levelManager.CurrentEra != null && _lastSeenEra != _levelManager.CurrentEra)
        {
            _lastSeenEra = _levelManager.CurrentEra;

            _scrollerLevels.RemoveOptions();
            List<GameLevel> CurrentLevels = _levelManager.CurrentEra.Levels;

            for (int i = 0; i < CurrentLevels.Count; i++)
            {
                _scrollerLevels.AddOption(CurrentLevels[i].Title, CurrentLevels[i].Sprite, CurrentLevels[i].Unlocked);
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
