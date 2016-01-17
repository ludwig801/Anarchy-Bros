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
        Levels = 2,
        Options = 3
    }
    // Public vars
    [Range(0f, 8f)]
    public float AnimationsSpeed;
    [ReadOnly]
    public Menus CurrentMenu;
    public MenuPanel MainMenu, ErasMenu, LevelsMenu, OptionsMenu;
    // Properties
    public GameOptions GameOptions
    {
        get
        {
            if (_gameOptions == null)
            {
                _gameOptions = GetComponent<GameOptions>();
            }
            return _gameOptions;
        }
    }
    // Private vars
    ScrollerBehavior _scrollerEras, _scrollerLevels;
    GlobalManager _globalManager;
    GameEra _lastSeenEra;
    GameOptions _gameOptions;

    void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
    }

    void Start()
    {
        _globalManager = GlobalManager.Instance;

        _scrollerEras = ErasMenu.GetComponent<ScrollerBehavior>();
        for (int i = 0; i < _globalManager.Eras.Count; i++)
        {
            _scrollerEras.AddOption(_globalManager.Eras[i].Name.ToString(), _globalManager.Eras[i].Sprite, _globalManager.Eras[i].Unlocked);
        }

        _scrollerLevels = LevelsMenu.GetComponent<ScrollerBehavior>();

        ErasMenu.HideAbove(float.MaxValue);
        LevelsMenu.HideRight(float.MaxValue);
        MainMenu.HideBelow(float.MaxValue);
        MainMenu.Show(float.MaxValue);
        OptionsMenu.HideBelow(float.MaxValue);
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
                ErasMenu.Description.text = _globalManager.CurrentEra.Name.ToString();
                UpdateCurrentLevels();
                break;

            case Menus.Levels:
                LevelsMenu.Title.text = "Choose a level";
                LevelsMenu.Description.text = _globalManager.CurrentLevel.Title;
                break;
        }

        _globalManager.CurrentEra = _globalManager.Eras[_scrollerEras.CurrentOption];
        if (_globalManager.CurrentEra.Levels.Count > 0)
        {
            _globalManager.CurrentLevel = _globalManager.CurrentEra.Levels[_scrollerLevels.CurrentOption];
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

            case Menus.Options:
                OptionsMenu.HideBelow(AnimationsSpeed);
                MainMenu.Show(AnimationsSpeed);
                CurrentMenu = Menus.Main;
                break;
        }
    }

    public void OpenOptions()
    {
        MainMenu.HideAbove(AnimationsSpeed);
        OptionsMenu.Show(AnimationsSpeed);
        CurrentMenu = Menus.Options;
    }

    public void CloseOptions()
    {
        MainMenu.Show(AnimationsSpeed);
        OptionsMenu.HideBelow(AnimationsSpeed);
        CurrentMenu = Menus.Options;
    }

    public void UpdateCurrentLevels()
    {
        if (_globalManager.CurrentEra != null && _lastSeenEra != _globalManager.CurrentEra)
        {
            _lastSeenEra = _globalManager.CurrentEra;

            _scrollerLevels.RemoveOptions();
            List<GameLevel> CurrentLevels = _globalManager.CurrentEra.Levels;

            for (int i = 0; i < CurrentLevels.Count; i++)
            {
                _scrollerLevels.AddOption(CurrentLevels[i].Title, CurrentLevels[i].Sprite, CurrentLevels[i].Unlocked);
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        _globalManager.SetInGame(true);
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
