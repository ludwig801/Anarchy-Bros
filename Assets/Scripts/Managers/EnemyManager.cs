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
            int randSpawnSpot = Random.Range(0, int.MaxValue);
            randSpawnSpot = randSpawnSpot % EnemySpawns.Count;

            List<Node> pawnSpots = GraphManager.Instance.TowerSpots;
            int randPawnSpot = Random.Range(0, int.MaxValue);
            randPawnSpot %= pawnSpots.Count;

            //Debug.Log(randSpawnSpot + " | " + randPawnSpot); 

            int i = 0;
            int iter = 0;
            while (randPawnSpot >= 0)
            {
                if (pawnSpots[i].Occupied)
                {
                    randPawnSpot--;
                    if (randPawnSpot < 0)
                    {
                        break;
                    }
                }

                i++;
                i %= pawnSpots.Count;

                iter++;
                if (iter > 10000)
                {
                    Debug.Log("max iter");
                    break;               
                }
            }

            GameObject instance = Instantiate(EnemyPrefab);
            instance.transform.parent = EnemiesObj;

            Enemy e = instance.GetComponent<Enemy>();
            e.LocalObjective = EnemySpawns[randSpawnSpot].transform.position;
            e.Objective = pawnSpots[i].transform.position;
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
