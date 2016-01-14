using UnityEngine;

public class Level : MonoBehaviour
{
    public int Order;
    public string Title;
    [TextArea(3, 10)]
    public string Description;
    public Sprite Sprite, Background;
    public bool Unlocked;

    public bool Playable
    {
        get
        {
            return Title.Length > 0 && Description.Length > 0 && Sprite != null;
        }
    }
}

