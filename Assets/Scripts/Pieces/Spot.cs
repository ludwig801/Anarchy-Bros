using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Spot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Color ColorInEditor, ColorInGame;
    public int Index;
    public float HitRadius;
    public SpotTypes Type;
    public Piece Tower;
    public bool Occupied { get { return Tower != null; } }
    public List<Edge> Edges;
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

    GameManager _gameManager;
    SpriteRenderer _renderer;
    Collider2D _collider;
    bool _justScrolled;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (_gameManager.CurrentState)
        {
            case GameStates.Play:
                _renderer.color = Color.Lerp(_renderer.color, ColorInGame, Time.deltaTime * 8f);
                break;

            default:
                _renderer.color = Color.Lerp(_renderer.color, ColorInEditor, Time.deltaTime * 8f);
                break;
        }
    }

    public void AddEdge(Edge e)
    {
        Edges.Add(e);
    }

    public void RemoveEdge(Edge e)
    {
        Edges.Remove(e);
    }

    public Spot GetNeighbor(int edgeIndex)
    {
        return Edges[edgeIndex].Neighbor(this);
    }

    public Spot GetNeighbor(Edge edge)
    {
        return edge.Neighbor(this);
    }

    public bool GetEdge(Spot neighbor, out Edge linking)
    {
        linking = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            if (Edges[i].HasSpot(neighbor))
            {
                linking = Edges[i];
                break;
            }
        }

        return (linking != null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //_justScrolled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (_gameManager.IsCurrentState(GameStates.Play))
        //{
        //    Vector2 newPos = Camera.main.ScreenToWorldPoint(eventData.position);
        //    transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        //    _mapManager.OnSpotDrag(eventData, this);
        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (_gameManager.IsCurrentState(GameStates.Play))
        //{
        //    _mapManager.OnSpotDrag(eventData, this);
        //}
    }
}