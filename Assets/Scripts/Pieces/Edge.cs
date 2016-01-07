using UnityEngine;

public class Edge : MonoBehaviour
{
    public Color ColorInEditor, ColorInGame;
    public Spot A { get; private set; }
    public Spot B { get; private set; }
    public Collider2D Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }

            return _collider;
        }
    }

    SpriteRenderer _renderer;
    Color _colorTo;
    Collider2D _collider;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        OnGameStateChanged(GameController.Instance.CurrentState);
    }

    void Update()
    {
        _renderer.color = Color.Lerp(_renderer.color, _colorTo, Time.deltaTime * 8f);
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

    public bool HasNode(Spot n)
    {
        return (n == A) || (n == B);
    }

    public void SetNodes(Spot a, Spot b)
    {
        A = a;
        B = b;

        SetVertices(A.transform.position, B.transform.position);
    }

    public void SetVertices(Vector2 posA, Vector2 posB)
    {
        Vector2 delta = posB - posA;
        transform.position = posA + (0.5f * delta);
        transform.localScale = new Vector3(transform.localScale.x, 0.9f * delta.magnitude, transform.localScale.z);
        float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void OnSpotsPositionChanged()
    {
        SetVertices(A.transform.position, B.transform.position);
    }

    public void OnGameStateChanged(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Edit:
                _colorTo = ColorInEditor;
                break;

            case GameStates.Place:
                _colorTo = ColorInEditor;
                break;

            case GameStates.Play:
                _colorTo = ColorInGame;
                break;
        }
    }
}