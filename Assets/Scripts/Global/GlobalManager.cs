using UnityEngine;
using System.Collections.Generic;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;

    public Transform ErasObj, LevelsObj;
    public GameEra CurrentEra;
    public GameLevel CurrentLevel;
    public List<GameEra> Eras;
    public bool Debugging, InGame;
    public int SoundEffectsValue, MusicValue, BodyCountValue, Difficulty;

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance.gameObject != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Destroy(Instance.gameObject);
            }
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadErasAndLevels();

        CurrentEra = Eras[0];
        CurrentLevel = CurrentEra.Levels[0];
    }

    void LoadErasAndLevels()
    {
        for (int i = 0; i < ErasObj.childCount; i++)
        {
            GameEra era = ErasObj.GetChild(i).GetComponent<GameEra>();
            if (era != null)
            {
                Eras.Add(era);
            }
        }

        for (int i = 0; i < Eras.Count; i++)
        {
            GameEra era = Eras[i];
            for (int j = 0; j < LevelsObj.childCount; j++)
            {
                GameLevel level = LevelsObj.GetChild(j).GetComponent<GameLevel>();
                if (level != null && level.Era == era.Name)
                {
                    era.Levels.Add(level);
                }
            }
        }
    }

    internal void SetInGame(bool value)
    {
        InGame = value;
        if (value)
        {
            SoundEffectsValue = (int)MenuUIManager.Instance.GameOptions.SoundEffectsSlider.value;
            MusicValue = (int)MenuUIManager.Instance.GameOptions.MusicSlider.value;
            BodyCountValue = MenuUIManager.Instance.GameOptions.BodyCount.Value;
            Difficulty = MenuUIManager.Instance.GameOptions.Difficulty.Value;
        }
    }
}
