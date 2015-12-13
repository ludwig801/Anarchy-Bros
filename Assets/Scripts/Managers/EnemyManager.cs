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
            Spot objective;
            Spot localObjective;

            if (GenerateObjective(out objective, out localObjective))
            {
                Enemy e = GetEnemy();
                e.gameObject.SetActive(true);
                e.transform.position = localObjective.transform.position;
                e.LocalObjective = localObjective;
                e.Objective = objective;
                ActiveEnemies++;
            }
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

        bool GenerateObjective(out Spot finalObjective, out Spot localObjective)
        {
            Spot enemySpot = GraphManager.Instance.GetRandomSpot(SpotTypes.EnemySpawn);
            Spot towerSpot = GraphManager.Instance.GetRandomSpot(SpotTypes.TowerSpot);

            localObjective = enemySpot;
            finalObjective = towerSpot;

            if (enemySpot == null || towerSpot == null)
            {
                return false;
            }

            return true;
        }

        public bool GenerateObjective(Spot currentSpot, out Spot newObjective)
        {
            Spot localObjective;
            return GenerateObjective(out newObjective, out localObjective);
        }

        public void OnEnemyKill()
        {
            ActiveEnemies--;
        }

        void DestroyAllEnemies()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Destroy(Enemies[i].gameObject);
            }

            Enemies.Clear();
        }

        public void ReEvaluate()
        {
            switch (GameManager.Instance.CurrentState)
            {
                case GameStates.Edit:
                    DestroyAllEnemies();
                    break;
            }
        }
    }
}
