using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Spot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Color ColorInEditor, ColorInGame;
    public int Index { get; set; }
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

    SpriteRenderer _renderer;
    Color _colorTo;
    Collider2D _collider;
    bool _justScrolled;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        OnGameStateChanged(GameController.Instance.CurrentState);
    }

    void Update()
    {
        _renderer.color = Color.Lerp(_renderer.color, _colorTo, Time.deltaTime * 8f);
    }

    public void AddEdge(Edge e)
    {
        Edges.Add(e);
    }

    public void RemoveEdge(Edge e)
    {
        Edges.Remove(e);
    }

    public Spot GetNeighbor(int index)
    {
        return Edges[index].Neighbor(this);
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