using UnityEngine;

public class MapEdge : MonoBehaviour
{
    public Color ColorInEditor, ColorInGame, ColorInGamePaused;
    public MapSpot A, B;
    public float Thickness;
    public int RenderUpdatesPerSecond;

    SpriteRenderer _renderer;
    GameManager _gameManager;
    float _renderUpdatesInterval, _timeSinceLastRenderUpdate;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _renderer = GetComponent<SpriteRenderer>();

        _renderUpdatesInterval = 1f / RenderUpdatesPerSecond;

        _timeSinceLastRenderUpdate = 0;

        transform.localScale = new Vector3(transform.localScale.x * Thickness, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        _timeSinceLastRenderUpdate += Time.unscaledDeltaTime;

        if (_timeSinceLastRenderUpdate > _renderUpdatesInterval)
        {
            _timeSinceLastRenderUpdate = 0;

            switch (_gameManager.CurrentState)
            {
                case GameStates.Play:
                    _renderer.color = Color.Lerp(_renderer.color, ColorInGame, Time.deltaTime * 8f);
                    break;

                case GameStates.Edit:
                    _renderer.color = Color.Lerp(_renderer.color, ColorInEditor, Time.deltaTime * 8f);
                    break;

                default:
                    _renderer.color = Color.Lerp(_renderer.color, ColorInGamePaused, Time.unscaledDeltaTime * 8f);
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
    }

    public MapSpot Neighbor(MapSpot n)
    {
        return (n == A) ? B : (n == B) ? A : null;
    }

    public void ReplaceNeighbor(MapSpot oldNeighbor, MapSpot newNeighbor)
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

    public bool HasSpot(MapSpot n)
    {
        return (n == A) || (n == B);
    }
}