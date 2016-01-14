using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Spot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Color ColorInEditor, ColorInGame, ColorInGamePaused;
    public SortingLayers LayerDefault, LayerInGamePaused;
    public int Index;
    public float ScaleInEditor, ScaleInGame, ScaleInGamePaused;
    public SpotTypes Type;
    public PieceBehavior Piece;
    public bool Occupied { get { return Piece != null; } }
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
    int _layerDefault, _layerInGamePaused;
    Vector3 _scaleDefault;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _renderer = GetComponent<SpriteRenderer>();

        _layerDefault = SortingLayer.NameToID(LayerDefault.ToString());
        _layerInGamePaused = SortingLayer.NameToID(LayerInGamePaused.ToString());

        _scaleDefault = transform.localScale;
    }

    void Update()
    {
        if (Piece != null && !Piece.Alive)
        {
            Piece = null;
        }

        switch (_gameManager.CurrentState)
        {
            case GameStates.Play:
                _renderer.color = Color.Lerp(_renderer.color, ColorInGame, Time.deltaTime * 8f);
                _renderer.sortingLayerID = _layerDefault;
                transform.localScale = Vector3.Lerp(transform.localScale, Tools2D.Multiply(_scaleDefault, ScaleInGame), Time.deltaTime * 8f);
                break;

            case GameStates.Edit:
                _renderer.color = Color.Lerp(_renderer.color, ColorInEditor, Time.deltaTime * 8f);
                _renderer.sortingLayerID = _layerDefault;
                transform.localScale = Vector3.Lerp(transform.localScale, Tools2D.Multiply(_scaleDefault, ScaleInEditor), Time.deltaTime * 8f);
                break;

            default:
                _renderer.color = Color.Lerp(_renderer.color, ColorInGamePaused, Time.unscaledDeltaTime * 8f);
                _renderer.sortingLayerID = _layerInGamePaused;
                transform.localScale = Vector3.Lerp(transform.localScale, Tools2D.Multiply(_scaleDefault, ScaleInGamePaused), Time.unscaledDeltaTime * 8f);
                //if (!Occupied)
                //{
                //    transform.localScale = Vector3.Lerp(transform.localScale, Tools2D.Multiply(_scaleDefault, ScaleInGamePaused), Time.unscaledDeltaTime * 8f);
                //}
                //else
                //{
                //    transform.localScale = Vector3.Lerp(transform.localScale, Tools2D.Multiply(_scaleDefault, ScaleInGamePaused), Time.deltaTime * 8f);
                //}
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