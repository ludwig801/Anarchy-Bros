using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class SpawnManager : MonoBehaviour
    {
        public Transform Objective, EnemyHolder;
        public GameObject EnemyPrefab;
        public float SpawnTime;
        public int MaxEnemyCount;

        int EnemyCount
        {
            get
            {
                return EnemyHolder.transform.childCount;
            }
        }

        bool CanSpawnEnemy
        {
            get
            {
                return EnemyCount < MaxEnemyCount;
            }
        }

        float _deltaTime;
        List<Transform> _spawnPoints;

        void Start()
        {
            GetSpawnPoints();
        }

        void Update()
        {
            _deltaTime += Time.deltaTime;

            if (_deltaTime > SpawnTime && CanSpawnEnemy)
            {
                Spawn();
                _deltaTime = 0f;
            }
        }

        public void Spawn()
        {
            int rand = Random.Range(0, int.MaxValue);
            rand %= _spawnPoints.Count;

            Enemy e = (Instantiate(EnemyPrefab, _spawnPoints[rand].position, Quaternion.identity) as GameObject).GetComponent<Enemy>();
            e.Objective = Objective;
            e.transform.parent = EnemyHolder;
        }

        void GetSpawnPoints()
        {
            if (_spawnPoints == null)
            {
                _spawnPoints = new List<Transform>();
            }
            else
            {
                _spawnPoints.Clear();
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                _spawnPoints.Add(transform.GetChild(i));
            }
        }
    }
}
