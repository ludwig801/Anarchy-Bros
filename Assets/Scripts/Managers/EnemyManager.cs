using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        public Transform EnemiesObj;
        public GameObject EnemyPrefab;
        public float SpawnTime;
        public int MaxEnemyCount, ActiveEnemies;
        public bool TargetsAreRandom;
        public List<Enemy> Enemies;

        float _deltaTime;
        bool CanSpawnEnemy { get { return ActiveEnemies < MaxEnemyCount; } }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            ActiveEnemies = 0;
        }

        void Update()
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                _deltaTime += Time.deltaTime;

                if (_deltaTime > SpawnTime && CanSpawnEnemy)
                {
                    Spawn();
                    _deltaTime = 0f;
                }
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
            spawnSpot = MapManager.Instance.GetRandomSpot(SpotTypes.EnemySpot);
            finalObjective = TowerManager.Instance.GetRandomTower();

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
            instance.transform.parent = EnemiesObj;

            Enemy e = instance.GetComponent<Enemy>();
            Enemies.Add(e);

            return e;
        }

        public bool GetNewObjective(out Tower newObjective)
        {
            newObjective = TowerManager.Instance.GetRandomTower();
            return (newObjective != null);
        }

        public Enemy GetNearestEnemy(Tower t)
        {
            float minDist = float.MaxValue;
            Enemy best = null;

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].IsAlive)
                {
                    float d = MapManager.Instance.DistanceBetween(t, Enemies[i]);
                    if (d < minDist)
                    {
                        minDist = d;
                        best = Enemies[i];
                    }
                }
            }

            return best;
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
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Enemies[i].GetComponent<EditBehavior>().enabled = false;
                    }
                    break;
            }
        }
    }
}
