using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // Public vars
    public Transform ObjMap, ObjTowers, ObjEnemies, ObjCanvas, ObjIO, ObjWounds;
    public GameStates CurrentState;
    public int Score, ScorePerKill;
    public bool Debugging;
    // Properties
    public GlobalManager GlobalManager
    {
        get
        {
            return GlobalManager.Instance;
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
    // Private vars
    TowerManager _towers;
    EnemyManager _enemies;
    MapManager _map;
    GameUIManager _ui;
    WoundManager _wounds;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debugging = GlobalManager.Instance.Debugging;
        ObjIO.GetComponent<IOManager>().LoadGraph();

        Enemies.MaxNumDeadBodies = GlobalManager.BodyCountValue * 5;
        Enemies.MaxNumEnemies = (GlobalManager.Difficulty + 1) * 4;

        ChangeState(GameStates.Place);
        Score = 0;
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
                    GlobalManager.InGame = false;
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
            _enemies.OnEnemyKilled();
            Score += ScorePerKill;
        }
        else if (requester.tag == Tags.Tower.ToString())
        {
            _towers.OnTowerKilled();
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

        CurrentState = newState;
    }

    public bool IsCurrentState(GameStates comp)
    {
        return comp == CurrentState;
    }

    public bool ProvideEnemyWithTargetSpot(MoveBehavior requester)
    {
        MoveBehavior closestTower;
        return Towers.GetClosestTower(requester, out closestTower) && Map.GraphLogic.ProvideTarget(requester, closestTower, out requester.Target);
    }

    public bool ProvideRangedPieceWithEnemyTarget(RangedBehavior requester)
    {
        return Enemies.GetClosestEnemy(requester, out requester.Target);
    }

    public bool StepToTarget(MoveBehavior requester, out MapSpot newStep)
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

        MapSpot hitSpot;
        if (Towers.HasSelectedTower && Map.Graph.FindSpotNear(worldPos, out hitSpot, SpotTypes.TowerSpot) && Towers.NoTowerHasTarget(hitSpot))
        {
            Towers.MoveTower(Towers.SelectedTower, hitSpot);
            ChangeState(GameStates.Play);
        }
    }

    void HandlePlaceClick(PointerEventData eventData, Vector2 worldPos)
    {
        MapSpot hitSpot;
        if (Map.Graph.FindSpotNear(worldPos, out hitSpot, SpotTypes.TowerSpot))
        {
            if (hitSpot.Occupied)
            {
                Towers.Remove(hitSpot.Piece);
            }
            else if (hitSpot.Type == SpotTypes.TowerSpot)
            {
                Towers.Spawn(hitSpot);
            }
        }
    }
}