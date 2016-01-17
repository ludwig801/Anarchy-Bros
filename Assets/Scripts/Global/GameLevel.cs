using UnityEngine;

public class GameLevel : MonoBehaviour
{
    public Eras Era;
    public int Order;
    public string Title;
    [TextArea(3, 10)]
    public string Description;
    public Sprite Sprite, BackgroundSprite;
    public bool Unlocked;

    public bool Playable
    {
        get
        {
            return Title.Length > 0 && Description.Length > 0 && Sprite != null;
        }
    }
}

