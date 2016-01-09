using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Graph))]
public class GraphLogic : MonoBehaviour
{
    Graph _graph;
    float[,] _distances;

    void Start()
    {
        _graph = GetComponent<Graph>();
    }

    public void ReCalculate()
    {
        int spotCount = _graph.SpotCount;

        _distances = new float[spotCount, spotCount];
        List<GraphSpot> graph = new List<GraphSpot>();

        for (int i = 0; i < spotCount; i++)
        {
            Spot n = _graph.Spots[i];
            n.Index = i;
            GraphSpot gSpot = new GraphSpot(n.transform.position, float.MaxValue);
            graph.Add(gSpot);
        }

        for (int i = 0; i < graph.Count; i++)
        {
            ReCalculateDistances(i, graph);
        }

        graph.Clear();
    }

    void ReCalculateDistances(int sourceIndex, List<GraphSpot> graph)
    {
        List<GraphSpot> unvisited = new List<GraphSpot>();

        for (int i = 0; i < graph.Count; i++)
        {
            unvisited.Add(graph[i]);
            graph[i].Dist = int.MaxValue;
        }

        for (int i = 0; i < _graph.SpotCount; i++)
        {
            Spot spot = _graph.Spots[i];

            for (int j = 0; j < spot.Edges.Count; j++)
            {
                Spot neighbor = spot.GetNeighbor(j);
                for (int w = 0; w < unvisited.Count; w++)
                {
                    if (Tools2D.SamePos(unvisited[w].position, neighbor.transform.position))
                    {
                        unvisited[i].Neighbors.Add(unvisited[w]);
                    }
                }
            }
        }

        unvisited[sourceIndex].Dist = 0;

        while (unvisited.Count > 0)
        {
            GraphSpot spot = unvisited[GetNextBest(unvisited)];
            unvisited.Remove(spot);

            for (int i = 0; i < spot.Neighbors.Count; i++)
            {
                GraphSpot neighbor = spot.Neighbors[i];
                if (unvisited.Contains(neighbor))
                {
                    float d = spot.Dist + Vector2.Distance(neighbor.position, spot.position);
                    if (d <= neighbor.Dist)
                    {
                        neighbor.Dist = d;
                    }
                }
            }
        }

        for (int i = 0; i < graph.Count; i++)
        {
            _distances[sourceIndex, i] = graph[i].Dist;
        }
    }

    public bool NextStep(MoveBehavior source, out Spot step)
    {
        step = null;

        if (source.HasCurrentSpot)
        {
            step = NextStep(source.CurrentSpot, source.Target);
        }
        else if (source.HasCurrentEdge)
        {
            step = NextStep(source, source.Target);
        }

        return (step != null);
    }

    public bool NextStep(MoveBehavior source, MoveBehavior target, out Spot step)
    {
        step = null;

        if (source.HasCurrentSpot)
        {
            if (target.HasCurrentSpot)
            {
                step = NextStep(source.CurrentSpot, target.CurrentSpot);
            }
            else if (target.HasCurrentEdge)
            {
                step = NextStep(source.CurrentSpot, target);
            }
        }
        else if (source.HasCurrentEdge)
        {
            if (target.HasCurrentSpot)
            {
                step = NextStep(source, target.CurrentSpot);
            }
            else if (target.HasCurrentEdge)
            {
                step = NextStep(source, target);
            }
        }

        return (step != null);
    }

    Spot NextStep(Spot source, Spot target)
    {
        if (source == target)
        {
            return target;
        }

        float minDist = int.MaxValue;
        Spot best = null;

        for (int i = 0; i < source.Edges.Count; i++)
        {
            Spot neighbor = source.Edges[i].Neighbor(source);
            float d = _distances[neighbor.Index, target.Index] + DistanceBetween(source, neighbor);

            if (d < minDist)
            {
                minDist = d;
                best = neighbor;
            }
        }

        return best;
    }

