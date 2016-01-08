using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnTime;
    public int MaxNumEnemies;
    public List<Piece> Objects;
    public int ActiveEnemies {get { return CountActiveEnemies(); }}

    GameManager _gameController;
    float _timeSinceLastSpawn;

    void Start()
    {
        _gameController = GameManager.Instance;

        _timeSinceLastSpawn = 0;
    }

    void Update()
    {
        switch (_gameController.CurrentState)
        {
            case GameStates.Play:
                _timeSinceLastSpawn += Time.deltaTime;
                if (_timeSinceLastSpawn >= SpawnTime && ActiveEnemies < MaxNumEnemies)
                {
                    SpawnEnemy();
                    _timeSinceLastSpawn = 0;
                }
                break;
        }
    }

    void SpawnEnemy()
    {
        Piece objective;
        Spot spawnSpot;

        if (NewTarget(out objective, out spawnSpot))
        {
            Piece obj = Find();
            obj.transform.position = spawnSpot.transform.position;
            obj.name = "Enemy";
            obj.gameObject.SetActive(true);
            obj.Live();
            obj.Movement.Step = spawnSpot;
            obj.Movement.CurrentSpot = spawnSpot;
        }
    }

    Piece Find()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Piece x = Objects[i];
            if (!x.Alive)
            {
                return Objects[i];
            }
        }

        Piece piece = Instantiate(EnemyPrefab).GetComponent<Piece>();
        piece.name = tag.ToString();
        piece.transform.parent = transform;
        Objects.Add(piece);
        //piece.SetHealthElement(_uiController.GetHealthElement());

        return piece;
    }

    bool NewTarget(out Piece finalObjective, out Spot spawnSpot)
    {
        spawnSpot = _gameController.Map.Graph.RandomSpot(SpotTypes.EnemySpawn);
        finalObjective = _gameController.Towers.Random();

        return (finalObjective != null && spawnSpot != null);
    }

    int CountActiveEnemies()
    {
        int count = 0;

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }

    public void Remove(Piece enemy)
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
            Piece x = Objects[i];
            if (x.Alive)
            {
                StartCoroutine(x.Die());
            }
        }
    }
}
