using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    public Transform ObjGround, ObjGraph;
    public SpotTypes CurrentMode;
    public Graph Graph;
    public GraphLogic GraphLogic;
    public bool Targeting
    {
        get { return _targeting; }

        private set
        {
            _targeting = value;
        }
    }

    GameManager _gameManager;
    Vector2 _editSource, _editTarget, _mapBottomLeft, _mapTopRight;
    bool _targeting;

    void Start()
    {
        _gameManager = GameManager.Instance;

        Graph = ObjGraph.GetComponent<Graph>();
        GraphLogic = ObjGraph.GetComponent<GraphLogic>();

        Targeting = false;
        CurrentMode = SpotTypes.Connection;
    }

    void Update()
    {
        _mapBottomLeft = ObjGround.transform.position - 0.5f * ObjGround.transform.localScale;
        _mapTopRight = ObjGround.transform.position + 0.5f * ObjGround.transform.localScale;
    }

    void RemoveSpotsAndEdges(Spot spot)
    {
        Graph.Remove(spot);
    }

    public bool GetClosestTower(MoveBehavior requester, out Piece closestTower)
    {
        float minDist = float.MaxValue;
        closestTower = null;

        for (int i = 0; i < _gameManager.Towers.Objects.Count; i++)
        {
            Piece tower = _gameManager.Towers.Objects[i];
            if (tower.Alive)
            {
                float d = GraphLogic.DistanceBetween(requester, tower.Movement);
                if (d < minDist)
                {
                    minDist = d;
                    closestTower = tower;
                }
            }
        }

        return (closestTower != null);
    }

    public void OnModeChanged(int newMode)
    {
        Targeting = false;
        CurrentMode = (SpotTypes)newMode;
    }

    public bool OutOfMap(Vector2 position, Vector2 margin)
    {
        Vector2 objBottomLeft = position - margin;
        Vector2 objTopRight = position + margin;

        return Tools2D.NotInside(objBottomLeft, objTopRight, _mapBottomLeft, _mapTopRight);
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        PointerEventData pEventData = eventData as PointerEventData;
        Vector2 worldPos = pEventData.pointerCurrentRaycast.worldPosition;

        _gameManager.OnPointerClick(pEventData, worldPos);

        if (_gameManager.Towers.OnPointerClick(worldPos))
        {
            Debug.Log("Found Tower");
            return;
        }

        if (Graph.OnPointerClick(worldPos))
        {
            Debug.Log("Found Spot or Edge");
            return;
        }

        Debug.Log("Ground");

        //bool leftBtn = pEventData.button == PointerEventData.InputButton.Left;
        ////bool rightBtn = pEventData.button == PointerEventData.InputButton.Right;

        //bool touchedSpot = Graph.FindSpot(worldPos, Graph.Spo);
        //bool touchedEdge;
        //Spot spot;
        //Edge edge;

        //switch (_gameController.CurrentState)
        //{
        //    case GameStates.Edit:
        //        #region Edit
        //        if (Graph.FindSpot(worldPos, out spot, 0.5f))
        //        {
        //            Debug.Log("Touched spot"); return;
        //            switch (CurrentMode)
        //            {
        //                case SpotTypes.Connection:
        //                    if (leftBtn)
        //                    {
        //                        if (Targeting)
        //                        {
        //                            _editTarget = spot.transform.position;
        //                            CreateLink(_editSource, _editTarget);
        //                            _editSource = _editTarget;  // For continuos targeting
        //                        }
        //                        else
        //                        {
        //                            Targeting = true;
        //                            _editSource = spot.transform.position;
        //                            _editTarget = _editSource;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Targeting)
        //                        {
        //                            Targeting = false;
        //                        }
        //                        else
        //                        {
        //                            if (spot.Type != SpotTypes.Connection)
        //                            {
        //                                Graph.ChangeType(spot, SpotTypes.Connection);
        //                            }
        //                            else
        //                            {
        //                                RemoveSpotsAndEdges(spot);
        //                            }
        //                        }
        //                    }
        //                    break;

        //                case SpotTypes.EnemySpawn:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        Graph.ChangeType(spot, SpotTypes.EnemySpawn);
        //                    }
        //                    else
        //                    {
        //                        if (spot.Type == SpotTypes.Connection)
        //                        {
        //                            if (Targeting)
        //                            {
        //                                Targeting = false;
        //                            }
        //                            else
        //                            {
        //                                RemoveSpotsAndEdges(spot);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Graph.ChangeType(spot, SpotTypes.Connection);
        //                        }
        //                    }
        //                    break;

        //                case SpotTypes.TowerSpot:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        Graph.ChangeType(spot, SpotTypes.TowerSpot);
        //                    }
        //                    else
        //                    {
        //                        if (spot.Type == SpotTypes.Connection)
        //                        {
        //                            if (Targeting)
        //                            {
        //                                Targeting = false;
        //                            }
        //                            else
        //                            {
        //                                RemoveSpotsAndEdges(spot);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Graph.ChangeType(spot, SpotTypes.Connection);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //        else if (Graph.FindEdge(worldPos, out edge, 0.02f))
        //        {
        //            Debug.Log("Touched edge"); return;
        //            switch (CurrentMode)
        //            {
        //                case SpotTypes.Connection:
        //                    if (leftBtn)
        //                    {
        //                        if (Targeting)
        //                        {
        //                            _editTarget = worldPos;
        //                            CreateLink(_editSource, _editTarget);

        //                            // For continuos targeting
        //                            _editSource = _editTarget;
        //                            // For one time targeting
        //                            // Targeting = false;
        //                        }
        //                        else
        //                        {
        //                            Targeting = true;
        //                            _editSource = worldPos;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Targeting)
        //                        {
        //                            Targeting = false;
        //                        }
        //                        else
        //                        {
        //                            Graph.Remove(edge);
        //                        }
        //                    }
        //                    break;

        //                case SpotTypes.TowerSpot:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        _editSource = worldPos;
        //                        CreateLink(_editSource);
        //                    }
        //                    else
        //                    {
        //                        if (Targeting)
        //                        {
        //                            Targeting = false;
        //                        }
        //                        else
        //                        {
        //                            Graph.Remove(edge);
        //                        }
        //                    }
        //                    break;

        //                case SpotTypes.EnemySpawn:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        _editSource = worldPos;
        //                        CreateLink(_editSource);
        //                    }
        //                    else
        //                    {
        //                        if (Targeting)
        //                        {
        //                            Targeting = false;
        //                        }
        //                        else
        //                        {
        //                            Graph.Remove(edge);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("Touched ground"); return;
        //            switch (CurrentMode)
        //            {
        //                case SpotTypes.Connection:
        //                    if (leftBtn)
        //                    {
        //                        if (Targeting)
        //                        {
        //                            _editTarget = worldPos;
        //                            CreateLink(_editSource, _editTarget);
        //                            _editSource = _editTarget;  // For continuos targeting
        //                        }
        //                        else
        //                        {
        //                            Targeting = true;
        //                            _editSource = worldPos;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Targeting = false;
        //                    }
        //                    break;

        //                case SpotTypes.EnemySpawn:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        Graph.CreateSpot(worldPos, SpotTypes.EnemySpawn);
        //                    }
        //                    else
        //                    {
        //                        Targeting = false;
        //                    }
        //                    break;

        //                case SpotTypes.TowerSpot:
        //                    if (leftBtn)
        //                    {
        //                        Targeting = false;
        //                        Graph.CreateSpot(worldPos, SpotTypes.TowerSpot);
        //                    }
        //                    else
        //                    {
        //                        Targeting = false;
        //                    }
        //                    break;
        //            }
        //        }
        //        #endregion
        //        break;

        //    case GameStates.Place:
        //        #region Place
        //        if (Graph.FindSpot(worldPos, out spot, 0.5f))
        //        {
        //            Debug.Log("Touched spot");
        //            //_gameController.OnSpotClicked(spot);
        //        }
        //        else if (Graph.FindEdge(worldPos, out edge, 0.01f))
        //        {
        //            Debug.Log("Touched edge");
        //        }
        //        else
        //        {
        //            Debug.Log("Touched ground");
        //        }
        //        #endregion
        //        break;

        //    case GameStates.Play:
        //        #region Play
        //        #endregion
        //        break;
        //}
    }
}