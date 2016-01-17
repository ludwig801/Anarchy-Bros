using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    public Transform ObjGround, ObjGraph;
    public GameObject EditSourcePrefab, EditTargetPrefab;
    public SpotTypes CurrentMode;
    public GraphManager Graph
    {
        get
        {
            if (_graph == null)
            {
                _graph = ObjGraph.GetComponent<GraphManager>();
            }
            return _graph;
        }
    }
    public GraphLogic GraphLogic
    {
        get
        {
            if (_graphLogic == null)
            {
                _graphLogic = ObjGraph.GetComponent<GraphLogic>();
            }
            return _graphLogic;
        }
    }
    public bool Targeting { get; private set; }
    public Vector2 Center, Size;

    GraphManager _graph;
    GraphLogic _graphLogic;
    GameManager _gameManager;
    Transform _editSource, _editTarget;

    void Start()
    {
        _gameManager = GameManager.Instance;

        if (_gameManager.Debugging)
        {
            _editSource = Instantiate(EditSourcePrefab).transform;
            _editSource.name = "Source [Edit Only]";
            _editSource.transform.SetParent(transform);
            _editSource.gameObject.SetActive(false);

            _editTarget = Instantiate(EditTargetPrefab).transform;
            _editTarget.name = "Target [Edit Only]";
            _editTarget.transform.SetParent(transform);
            _editTarget.gameObject.SetActive(false);

            Targeting = false;
            CurrentMode = SpotTypes.Connection;
        }

        SpriteRenderer groundRenderer = ObjGround.GetComponent<SpriteRenderer>();
        groundRenderer.sprite = _gameManager.GlobalManager.CurrentLevel.BackgroundSprite;
        float mapWidth = ObjGround.localScale.x * groundRenderer.sprite.textureRect.width / groundRenderer.sprite.pixelsPerUnit;
        float mapHeight = ObjGround.localScale.y * groundRenderer.sprite.textureRect.height / groundRenderer.sprite.pixelsPerUnit;
        Size = new Vector3(mapWidth, mapHeight, 1);
        Center = ObjGraph.position;
        ObjGround.GetComponent<BoxCollider2D>().size = Tools2D.Divide(Size, ObjGround.localScale);
    }

    void Update()
    {
        if (_gameManager.Debugging)
        {
            switch (_gameManager.CurrentState)
            {
                case GameStates.Edit:
                    _editSource.gameObject.SetActive(true);
                    _editTarget.gameObject.SetActive(Targeting);
                    if (Targeting)
                    {
                        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _editTarget.position = new Vector3(newPos.x, newPos.y, _editTarget.position.z);
                    }
                    else
                    {
                        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _editSource.position = new Vector3(newPos.x, newPos.y, _editSource.position.z);
                    }
                    break;

                default:
                    _editSource.gameObject.SetActive(false);
                    _editTarget.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void OnEditModeChanged(int newMode)
    {
        Targeting = false;
        CurrentMode = (SpotTypes)newMode;
    }

    public bool OutOfBounds(Transform requester)
    {
        Vector2 requesterPos = transform.position;
        Vector2 requesterScale = transform.localScale;
        Vector2 requesterBottomLeft = requesterPos - requesterScale;
        Vector2 requesterTopRight = requesterPos + requesterScale;

        return Tools2D.NotInside(requesterBottomLeft, requesterTopRight, 0.5f * -Size, 0.5f * Size);
    }

    public void HandleEditClick(PointerEventData eventData, Vector2 worldPos)
    {
        switch (CurrentMode)
        {
            case SpotTypes.Connection:
                HandleConnectionModeClick(eventData, worldPos);
                break;

            case SpotTypes.TowerSpot:
                HandleTowerSpotModeClick(eventData, worldPos);
                break;

            case SpotTypes.EnemySpawn:
                HandleEnemySpawnModeClick(eventData, worldPos);
                break;
        }
    }

    void HandleConnectionModeClick(PointerEventData eventData, Vector2 worldPos)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        MapSpot hitSpot;

        if (Graph.FindSpotNear(worldPos, out hitSpot) && rightClick)
        {
            if (hitSpot.Type != SpotTypes.Connection)
            {
                Graph.ChangeSpotType(hitSpot, SpotTypes.Connection);
            }
            else
            {
                Graph.RemoveSpot(hitSpot);
            }
        }
        else if (Targeting)
        {
            if (leftClick)
            {
                CreateLink();
                _editSource.position = _editTarget.position;
            }
            else if (rightClick)
            {
                Targeting = false;
            }
        }
        else
        {
            Targeting = leftClick && !rightClick;
        }
    }

    void HandleTowerSpotModeClick(PointerEventData eventData, Vector2 worldPos)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        MapSpot hitSpot;

        if (Graph.FindSpotNear(worldPos, out hitSpot))
        {
            if (leftClick)
            {
                Graph.ChangeSpotType(hitSpot, SpotTypes.TowerSpot);
            }
            else if (rightClick && hitSpot.Type != SpotTypes.Connection)
            {
                Graph.ChangeSpotType(hitSpot, SpotTypes.Connection);
            }          
        }
    }

    void HandleEnemySpawnModeClick(PointerEventData eventData, Vector2 worldPos)
    {
        bool leftClick = eventData.button == PointerEventData.InputButton.Left;
        bool rightClick = eventData.button == PointerEventData.InputButton.Right;
        MapSpot hitSpot;

        if (Graph.FindSpotNear(worldPos, out hitSpot))
        {
            if (leftClick)
            {
                Graph.ChangeSpotType(hitSpot, SpotTypes.EnemySpawn);
            }
            else if (rightClick && hitSpot.Type != SpotTypes.Connection)
            {
                Graph.ChangeSpotType(hitSpot, SpotTypes.Connection);
            }
        }
    }

    void CreateLink()
    {
        Vector2 sourcePos = _editSource.position;
        Vector2 targetPos = _editTarget.position;
        MapSpot spotA, spotB;
        MapEdge hitEdge;

        if (!Graph.FindSpotNear(sourcePos, out spotA))
        {
            spotA = Graph.CreateSpot(sourcePos, CurrentMode);
            if (Graph.FindEdge(sourcePos, out hitEdge))
            {
                SplitEdge(hitEdge, spotA);
            }
        }

        if (!Targeting) return;

        if (!Graph.FindSpotNear(targetPos, out spotB))
        {
            spotB = Graph.CreateSpot(targetPos, CurrentMode);
            if (Graph.FindEdge(targetPos, out hitEdge))
            {
                SplitEdge(hitEdge, spotB);
            }
        }

        if (Graph.FindEdge(spotA, spotB)) return;

        Graph.CreateEdge(spotA, spotB);

    }

    void SplitEdge(MapEdge hitEdge, MapSpot spliterSpot)
    {
        MapSpot spotA = hitEdge.A;
        hitEdge.A.Edges.Remove(hitEdge);
        hitEdge.A = spliterSpot;
        spliterSpot.Edges.Add(hitEdge);
        Graph.CreateEdge(spliterSpot, spotA);
    }
}