using UnityEngine;
using System.Collections.Generic;
using System;

public class GraphManager : MonoBehaviour
{
    public Transform ObjSpots, ObjEdges;
    public GameObject ConnectionPrefab, TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab;
    public float EdgeHitTolerance;
    public List<MapSpot> Spots;
    public List<MapEdge> Edges;
    public int SpotCount { get { return Spots.Count; } }

    public MapSpot CreateSpot(Vector2 worldPos, SpotTypes type)
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

        MapSpot spot = obj.GetComponent<MapSpot>();
        spot.transform.position = worldPos;
        spot.Type = type;
        spot.transform.parent = ObjSpots;
        Spots.Add(spot);

        return spot;
    }

    public MapEdge CreateEdge(MapSpot a, MapSpot b)
    {
        if (FindEdge(a, b)) return null;

        MapEdge obj = Instantiate(EdgePrefab).GetComponent<MapEdge>();
        obj.name = "Edge"; ;
        obj.transform.parent = ObjEdges;
        obj.A = a;
        obj.B = b;
        obj.A.Edges.Add(obj);
        obj.B.Edges.Add(obj);
        Edges.Add(obj);

        return obj;
    }

    public MapSpot ChangeSpotType(MapSpot spot, SpotTypes newType)
    {
        MapSpot to = CreateSpot(spot.transform.position, newType);
        for (int i = spot.Edges.Count - 1; i >= 0; i--)
        {
            MapEdge edge = spot.Edges[i];
            edge.ReplaceNeighbor(spot, to);
            to.Edges.Add(edge);
            spot.Edges.Remove(edge);
        }

        RemoveSpot(spot);

        return to;
    }

    public void RemovePieceFromSpot(PieceBehavior piece)
    {
        for (int i = 0; i < Spots.Count; i++)
        {
            if (Spots[i].Piece == piece)
            {
                Spots[i].Piece = null;
                break;
            }
        }
    }

    public void RemoveEdge(MapEdge edge)
    {
        Edges.Remove(edge);
        edge.A.Edges.Remove(edge);
        edge.B.Edges.Remove(edge);
        Destroy(edge.gameObject);
    }

    public void RemoveSpot(MapSpot spot)
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

    public bool FindSpotNear(Vector2 pos, out MapSpot hit, SpotTypes type = SpotTypes.All)
    {
        hit = null;

        for (int i = 0; i < Spots.Count; i++)
        {
            MapSpot spot = Spots[i];
            if (Tools2D.SamePos(spot.transform.position, pos, spot.transform.localScale.x) && (type == SpotTypes.All || spot.Type == type))
            {
                hit = spot;
                break;
            }
        }

        return (hit != null);
    }

    public bool FindSpotExact(Vector2 pos, out MapSpot hit)
    {
        hit = null;
        for (int i = 0; i < Spots.Count; i++)
        {
            MapSpot spot = Spots[i];
            if (Tools2D.SamePos(spot.transform.position, pos, 0.01f))
            {
                hit = spot;
                break;
            }
        }
        return (hit != null);
    }

    public bool FindEdge(MapSpot a, MapSpot b)
    {
        MapEdge hit = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            MapEdge x = Edges[i];
            if (x.Neighbor(a) == b)
            {
                hit = x;
                break;
            }
        }

        return (hit != null);
    }

    public bool FindEdge(Vector2 pos, out MapEdge hit)
    {
        hit = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            MapEdge edge = Edges[i];
            Vector2 posA = edge.A.transform.position;
            Vector2 posB = edge.B.transform.position;

            Vector2 vAB = (posB - posA).normalized;
            Vector2 vAP = (pos - posA).normalized;
            Vector3 vBP = (pos - posB);
            if (Mathf.Abs(Vector2.Dot(vAP, vAB) - 1f) < 0.01f && Vector2.Dot(vBP, vAB) < 0)
            {
                hit = edge;
                break;
            }
        }

        return (hit != null);
    }

    public bool GetRandomSpot(SpotTypes type, out MapSpot hit)
    {
        hit = null;
        int rand = UnityEngine.Random.Range(0, TypeCount(type));

        if (rand < 0) return false;

        for (int i = 0; i < Spots.Count; i++)
        {
            if (Spots[i].Type == type)
            {
                if (rand <= 0)
                {
                    hit = Spots[i];
                    break;
                }
                rand--;
            }
        }

        return (hit != null);
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
