using UnityEngine;
using System.Collections.Generic;

public class SpotManager : MonoBehaviour
{
    public GameObject ConnectionPrefab, TowerSpotPrefab, EnemySpawnPrefab;
    public List<Spot> Objects;
    public int SpotCount { get { return Objects.Count; } }

    public Spot Create(Vector2 worldPos, SpotTypes type)
    {
        GameObject obj;

        switch (type)
        {
            case SpotTypes.Connection:
                obj = Instantiate(ConnectionPrefab);
                obj.name = "Connection";
                break;

            case SpotTypes.TowerSpot:
                obj = Instantiate(TowerSpotPrefab);
                obj.name = "Tower Spot";
                break;

            case SpotTypes.EnemySpawn:
                obj = Instantiate(EnemySpawnPrefab);
                obj.name = "Enemy Spawn";
                break;

            default:
                obj = Instantiate(ConnectionPrefab);
                obj.name = "<Undefined>";
                break;
        }

        Spot spot = obj.GetComponent<Spot>();
        spot.transform.position = worldPos;
        spot.Type = type;
        spot.transform.parent = transform;
        Objects.Add(spot);

        return spot;
    }

    public Spot ChangeType(Spot spot, SpotTypes toType)
    {
        Spot to = Create(spot.transform.position, toType);
        for (int i = spot.Edges.Count - 1; i >= 0; i--)
        {
            Edge e = spot.Edges[i];
            e.ReplaceNeighbor(spot, to);
            to.AddEdge(e);
            spot.RemoveEdge(spot.Edges[i]);
        }

        Remove(spot);

        return to;
    }

    public void Remove(Spot s)
    {
        Objects.Remove(s);
        Destroy(s.gameObject);
    }

    public void RemoveAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Remove(Objects[i]);
        }
    }

    public bool Find(Vector2 pos, out Spot hit)
    {
        hit = null;

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Collider.OverlapPoint(pos))
            {
                hit = Objects[i];
                break;
            }
        }

        return (hit != null);
    }

    public Spot Find(Vector2 pos)
    {
        Spot hit;
        Find(pos, out hit);
        return hit;
    }

    public Spot Random(SpotTypes type)
    {
        int rand = UnityEngine.Random.Range(0, TypeCount(type));

        Spot s = null;

        if (rand < 0)
        {
            return s;
        }

        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Type == type)
            {
                if (rand <= 0)
                {
                    s = Objects[i];
                    break;
                }
                rand--;
            }
        }

        return s;
    }

    int TypeCount(SpotTypes type)
    {
        int count = 0;
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Type == type)
            {
                count++;
            }
        }

        return count;
    }
}
