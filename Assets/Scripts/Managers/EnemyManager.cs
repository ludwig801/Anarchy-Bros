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
        public List<Enemy> Enemies;

        float _deltaTime;
        bool CanSpawnEnemy { get { return ActiveEnemies < MaxEnemyCount; } }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Enemies = new List<Enemy>();
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
                e.CurrentSpot = spawnSpot;
                e.CurrentEdge = null;
                e.MoveTo = e.CurrentSpot;
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
            spawnSpot = GraphManager.Instance.GetRandomSpot(SpotTypes.EnemySpawn);
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

        public Tower GetNewObjective()
        {
            return TowerManager.Instance.GetRandomTower();
        }

        public void OnEnemyKill()
        {
            ActiveEnemies--;
        }

        public void ReEvaluate()
        {
            switch (GameManager.Instance.CurrentState)
            {
                case GameStates.Edit:
                    DestroyAllEnemies();
                    break;

                case GameStates.Place:
                    DestroyAllEnemies();
                    break;
            }
        }
    }
}
