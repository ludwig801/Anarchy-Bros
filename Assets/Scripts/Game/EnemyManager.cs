using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnTime;
    public int MaxNumEnemies, MaxNumDeadBodies;
    public List<PieceBehavior> Objects;
    public int AliveEnemies;

    GameManager _gameManager;
    float _timeSinceLastSpawn;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _timeSinceLastSpawn = 0;
        AliveEnemies = 0;
    }

    void Update()
    {
        switch (_gameManager.CurrentState)
        {
            case GameStates.Play:
                _timeSinceLastSpawn += Time.deltaTime;
                if (_timeSinceLastSpawn >= SpawnTime && AliveEnemies < MaxNumEnemies)
                {
                    Spawn();
                    _timeSinceLastSpawn = 0;
                }

                for (int i = 0; i < Objects.Count; i++)
                {
                    PieceBehavior enemy = Objects[i];
                    if (enemy.Alive && !_gameManager.ProvideEnemyWithTargetSpot(enemy.Movement))
                    {
                        StartCoroutine(enemy.Die());
                    }
                }
                break;

            default:
                _timeSinceLastSpawn = 0;
                break;
        }
    }

    void Spawn()
    {
        PieceBehavior objective;
        MapSpot spawnSpot;

        if (NewTarget(out objective, out spawnSpot))
        {
            PieceBehavior obj = Find();
            obj.transform.position = spawnSpot.transform.position;
            obj.name = "Enemy";
            obj.gameObject.SetActive(true);
            obj.Live();
            obj.Movement.Step = spawnSpot;
            obj.Movement.CurrentSpot = spawnSpot;
            obj.Renderer.sortingOrder = 1;
            _gameManager.UI.CreateEnemyHealthElement(obj);
            AliveEnemies++;
        }
    }

    PieceBehavior Find()
    {
        float deadBodies = Objects.Count - AliveEnemies;
        if (deadBodies > MaxNumDeadBodies)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                PieceBehavior enemy = Objects[i];
                if (enemy.Reciclable)
                {
                    return enemy;
                }
            }
        }

        PieceBehavior piece = Instantiate(EnemyPrefab).GetComponent<PieceBehavior>();
        piece.name = tag.ToString();
        piece.transform.parent = transform;
        Objects.Add(piece);

        return piece;
    }

    bool NewTarget(out PieceBehavior finalObjective, out MapSpot spawnSpot)
    {
        spawnSpot = null;
        finalObjective = null;

        return (_gameManager.Map.Graph.GetRandomSpot(SpotTypes.EnemySpawn, out spawnSpot) &&
            _gameManager.Towers.GetRandomTower(out finalObjective));
    }

    public bool GetClosestEnemy(RangedBehavior requester, out MoveBehavior closestEnemy)
    {
        float minDist = float.MaxValue;
        closestEnemy = null;

        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior enemy = Objects[i];
            if (enemy.Alive && Tools2D.Distance(enemy.transform.position, requester.Piece.transform.position) <= requester.Range)
            {
                float d = _gameManager.Map.GraphLogic.DistanceBetween(requester.Piece.Movement, enemy.Movement);
                if (d < minDist)
                {
                    minDist = d;
                    closestEnemy = enemy.Movement;
                }
            }
        }

        return (closestEnemy != null);
    }

    public void RemoveAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Destroy(Objects[i].gameObject);
        }

        Objects.Clear();
    }

    public void OnEnemyKilled()
    {
        AliveEnemies--;
    }
}
