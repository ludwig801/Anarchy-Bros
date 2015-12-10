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
        public int MaxEnemyCount;
        public List<Node> EnemySpawns;

        float _deltaTime;      
        int EnemyCount { get { return EnemiesObj.transform.childCount; } }   
        bool CanSpawnEnemy { get { return EnemyCount < MaxEnemyCount; } }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GetSpawnSpots();
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

        public void Spawn()
        {
            int randSpawnSpot = Random.Range(0, EnemySpawns.Count);
            int randTowerSpot = Random.Range(0, TowerManager.Instance.Towers.Count);

            int count = 0, iter = 0, i = 0;

            while (count <= randTowerSpot && iter < GraphManager.Instance.Nodes.Count * 2)
            {
                Node n = GraphManager.Instance.Nodes[i];
                if (n.Type == Node.NodeType.TowerSpot && n.Occupied)
                {
                    count++;
                    if (count > randTowerSpot)
                    {
                        break;
                    }
                }

                iter++;
                i++;
                i %= GraphManager.Instance.Nodes.Count;
            }

            GameObject instance = Instantiate(EnemyPrefab);
            instance.transform.parent = EnemiesObj;

            Enemy e = instance.GetComponent<Enemy>();
            e.LocalObjective = EnemySpawns[randSpawnSpot].transform.position;
            e.Objective = GraphManager.Instance.Nodes[i].transform.position;
        }

        public void ReEvaluate()
        {
            GetSpawnSpots();
        }

        void GetSpawnSpots()
        {
            if (EnemySpawns == null)
            {
                EnemySpawns = new List<Node>();
            }
            else
            {
                EnemySpawns.Clear();
            }

            for (int i = 0; i < SpawnSpotsObj.childCount; i++)
            {
                EnemySpawns.Add(SpawnSpotsObj.GetChild(i).GetComponent<Node>());
            }
        }
    }
}
