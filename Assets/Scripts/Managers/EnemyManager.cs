using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        public Transform EnemiesObj, SpawnSpotsObj;
        public GameObject EnemyPrefab;
        public float SpawnTime;
        public int MaxEnemyCount, ActiveEnemies;
        public List<Spot> EnemySpawns;
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
            GetSpots();
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

        void GetSpots()
        {
            if (EnemySpawns == null)
            {
                EnemySpawns = new List<Spot>();
            }
            else
            {
                EnemySpawns.Clear();
            }

            for (int i = 0; i < SpawnSpotsObj.childCount; i++)
            {
                EnemySpawns.Add(SpawnSpotsObj.GetChild(i).GetComponent<Spot>());
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

        bool GenerateObjective(out Spot objective, out Spot localObjective)
        {
            if (EnemySpawns.Count <= 0)
            {
                objective = null;
                localObjective = null;
                return false;
            }

            int randSpawnSpot = Random.Range(0, EnemySpawns.Count);
            int randTowerSpot = Random.Range(0, TowerManager.Instance.Towers.Count);

            int count = 0, iter = 0, i = 0;

            while (count <= randTowerSpot && iter < GraphManager.Instance.Spots.Count * 2)
            {
                Spot n = GraphManager.Instance.Spots[i];
                if (n.Type == Spot.NodeType.TowerSpot && n.Occupied)
                {
                    count++;
                    if (count > randTowerSpot)
                    {
                        break;
                    }
                }

                iter++;
                i++;
                i %= GraphManager.Instance.Spots.Count;
            }

            objective = GraphManager.Instance.Spots[i];
            localObjective = EnemySpawns[randSpawnSpot];

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

        public void ReEvaluate()
        {
            GetSpots();
        }
    }
}
