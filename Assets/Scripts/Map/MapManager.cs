using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    public Transform ObjGround, ObjGraph;
    public GameObject EditSourcePrefab, EditTargetPrefab;
    public SpotTypes CurrentMode;
    public GraphManager Graph;
    public GraphLogic GraphLogic;
    public bool Targeting { get; private set; }

    GameManager _gameManager;
    Transform _editSource, _editTarget;

    void Start()
    {
        _gameManager = GameManager.Instance;

        Graph = ObjGraph.GetComponent<GraphManager>();
        GraphLogic = ObjGraph.GetComponent<GraphLogic>();

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

    void Update()
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

    public void OnEditModeChanged(int newMode)
    {
        Targeting = false;
        CurrentMode = (SpotTypes)newMode;
    }

    public bool OutOfMapBounds(Vector2 position, Vector2 margin)
    {
        Vector2 mapBottomLeft = ObjGround.transform.position - 0.5f * ObjGround.transform.localScale;
        Vector2 mapTopRight = ObjGround.transform.position + 0.5f * ObjGround.transform.localScale;
        Vector2 objBottomLeft = position - margin;
        Vector2 objTopRight = position + margin;

        return Tools2D.NotInside(objBottomLeft, objTopRight, mapBottomLeft, mapTopRight);
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

        if (Targeting)
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
        Spot hitSpot;

        if (Graph.FindSpot(worldPos, out hitSpot))
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
        Spot hitSpot;

        if (Graph.FindSpot(worldPos, out hitSpot))
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
        Spot spotA, spotB;
        Edge hitEdge;

        if (!Graph.FindSpot(sourcePos, out spotA))
        {
            spotA = Graph.CreateSpot(sourcePos, CurrentMode);
            if (Graph.FindEdge(sourcePos, out hitEdge))
            {
                SplitEdge(hitEdge, spotA);
            }
        }

        if (!Targeting) return;

        if (!Graph.FindSpot(targetPos, out spotB))
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

    void SplitEdge(Edge hitEdge, Spot spliterSpot)
    {
        Spot spotA = hitEdge.A;
        hitEdge.A.RemoveEdge(hitEdge);
        hitEdge.A = spliterSpot;
        spliterSpot.AddEdge(hitEdge);
        Graph.CreateEdge(spliterSpot, spotA);
    }
}