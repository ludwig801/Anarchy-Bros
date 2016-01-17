using UnityEngine;
using System.Collections.Generic;

public class MapSpot : MonoBehaviour
{
    // Public vars
    public Color ColorInEditor, ColorInGame, ColorInGamePaused;
    public SortingLayers LayerDefault, LayerInGamePaused;
    public SpotTypes Type;
    public PieceBehavior Piece;
    public List<MapEdge> Edges;
    public int Index;
    public float ScaleInEditor, ScaleInGame, ScaleInGamePaused;
    // Properties
    public bool Occupied
    {
        get
        {
            return Piece != null;
        }
    }
    // Private vars
    GameManager _gameManager;
    SpriteRenderer _renderer;
    int _layerDefault, _layerInGamePaused;
    Vector3 _scaleInEditor, _scaleInGame, _scaleInGamePaused;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _renderer = GetComponent<SpriteRenderer>();
        _layerDefault = SortingLayer.NameToID(LayerDefault.ToString());
        _layerInGamePaused = SortingLayer.NameToID(LayerInGamePaused.ToString());
        _scaleInEditor = Tools2D.Multiply(transform.localScale, ScaleInEditor);
        _scaleInGame = Tools2D.Multiply(transform.localScale, ScaleInGame);
        _scaleInGamePaused = Tools2D.Multiply(transform.localScale, ScaleInGamePaused);
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
                transform.localScale = Vector3.Lerp(transform.localScale, _scaleInGame, Time.deltaTime * 8f);
                break;

            case GameStates.Edit:
                _renderer.color = Color.Lerp(_renderer.color, ColorInEditor, Time.deltaTime * 8f);
                _renderer.sortingLayerID = _layerDefault;
                transform.localScale = Vector3.Lerp(transform.localScale, _scaleInEditor, Time.deltaTime * 8f);
                break;

            default:
                _renderer.color = Color.Lerp(_renderer.color, ColorInGamePaused, Time.unscaledDeltaTime * 8f);
                _renderer.sortingLayerID = _layerInGamePaused;
                transform.localScale = Vector3.Lerp(transform.localScale, _scaleInGamePaused, Time.unscaledDeltaTime * 8f);
                break;
        }
    }

    public bool FindEdge(MapSpot neighbor, out MapEdge linking)
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
}