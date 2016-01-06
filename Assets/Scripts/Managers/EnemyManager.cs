using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        public GameObject EnemyPrefab;
        public float SpawnTime;
        public int MaxEnemyCount, ActiveEnemies;
        public bool TargetsAreRandom;
        public List<Enemy> Enemies;

        GameManager _gameManager;
        TowerManager _towerManager;
        MapManager _mapManager;
        float _deltaTime;
        bool CanSpawnEnemy { get { return ActiveEnemies < MaxEnemyCount; } }

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

            if (_deltaTime > SpawnTime && CanSpawnEnemy)
            {
                Spawn();
                _deltaTime = 0;
            }
        }

        void Spawn()
        {
            Tower objective;
            Spot spawnSpot;

            if (GenerateObjective(out objective, out spawnSpot))
            {
                Enemy e = GetEnemy();
                e.transform.position = spawnSpot.transform.position;
                e.name = "Enemy";
                e.Spot = spawnSpot;
                e.Edge = null;
                e.MoveTo = e.Spot;
                e.Objective = objective;
                e.gameObject.SetActive(true);
                e.Reborn();
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

        bool GenerateObjective(out Tower finalObjective, out Spot spawnSpot)
        {
            spawnSpot = _mapManager.RandomSpot(SpotTypes.EnemySpot);
            finalObjective = _towerManager.GetRandomTower();

            return (finalObjective != null && spawnSpot != null);
        }

        Enemy GetEnemy()
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

            Enemy e = instance.GetComponent<Enemy>();
            Enemies.Add(e);

            return e;
        }

        public bool GetNewObjective(out Tower newObjective)
        {
            newObjective = _towerManager.GetRandomTower();
            return (newObjective != null);
        }

        public Enemy GetNearestEnemy(Tower t)
        {
            float minDist = float.MaxValue;
            Enemy closest = null;

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].IsAlive())
                {
                    float d = _mapManager.DistanceBetween(t, Enemies[i]);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = Enemies[i];
                    }
                }
            }

            return closest;
        }

        public void OnEnemyKill()
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
}
