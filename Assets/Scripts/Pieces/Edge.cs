using UnityEngine;

public class Edge : MonoBehaviour
{
    public Color ColorInEditor, ColorInGame;
    public Spot A, B;
    public float Thickness;

    SpriteRenderer _renderer;
    GameManager _gameController;

    void Start()
    {
        _gameController = GameManager.Instance;

        _renderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(transform.localScale.x * Thickness, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        switch (_gameController.CurrentState)
        {
            case GameStates.Play:
                _renderer.color = Color.Lerp(_renderer.color, ColorInGame, Time.deltaTime * 8f);
                break;

            default:
                _renderer.color = Color.Lerp(_renderer.color, ColorInEditor, Time.deltaTime * 8f);
                break;
        }

        if (A != null && B != null)
        {
            Vector2 posA = A.transform.position;
            Vector2 posB = B.transform.position;
            Vector2 delta = (posA - posB);
            transform.position = posB + (0.5f * delta);
            transform.localScale = new Vector3(transform.localScale.x, delta.magnitude, transform.localScale.z);
            transform.rotation = Tools2D.LookAt(delta.normalized);
        }
    }

    public Spot Neighbor(Spot n)
    {
        return (n == A) ? B : (n == B) ? A : null;
    }

    public void ReplaceNeighbor(Spot oldNeighbor, Spot newNeighbor)
    {
        if (A == oldNeighbor)
        {
            A = newNeighbor;
        }
        else if (B == oldNeighbor)
        {
            B = newNeighbor;
        }
    }

    public bool HasSpot(Spot n)
    {
        return (n == A) || (n == B);
    }
}