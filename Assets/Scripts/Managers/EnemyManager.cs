using UnityEngine;
using System.Collections.Generic;
using Enums;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject EnemyPrefab;
    public float SpawnTime;
    public int MaxEnemyCount, ActiveEnemies;
    public List<Piece> Enemies;
    public bool CanSpawnEnemy { get { return ActiveEnemies < MaxEnemyCount; } }

    GameManager _gameManager;
    TowerManager _towerManager;
    MapManager _mapManager;
    float _deltaTime;  

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _towerManager = TowerManager.Instance;
        _mapManager = MapManager.Instance;

        ActiveEnemies = 0;
        _deltaTime = 0;
    }

    void Update()
    {
        if (!_gameManager.IsCurrentState(GameStates.Play))
        {
            _deltaTime = 0;
            return;
        }

        _deltaTime += Time.deltaTime;

        ActiveEnemies = 0;
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].Alive)
            {
                ActiveEnemies++;
            }
        }

        if (_deltaTime > SpawnTime && CanSpawnEnemy)
        {
            Spawn();
            _deltaTime = 0;
        }
    }

    void Spawn()
    {
        Piece objective;
        Spot spawnSpot;

        if (GenerateObjective(out objective, out spawnSpot))
        {
            Piece enemy = GetEnemy();
            enemy.transform.position = spawnSpot.transform.position;
            enemy.name = "Enemy";
            enemy.MoveTo = spawnSpot.transform;
            enemy.Target = objective.transform;
            enemy.gameObject.SetActive(true);
            enemy.Live();
            ActiveEnemies++;
        }
    }

    void DestroyAllEnemies()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            Destroy(Enemies[i].gameObject);
        }

        Enemies.Clear();
        ActiveEnemies = 0;
    }

    bool GenerateObjective(out Piece finalObjective, out Spot spawnSpot)
    {
        spawnSpot = _mapManager.RandomSpot(SpotTypes.EnemySpot);
        finalObjective = _towerManager.GetRandomTower();

        return (finalObjective != null && spawnSpot != null);
    }

    Piece GetEnemy()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (!Enemies[i].gameObject.activeSelf)
            {
                return Enemies[i];
            }
        }

        GameObject instance = Instantiate(EnemyPrefab);
        instance.transform.parent = transform;

        Piece enemy = instance.GetComponent<Piece>();
        Enemies.Add(enemy);
        enemy.SetHealthElement(UIManager.Instance.GetHealthElement());

        return enemy;
    }

    public bool GetNewObjective(out Piece newObjective)
    {
        newObjective = _towerManager.GetRandomTower();
        return (newObjective != null);
    }

    public void OnEnemyDie()
    {
        ActiveEnemies--;
    }

    public void OnGameStateChanged(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Edit:
                DestroyAllEnemies();
                break;

            case GameStates.Place:
                DestroyAllEnemies();
                break;

            case GameStates.Play:
                break;
        }
    }
}