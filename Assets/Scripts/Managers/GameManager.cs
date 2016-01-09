using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform ObjMap, ObjTowers, ObjEnemies, ObjCanvas, ObjIO;
    public Transform ObjWounds;
    public GameObject WoundPrefab;
    public List<Wound> Wounds;
    public TowerManager Towers
    {
        get
        {
            if (_towers == null)
            {
                _towers = ObjTowers.GetComponent<TowerManager>();
            }
            return _towers;
        }
    }
    public EnemyManager Enemies
    {
        get
        {
            if (_enemies == null)
            {
                _enemies = ObjEnemies.GetComponent<EnemyManager>();
            }
            return _enemies;
        }
    }
    public Map Map
    {
        get
        {
            if (_map == null)
            {
                _map = ObjMap.GetComponent<Map>();
            }

            return _map;
        }
    }

    TowerManager _towers;
    EnemyManager _enemies;
    Map _map;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameStates.Edit);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        switch (CurrentState)
        {
            case GameStates.Edit:
                break;

            case GameStates.Pause:
                break;

            case GameStates.Place:
                break;

            case GameStates.Play:
                if (Towers.ActiveTowers <= 0)
                {
                    Towers.RemoveAll();
                }
                break;
        }
    }

    public GameStates CurrentState { get { return _currentState; } }
    GameStates _currentState;

    public void ChangeState(string newState)
    {
        GameStates state = GameStates.Edit;
        bool found = false;
        if (GameStates.Edit.ToString() == newState)
        {
            found = true;
            state = GameStates.Edit;
        }
        else if (GameStates.Place.ToString() == newState)
        {
            found = true;
            state = GameStates.Place;
        }
        else if (GameStates.Play.ToString() == newState)
        {
            found = true;
            state = GameStates.Play;
        }

        if (found)
        {
            ChangeState(state);
        }
    }

    public void ChangeState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Edit:
                // Generic
                Time.timeScale = 1f;
                // Enemies
                Enemies.RemoveAll();
                Towers.RemoveAll();
                break;

            case GameStates.Place:
                // Generic
                Time.timeScale = 1f;
                // Enemies
                Enemies.RemoveAll();
                Towers.RemoveAll();
                break;

            case GameStates.Play:
                // Generic
                Time.timeScale = 1f;
                // Map
                Map.GraphLogic.ReCalculate();
                break;

            case GameStates.Pause:
                Time.timeScale = 0f;
                break;
        }

        _currentState = newState;
    }

    public bool IsCurrentState(GameStates comp)
    {
        return comp == _currentState;
    }

    public bool ProvideEnemyWithTargetSpot(MoveBehavior requester)
    {
        MoveBehavior closestTower;
        return Map.GetClosestTower(requester, out closestTower) && Map.GraphLogic.ProvideTarget(requester, closestTower, out requester.Target);
    }

    public bool StepToTarget(MoveBehavior requester, out Spot newStep)
    {
        return Map.GraphLogic.StepToTarget(requester, out newStep);
    }

    public void CreateWound(MoveBehavior movingPiece, Vector2 direction)
    {
        Wound wound = GetWound();
        wound.Follow = movingPiece.transform;
        wound.transform.position = wound.Follow.position;
        wound.transform.rotation = Tools2D.LookAt(direction);
        wound.Die();
    }

    Wound GetWound()
    {
        for (int i = 0; i < Wounds.Count; i++)
        {
            if (!Wounds[i].gameObject.activeSelf)
            {
                return Wounds[i];
            }
        }

        Wound wound = Instantiate(WoundPrefab).GetComponent<Wound>();
        wound.transform.SetParent(ObjWounds);
        wound.name = "Wound";
        Wounds.Add(wound);

        return wound;
    }

    public void OnPointerClick(PointerEventData eventData, Vector2 worldPos)
    {
        switch (CurrentState)
        {
            case GameStates.Play:
                HandlePlayClick(eventData, worldPos); 
                break;

            case GameStates.Pause:
                HandlePauseClick(eventData, worldPos);
                break;

            case GameStates.Place:
                HandlePlaceClick(eventData, worldPos);
                break;

            default:
                break;
        }
    }

    void HandlePlayClick(PointerEventData eventData, Vector2 worldPos)
    {
        Piece hitTower;
        if (Towers.Find(worldPos, out hitTower))
        {
            Towers.SelectTower(hitTower);
            ChangeState(GameStates.Pause);
        }
    }

    void HandlePauseClick(PointerEventData eventData, Vector2 worldPos)
    {
        Piece hitTower;
        if (Towers.Find(worldPos, out hitTower) && Towers.HasSelectedTower)
        {
            if (hitTower == Towers.SelectedTower)
            {
                Towers.SelectedTower = null;
                ChangeState(GameStates.Play);
            }
            else
            {
                Towers.SelectTower(hitTower);
            }
            return;
        }

        Spot hitSpot;
        if (Map.Graph.FindSpot(worldPos, out hitSpot) && hitSpot.Type == SpotTypes.TowerSpot && Towers.HasSelectedTower && Towers.NoTowerHasTarget(hitSpot))
        {
            Towers.MoveTower(Towers.SelectedTower, hitSpot);
            ChangeState(GameStates.Play);
        }
    }

    void HandlePlaceClick(PointerEventData eventData, Vector2 worldPos)
    {
        Spot hitSpot;
        if (Map.Graph.FindSpot(worldPos, out hitSpot))
        {
            Piece hitTower;
            if (Towers.Find(worldPos, out hitTower))
            {
                bool rightClick = (eventData.button == PointerEventData.InputButton.Right);

                if (rightClick)
                {
                    Towers.Remove(hitTower);
                }
            }
            else if (hitSpot.Type == SpotTypes.TowerSpot)
            {
                Towers.Spawn(hitSpot);
            }
        }
    }
}