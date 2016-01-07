using UnityEngine;
using System.Collections.Generic;

public class EdgeManager : MonoBehaviour
{
    public GameObject EdgePrefab;
    public List<Edge> Objects;

    public Edge CreateEdge(Spot a, Spot b)
    {
        if (AreNeighbors(a, b))
        {
            return null;
        }

        Edge obj = Instantiate(EdgePrefab).GetComponent<Edge>();
        obj.name = "Edge";;
        obj.transform.parent = transform;
        obj.SetNodes(a, b);
        obj.A.AddEdge(obj);
        obj.B.AddEdge(obj);
        Objects.Add(obj);

        return obj;
    }

    public void Remove(Edge edge)
    {
        Objects.Remove(edge);
        edge.A.RemoveEdge(edge);
        edge.B.RemoveEdge(edge);
        Destroy(edge.gameObject);
    }

    public void Remove(Spot contains)
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].HasNode(contains))
            {
                Remove(Objects[i]);
            }
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Remove(Objects[i]);
        }
    }

    public bool Find(Vector2 pos, out Edge hit)
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

    public Edge Find(Vector2 pos)
    {
        Edge hit = null;

        Find(pos, out hit);

        return hit;
    }

    public Edge Find(Spot a, Spot b, out Edge hit)
    {
        hit = null;
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].Neighbor(a) == b)
            {
                hit = Objects[i];
                break;
            }
        }

        return hit;
    }

    public bool AreNeighbors(Spot a, Spot b)
    {
        Edge hit;
        return Find(a, b, out hit);
    }
}
