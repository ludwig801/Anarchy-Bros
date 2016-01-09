using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    public Transform ObjSpots, ObjEdges;
    public GameObject ConnectionPrefab, TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab;
    public float SpotHitTolerance, EdgeHitTolerance;
    public List<Spot> Spots;
    public List<Edge> Edges;
    public int SpotCount { get { return Spots.Count; } }

    public Spot CreateSpot(Vector2 worldPos, SpotTypes type)
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
        spot.transform.parent = ObjSpots;
        Spots.Add(spot);

        return spot;
    }

    public Edge CreateEdge(Spot a, Spot b)
    {
        if (FindEdge(a, b)) return null;

        Edge obj = Instantiate(EdgePrefab).GetComponent<Edge>();
        obj.name = "Edge"; ;
        obj.transform.parent = ObjEdges;
        obj.A = a;
        obj.B = b;
        obj.A.AddEdge(obj);
        obj.B.AddEdge(obj);
        Edges.Add(obj);

        return obj;
    }

    public Spot ChangeSpotType(Spot spot, SpotTypes newType)
    {
        Spot to = CreateSpot(spot.transform.position, newType);
        for (int i = spot.Edges.Count - 1; i >= 0; i--)
        {
            Edge e = spot.Edges[i];
            e.ReplaceNeighbor(spot, to);
            to.AddEdge(e);
            spot.RemoveEdge(spot.Edges[i]);
        }

        RemoveSpot(spot);

        return to;
    }

    public void RemoveEdge(Edge edge)
    {
        Edges.Remove(edge);
        edge.A.RemoveEdge(edge);
        edge.B.RemoveEdge(edge);
        Destroy(edge.gameObject);
    }

    public void RemoveSpot(Spot spot)
    {
        for (int i = spot.Edges.Count - 1; i >= 0; i--)
        {
            RemoveEdge(spot.Edges[i]);
        }

        Spots.Remove(spot);
        Destroy(spot.gameObject);
    }

    public void RemoveAll()
    {
        for (int i = Spots.Count - 1; i >= 0; i--)
        {
            RemoveSpot(Spots[i]);
        }

        for (int i = Edges.Count - 1; i >= 0; i--)
        {
            RemoveEdge(Edges[i]);
        }

        Edges.Clear();
        Spots.Clear();
    }

    public bool FindSpot(Vector2 pos, out Spot hit, float tolerance)
    {
        return (hit = FindSpot(pos, tolerance)) != null;
    }

    public bool FindSpot(Vector2 pos, out Spot hit)
    {
        hit = FindSpot(pos);
        return hit != null;
    }

    public Spot FindSpot(Vector2 pos, float tolerance)
    {
        for (int i = 0; i < Spots.Count; i++)
        {
            Spot spot = Spots[i];
            if (Tools2D.SamePos(spot.transform.position, pos, tolerance))
            {
                return spot;
            }
        }
        return null;
    }

    public Spot FindSpot(Vector2 pos)
    {
        return FindSpot(pos, SpotHitTolerance);
    }

    public bool FindEdge(Spot a, Spot b)
    {
        Edge hit = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            Edge x = Edges[i];
            if (x.Neighbor(a) == b)
            {
                hit = x;
                break;
            }
        }

        return (hit != null);
    }

    public Edge FindEdge(Vector2 pos, float tolerance)
    {
        for (int i = 0; i < Edges.Count; i++)
        {
            Edge x = Edges[i];
            Vector2 posA = x.A.transform.position;
            Vector2 posB = x.B.transform.position;

            Vector2 vAB = (posB - posA).normalized;
            Vector2 vAP = (pos - posA).normalized;
            Vector3 vBP = (pos - posB);
            if (Mathf.Abs(Vector2.Dot(vAP, vAB) - 1f) < tolerance && Vector2.Dot(vBP, vAB) < 0)
            {
                return x;
            }
        }

        return null;
    }

    public Edge FindEdge(Vector2 pos)
    {
        return FindEdge(pos, EdgeHitTolerance);
    }

    public bool FindEdge(Vector2 pos, out Edge hit)
    {
        return (hit = FindEdge(pos)) != null;
    }

    public Spot GetRandomSpot(SpotTypes type)
    {
        int rand = Random.Range(0, TypeCount(type));

        Spot s = null;

        if (rand < 0)
        {
            return s;
        }

        for (int i = 0; i < Spots.Count; i++)
        {
            if (Spots[i].Type == type)
            {
                if (rand <= 0)
                {
                    s = Spots[i];
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
        for (int i = 0; i < Spots.Count; i++)
        {
            if (Spots[i].Type == type)
            {
                count++;
            }
        }

        return count;
    }
}
