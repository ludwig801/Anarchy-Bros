using UnityEngine;
using System.Collections.Generic;

public class GameEra : MonoBehaviour
{
    public Eras Name;
    [TextArea(3, 10)]
    public string Description;
    public Sprite Sprite;
    public bool Unlocked;

    public List<GameLevel> Levels;
}
