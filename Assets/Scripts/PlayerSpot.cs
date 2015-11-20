using UnityEngine;

public class PlayerSpot : MonoBehaviour
{
    Player _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnMouseUp()
    {
        _player.MoveTo = transform;
    }
}
