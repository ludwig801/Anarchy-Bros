using UnityEngine;
using System.Collections.Generic;

public class Era : MonoBehaviour
{
    public string Title;
    [TextArea(3, 10)]
    public string Description;
    public Sprite Sprite;
    public bool Unlocked;

    public List<Level> Levels;
}