    Spot NextStep(Spot source, MoveBehavior target)
    {
        if (target.CurrentEdge.HasSpot(source))
        {
            if (source == target.Step)
            {
                return target.CurrentEdge.Neighbor(target.Step);
            }
            else
            {
                return target.Step;
            }   
        }

        float minDist = float.MaxValue;
        Spot best = null;

        int tA = target.CurrentEdge.A.Index;
        int tB = target.CurrentEdge.B.Index;

        for (int i = 0; i < source.Edges.Count; i++)
        {
            Spot neighbor = source.Edges[i].Neighbor(source);
            int sourceIndex = neighbor.Index;
            float d = DistanceBetween(source, neighbor);
            float dA = _distances[sourceIndex, tA];
            float dB = _distances[sourceIndex, tB];
            d += (dA < dB) ? dA : dB;

            if (d < minDist)
            {
                minDist = d;
                best = neighbor;
            }
        }

        return best;
    }

    Spot NextStep(MoveBehavior source, Spot target)
    {
        if (target.Edges.Contains(source.CurrentEdge))
        {
            return target;
        }

        Spot sA = source.CurrentEdge.A;
        Spot sB = source.CurrentEdge.B;

        float distA = Vector2.Distance(source.transform.position, sA.transform.position) + DistanceBetween(sA, target);
        float distB = Vector2.Distance(source.transform.position, sB.transform.position) + DistanceBetween(sB, target);
        return (distA < distB) ? sA : sB;
    }

    Spot NextStep(MoveBehavior source, MoveBehavior target)
    {
        if (source.CurrentEdge == target.CurrentEdge)
        {
            Edge edge = target.CurrentEdge;
            Spot spot = target.Step == edge.A ? edge.A : edge.B;
            Vector2 spotPos = spot.transform.position;            
            return Tools2D.Distance(target.transform.position, spotPos) < Tools2D.Distance(source.transform.position, spotPos) ? spot : edge.Neighbor(spot);
        }

        Spot sA = source.CurrentEdge.A;
        Spot sB = source.CurrentEdge.B;
        Spot targetSpot = target.Step;

        float distA = Tools2D.Distance(source.transform.position, sA.transform.position) + DistanceBetween(sA, targetSpot);
        float distB = Tools2D.Distance(source.transform.position, sB.transform.position) + DistanceBetween(sB, targetSpot);

        return distA < distB ? sA : sB;
    }

    public float DistanceBetween(MoveBehavior source, MoveBehavior target)
    {
        if (source.HasCurrentSpot)
        {
            if (target.HasCurrentSpot)
            {
                return DistanceBetween(source.CurrentSpot, target.CurrentSpot);
            }
            else if (target.HasCurrentEdge)
            {
                return DistanceBetween(source.CurrentSpot, target);
            }
        }
        else if (source.HasCurrentEdge)
        {
            if (target.HasCurrentSpot)
            {
                return DistanceBetween(target.CurrentSpot, source);
            }
            else if (target.HasCurrentEdge)
            {
                return Mathf.Min(DistanceBetween(source.CurrentEdge.A, target), DistanceBetween(source.CurrentEdge.B, target));
            }
        }

        return float.MaxValue;
    }

    float DistanceBetween(Spot spot, Spot objective)
    {
        return _distances[spot.Index, objective.Index];
    }

    float DistanceBetween(Spot source, MoveBehavior target)
    {
        return Mathf.Min(DistanceBetween(source, target.CurrentEdge.A) + Tools2D.Distance(target.transform.position, target.CurrentEdge.A.transform.position),
            DistanceBetween(source, target.CurrentEdge.B) + Tools2D.Distance(target.transform.position, target.CurrentEdge.B.transform.position));
    }

    int GetNextBest(List<GraphSpot> graph)
    {
        float min = float.MaxValue;
        int index = 0;

        for (int i = 0; i < graph.Count; i++)
        {
            if (graph[i].Dist <= min)
            {
                min = graph[i].Dist;
                index = i;
            }
        }

        return index;
    }

    class GraphSpot
    {
        internal Vector2 position;
        internal float Dist;

        internal List<GraphSpot> Neighbors;

        internal GraphSpot(Vector2 pos, float dist)
        {
            position = pos;
            Dist = dist;

            Neighbors = new List<GraphSpot>();
        }
    }
}
