using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform Objective;
    public GameObject EnemyPrefab;
    public float SpawnTime;

    float _deltaTime;

    void Update()
    {
        _deltaTime += Time.deltaTime;

        if (_deltaTime > SpawnTime)
        {
            Enemy e = (Instantiate(EnemyPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Enemy>();
            e.Objective = Objective;

            _deltaTime = 0f;
        }
    }
}
