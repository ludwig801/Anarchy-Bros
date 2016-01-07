using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public Transform ObjMap, ObjCanvas, ObjIO;
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

    Map _map;
    UIController _uiController;
    IOController _ioController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Generic
        _uiController = ObjCanvas.GetComponent<UIController>();
        _ioController = ObjIO.GetComponent<IOController>();

        // Piece Manager
        Pieces = new List<Piece>();

        // Enemy Manager
        _enemyDeltaTime = 0;

        // Game States
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
                ActiveTowers = NumActivePieces(Tags.Tag.Tower);
                break;

            case GameStates.Play:
                ActiveTowers = NumActivePieces(Tags.Tag.Tower);
                ActiveEnemies = NumActivePieces(Tags.Tag.Enemy);
                if (ActiveTowers <= 0)
                {
                    KillAllPieces();
                    return;
                }
                _enemyDeltaTime += Time.deltaTime;
                if (_enemyDeltaTime >= SpawnTime && ActiveEnemies < MaxNumEnemies)
                {
                    SpawnEnemy();
                    _enemyDeltaTime = 0;
                }
                break;
        }
    }

    #region Game States
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
                DestroyAllPieces();
                // Towers
                DestroyAllBullets();
                break;

            case GameStates.Place:
                // Generic
                Time.timeScale = 1f;
                // Enemies
                DestroyAllPieces();
                // Towers
                DestroyAllBullets();
                break;

            case GameStates.Play:
                // Generic
                Time.timeScale = 1f;
                // Map
                _map.ReCalculateDistances();
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
    #endregion

    #region Piece Manager
    public List<Piece> Pieces;

    public Piece PieceAt(Transform t, Tags.Tag tag)
    {
        if (t == null) return null;

        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];

            if (x.tag == Tags.GetStringTag(tag) && x.Collider.OverlapPoint(t.position))
            {
                return x;
            }
        }

        return null;
    }

    public void OnSpotClicked(Spot spot)
    {
        switch (CurrentState)
        {
            case GameStates.Place:
                if (spot.Type == SpotTypes.TowerSpot && ActiveTowers < MaxNumTowers)
                {
                    SpawnTower(Tags.Tag.Tower, spot);
                }
                break;

            case GameStates.Play:
                if (spot.Type == SpotTypes.TowerSpot)
                {
                    if (SelectedTower != null)
                    {
                        if (spot.Occupied)
                        {
                            SelectedTower = spot.Tower;
                            ChangeState(GameStates.Pause);
                        }
                        else
                        {
                            spot.Tower = SelectedTower;
                        }
                    }
                    else
                    {
                        SelectedTower = spot.Tower;
                    }
                }
                break;

            case GameStates.Pause:
                if (spot.Type == SpotTypes.TowerSpot)
                {
                    if (SelectedTower != null)
                    {
                        if (spot.Occupied)
                        {
                            SelectedTower = spot.Tower;
                        }
                        else
                        {
                            spot.Tower = SelectedTower;
                        }
                    }
                    else
                    {
                        SelectedTower = spot.Tower;
                    }
                }
                break;
        }
    }

    Piece GetPiece(Tags.Tag tag, GameObject prefab, Transform parent)
    {
        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];
            if (x.tag == Tags.GetStringTag(tag) && !x.gameObject.activeSelf)
            {
                return Pieces[i];
            }
        }

        Piece piece = Instantiate(prefab).GetComponent<Piece>();
        piece.name = tag.ToString();
        piece.transform.parent = parent;
        Pieces.Add(piece);
        piece.SetHealthElement(_uiController.GetHealthElement());

        return piece;
    }

    int NumActivePieces(Tags.Tag tag)
    {
        string sTag = Tags.GetStringTag(tag);
        int count = 0;

        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];
            if (x.tag == sTag && x.gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    void DestroyAllPieces()
    {
        for (int i = 0; i < Pieces.Count; i++)
        {
            Destroy(Pieces[i].gameObject);
        }

        Pieces.Clear();
    }

    void KillAllPieces()
    {
        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];
            if (x.Alive)
            {
                StartCoroutine(Pieces[i].Die());
            }
        }
    }

    public Transform NewTarget(Tags.Tag tag)
    {
        Transform newTarget = null;
        if (tag == Tags.Tag.Enemy)
        {
            // TODO
        }
        else if (tag == Tags.Tag.Tower)
        {
            Piece newObjective;
            if (GetObjective(out newObjective))
            {
                newTarget = newObjective.transform;
            }
        }

        return newTarget;
    }
    #endregion

    #region Tower Manager
    public Transform ObjTowers, ObjBullets;
    public GameObject TowerPrefab;
    public int MaxNumTowers;
    public Piece SelectedTower;
    public int ActiveTowers;

    public void SpawnTower(Tags.Tag tag, Spot spot)
    {
        Piece tower = GetPiece(Tags.Tag.Tower, TowerPrefab, ObjTowers);
        tower.gameObject.SetActive(true);
        spot.Tower = tower;
        tower.transform.position = Tools2D.Convert(tower.transform.position, spot.transform.position);
        tower.Target = spot.transform;
        tower.MoveTo = spot.transform;
        tower.Live();
    }

    public Piece GetRandomTower()
    {
        int rand = Random.Range(0, ActiveTowers);
        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];
            if (x.tag == Tags.GetStringTag(Tags.Tag.Tower) && x.Alive)
            {
                if (rand <= 0) return x;
                rand--;
            }
        }

        return null;
    }

    void DestroyAllBullets()
    {
        for (int i = 0; i < ObjBullets.childCount; i++)
        {
            Destroy(ObjBullets.GetChild(i).gameObject);
        }
    }
    #endregion

    #region Enemy Manager
    public Transform ObjEnemies;
    public GameObject EnemyPrefab;
    public float SpawnTime;
    public int MaxNumEnemies;
    public int ActiveEnemies;

    float _enemyDeltaTime;

    public Piece GetNearest(Tags.Tag tag, Transform t)
    {
        float minDist = float.MaxValue;
        Piece closest = null;

        for (int i = 0; i < Pieces.Count; i++)
        {
            Piece x = Pieces[i];
            if ((x.tag == Tags.GetStringTag(tag)) && x.Alive)
            {
                float d = _map.DistanceBetween(t, x.transform);
                if (d < minDist)
                {
                    minDist = d;
                    closest = x;
                }
            }
        }

        return closest;
    }

    public bool GetObjective(out Piece newObjective)
    {
        newObjective = GetRandomTower();
        return (newObjective != null);
    }

    public bool GetObjective(out Piece finalObjective, out Spot spawnSpot)
    {
        spawnSpot = _map.Spots.Random(SpotTypes.EnemySpawn);
        finalObjective = GetRandomTower();

        return (finalObjective != null && spawnSpot != null);
    }

    void SpawnEnemy()
    {
        Piece objective;
        Spot spawnSpot;

        if (GetObjective(out objective, out spawnSpot))
        {
            Piece enemy = GetPiece(Tags.Tag.Enemy, EnemyPrefab, ObjEnemies);
            enemy.transform.position = spawnSpot.transform.position;
            enemy.name = "Enemy";
            enemy.MoveTo = spawnSpot.transform;
            enemy.Target = objective.transform;
            enemy.gameObject.SetActive(true);
            enemy.Live();
        }
    }
    #endregion

    #region Misc
    public Transform ObjWounds;
    public GameObject WoundPrefab;
    public List<Wound> Wounds;

    public void CreateWound(Transform t, Vector2 direction)
    {
        Wound wound = GetWound();
        wound.Follow = t;
        wound.transform.position = t.position;
        wound.transform.rotation = Tools2D.LookAt(direction);
        wound.Die();
    }

    public Wound GetWound()
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
    #endregion
}