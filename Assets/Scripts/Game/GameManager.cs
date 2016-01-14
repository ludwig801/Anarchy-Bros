using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform ObjMap, ObjTowers, ObjEnemies, ObjCanvas, ObjIO, ObjWounds;
    public GameStates CurrentState
    {
        get
        {
            return _currentState;
        }
    }
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
    public MapManager Map
    {
        get
        {
            if (_map == null)
            {
                _map = ObjMap.GetComponent<MapManager>();
            }

            return _map;
        }
    }
    public GameUIManager UI
    {
        get
        {
            if (_ui == null)
            {
                _ui = ObjCanvas.GetComponent<GameUIManager>();
            }
            return _ui;
        }
    }
    public WoundManager Wounds
    {
        get
        {
            if (_wounds == null)
            {
                _wounds = ObjWounds.GetComponent<WoundManager>();
            }
            return _wounds;
        }
    }
    public IOManager IO
    {
        get
        {
            if (_io == null)
            {
                _io = ObjIO.GetComponent<IOManager>();
            }
            return _io;
        }
    }
    public int Score, ScorePerKill;

    GameStates _currentState;
    TowerManager _towers;
    EnemyManager _enemies;
    MapManager _map;
    GameUIManager _ui;
    WoundManager _wounds;
    IOManager _io;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Score = 0;

        IO.LoadGraph();

        ChangeState(GameStates.Place);
    }

    void Update()
    {
        switch (CurrentState)
        {
            case GameStates.Play:
                if (Towers.AliveTowers <= 0)
                {
                    Towers.RemoveAll();
                    Enemies.RemoveAll();
                    ChangeState(GameStates.GameOver);
                }
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    ChangeState(GameStates.Pause);
                }
                break;

            case GameStates.Pause:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    ChangeState(GameStates.Play);
                }
                break;

            default:
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    LoadScene("initial");
                }
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OnPieceKilled(PieceBehavior requester)
    {
        if (requester.tag == Tags.Enemy.ToString())
        {
            Score += ScorePerKill;
        }      
    }

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
        else if (GameStates.Pause.ToString() == newState)
        {
            found = true;
            state = GameStates.Pause;
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
                if (Towers.HasSelectedTower)
                {
                    Towers.SelectedTower = null;
                }
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
        return Towers.GetClosestTower(requester, out closestTower) && Map.GraphLogic.ProvideTarget(requester, closestTower, out requester.Target);
    }

    public bool ProvideRangedPieceWithEnemyTarget(RangedPiece requester)
    {
        return Enemies.GetClosestEnemy(requester, out requester.Target);
    }

    public bool StepToTarget(MoveBehavior requester, out Spot newStep)
    {
        return Map.GraphLogic.StepToTarget(requester, out newStep);
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        PointerEventData pEventData = eventData as PointerEventData;
        Vector2 worldPos = pEventData.pointerCurrentRaycast.worldPosition;

        switch (CurrentState)
        {
            case GameStates.Play:
                HandlePlayClick(pEventData, worldPos);
                break;

            case GameStates.Pause:
                HandlePauseClick(pEventData, worldPos);
                break;

            case GameStates.Place:
                HandlePlaceClick(pEventData, worldPos);
                break;

            case GameStates.Edit:
                Map.HandleEditClick(pEventData, worldPos);
                break;

            default:
                break;
        }
    }

    void HandlePlayClick(PointerEventData eventData, Vector2 worldPos)
    {
        PieceBehavior hitTower;
        if (Towers.Find(worldPos, out hitTower))
        {
            Towers.SelectTower(hitTower);
            ChangeState(GameStates.Pause);
        }
    }

    void HandlePauseClick(PointerEventData eventData, Vector2 worldPos)
    {
        PieceBehavior hitTower;
        if (Towers.Find(worldPos, out hitTower) && hitTower.Alive)
        {
            if (Towers.HasSelectedTower)
            {
                if (hitTower == Towers.SelectedTower)
                {
                    ChangeState(GameStates.Play);
                }
                else
                {
                    Towers.SelectTower(hitTower);
                }
            }
            else
            {
                Towers.SelectTower(hitTower);
            }
            return;
        }

        Spot hitSpot;
        if (Map.Graph.FindSpotNear(worldPos, out hitSpot) && hitSpot.Type == SpotTypes.TowerSpot && Towers.HasSelectedTower && Towers.NoTowerHasTarget(hitSpot))
        {
            Towers.MoveTower(Towers.SelectedTower, hitSpot);
            ChangeState(GameStates.Play);
        }
    }

    void HandlePlaceClick(PointerEventData eventData, Vector2 worldPos)
    {
        Spot hitSpot;
        if (Map.Graph.FindSpotNear(worldPos, out hitSpot))
        {
            PieceBehavior hitTower;
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