using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public Transform ErasObj;
    public Era CurrentEra;
    public Level CurrentLevel;
    public List<Era> Eras;

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

        for (int i = 0; i < ErasObj.childCount; i++)
        {
            Era era = ErasObj.GetChild(i).GetComponent<Era>();
            if (era != null)
            {
                Eras.Add(era);
            }
        }

        CurrentEra = Eras[0];
        CurrentLevel = CurrentEra.Levels[0];
    }
}
