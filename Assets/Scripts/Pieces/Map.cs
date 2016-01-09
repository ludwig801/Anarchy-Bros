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

    public bool GetClosestTower(MoveBehavior requester, out MoveBehavior closestTower)
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
                    closestTower = tower.Movement;
                }
            }
        }

        return (closestTower != null);
    }

    public bool GetClosestEnemy(MoveBehavior requester, out MoveBehavior closestEnemy)
    {
        float minDist = float.MaxValue;
        closestEnemy = null;

        for (int i = 0; i < _gameManager.Enemies.Objects.Count; i++)
        {
            Piece enemy = _gameManager.Enemies.Objects[i];
            if (enemy.Alive)
            {
                float d = GraphLogic.DistanceBetween(requester, enemy.Movement);
                if (d < minDist)
                {
                    minDist = d;
                    closestEnemy = enemy.Movement;
                }
            }
        }

        return (closestEnemy != null);
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
    }
}