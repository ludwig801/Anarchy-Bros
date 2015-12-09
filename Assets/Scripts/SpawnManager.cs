using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;

        public Transform EnemyHolder;
        public GameObject EnemyPrefab;
        public float SpawnTime;
        public int MaxEnemyCount;
        public List<Transform> SpawnPoints;

        float _deltaTime;
        
        int EnemyCount { get { return EnemyHolder.transform.childCount; } }   
        bool CanSpawnEnemy { get { return EnemyCount < MaxEnemyCount; } }

        void Start()
        {
            Instance = this;

            GetSpawnPoints();
        }

        void Update()
        {
            if (GameManager.Instance.IsPlay)
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
            int rand = Random.Range(0, int.MaxValue);
            rand %= SpawnPoints.Count;

            Enemy e = (Instantiate(EnemyPrefab) as GameObject).GetComponent<Enemy>();
            e.LocalObjective = SpawnPoints[rand].position;

            rand = Random.Range(0, int.MaxValue);
            List<PlayerSpot> spots = NodeManager.Instance.PlayerSpots;
            rand %= spots.Count;
            int i = 0;
            int iter = 0;
            while (rand >= 0)
            {
                if (spots[i].Occupied)
                {
                    rand--;
                    if (rand < 0)
                    {
                        break;
                    }
                }

                i++;
                i %= spots.Count;

                iter++;
                if (iter > 10000)
                {
                    Debug.Log("max iter");
                    break;               
                }
            }

            e.Objective = spots[i].transform.position;
            e.transform.parent = EnemyHolder;
        }

        public void ReEvaluate()
        {
            GetSpawnPoints();
        }

        void GetSpawnPoints()
        {
            if (SpawnPoints == null)
            {
                SpawnPoints = new List<Transform>();
            }
            else
            {
                SpawnPoints.Clear();
            }

            Transform spawnPoints = GameObject.FindGameObjectWithTag("Spawn Points").transform;

            for (int i = 0; i < spawnPoints.childCount; i++)
            {
                SpawnPoints.Add(spawnPoints.GetChild(i));
            }
        }
    }
}
