using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnTime;
    public int MaxNumEnemies, MaxNumDeadBodies;
    public List<PieceBehavior> Objects;
    public int AliveEnemies {get { return CountAliveEnemies(); }}

    GameManager _gameManager;
    float _timeSinceLastSpawn;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _timeSinceLastSpawn = 0;
    }

    void Update()
    {
        switch (_gameManager.CurrentState)
        {
            case GameStates.Play:
                _timeSinceLastSpawn += Time.deltaTime;
                if (_timeSinceLastSpawn >= SpawnTime && AliveEnemies < MaxNumEnemies)
                {
                    SpawnEnemy();
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

    void SpawnEnemy()
    {
        PieceBehavior objective;
        Spot spawnSpot;

        if (NewTarget(out objective, out spawnSpot))
        {
            PieceBehavior obj = Find();
            obj.transform.position = spawnSpot.transform.position;
            obj.name = "Enemy";
            obj.gameObject.SetActive(true);
            obj.Live();
            obj.Movement.Step = spawnSpot;
            obj.Movement.CurrentSpot = spawnSpot;
        }
    }

    PieceBehavior Find()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior x = Objects[i];
            if (x.Reciclable)
            {
                return Objects[i];
            }
        }

        PieceBehavior piece = Instantiate(EnemyPrefab).GetComponent<PieceBehavior>();
        piece.name = tag.ToString();
        piece.transform.parent = transform;
        Objects.Add(piece);
        //piece.SetHealthElement(_uiController.GetHealthElement());

        return piece;
    }

    bool NewTarget(out PieceBehavior finalObjective, out Spot spawnSpot)
    {
        spawnSpot = _gameManager.Map.Graph.GetRandomSpot(SpotTypes.EnemySpawn);
        finalObjective = _gameManager.Towers.Random();

        return (finalObjective != null && spawnSpot != null);
    }

    int CountAliveEnemies()
    {
        int count = 0;

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Alive)
            {
                count++;
            }
        }

        return count;
    }

    public void Remove(PieceBehavior enemy)
    {
        Objects.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void RemoveAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Destroy(Objects[i].gameObject);
        }

        Objects.Clear();
    }

    public void KillAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            PieceBehavior x = Objects[i];
            if (x.Alive)
            {
                StartCoroutine(x.Die());
            }
        }
    }
}
