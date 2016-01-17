using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GraphManager))]
public class GraphLogic : MonoBehaviour
{
    GraphManager Graph
    {
        get
        {
            if (_graph == null)
            {
                _graph = GetComponent<GraphManager>();
            }
            return _graph;
        }
    }

    GraphManager _graph;
    float[,] _distancesBetweenSpots;

    public bool StepToTarget(MoveBehavior source, out MapSpot step)
    {
        step = null;

        if (source.HasCurrentSpot)
        {
            step = StepToTarget(source.CurrentSpot, source.Target);
        }
        else if (source.HasCurrentEdge)
        {
            step = StepToTarget(source, source.Target);
        }

        return (step != null);
    }

    public bool ProvideTarget(MoveBehavior source, MoveBehavior target, out MapSpot step)
    {
        step = null;

        if (target.HasCurrentSpot)
        {
            step = target.CurrentSpot;
        }
        else if (target.HasCurrentEdge)
        {
            if (source.HasCurrentSpot)
            {
                step = ProvideTarget(source.CurrentSpot, target);
            }
            else if (source.HasCurrentEdge)
            {
                step = ProvideTarget(source, target);
            }
        }

        return (step != null);
    }

    MapSpot ProvideTarget(MapSpot source, MoveBehavior target)
    {
        MapEdge targetEdge = target.CurrentEdge;
        if (source.Edges.Contains(targetEdge))
        {
            return (target.Step == source) ? targetEdge.Neighbor(target.Step) : target.Step;
        }

        float distA = DistanceBetween(source, target.CurrentEdge.A) + DistanceBetween(target.CurrentEdge.A, target);
        float distB = DistanceBetween(source, target.CurrentEdge.B) + DistanceBetween(target.CurrentEdge.B, target);

        return (distA < distB) ? targetEdge.A : targetEdge.B;
    }

    MapSpot ProvideTarget(MoveBehavior source, MoveBehavior target)
    {
        MapEdge sourceEdge = source.CurrentEdge;
        MapEdge targetEdge = target.CurrentEdge;
        if (sourceEdge == targetEdge)
        {
            MapSpot targetSpot = target.Step == targetEdge.A ? targetEdge.A : targetEdge.B;
            Vector2 spotPos = targetSpot.transform.position;
            return Tools2D.Distance(target.transform.position, spotPos) < Tools2D.Distance(source.transform.position, spotPos) ? targetSpot : targetEdge.Neighbor(targetSpot);
        }

        MapSpot bestSourceSpot = DistanceBetween(sourceEdge.A, target) < DistanceBetween(sourceEdge.B, target) ? sourceEdge.A : sourceEdge.B;

        return DistanceBetween(bestSourceSpot, targetEdge.A) < DistanceBetween(bestSourceSpot, targetEdge.B) ? targetEdge.A : targetEdge.B;
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

    public void ReCalculate()
    {
        int spotCount = Graph.SpotCount;

        _distancesBetweenSpots = new float[spotCount, spotCount];
        List<GraphSpot> graph = new List<GraphSpot>();

        for (int i = 0; i < spotCount; i++)
        {
            MapSpot n = Graph.Spots[i];
            n.Index = i;
            GraphSpot gSpot = new GraphSpot(n.transform.position, float.MaxValue);
            graph.Add(gSpot);
        }

        for (int i = 0; i < graph.Count; i++)
        {
            ReCalculate(i, graph);
        }

        graph.Clear();
    }

    MapSpot StepToTarget(MapSpot source, MapSpot target)
    {
        MapSpot best = null;

        if (source != target)
        {
            float minDist = int.MaxValue;

            for (int i = 0; i < source.Edges.Count; i++)
            {
                MapSpot neighbor = source.Edges[i].Neighbor(source);
                float d = _distancesBetweenSpots[neighbor.Index, target.Index] + DistanceBetween(source, neighbor);

                if (d < minDist)
                {
                    minDist = d;
                    best = neighbor;
                }
            }
        }

        return best;
    }

    MapSpot StepToTarget(MapSpot source, MoveBehavior target)
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
        MapSpot best = null;

        int tA = target.CurrentEdge.A.Index;
        int tB = target.CurrentEdge.B.Index;

        for (int i = 0; i < source.Edges.Count; i++)
        {
            MapSpot neighbor = source.Edges[i].Neighbor(source);
            int sourceIndex = neighbor.Index;
            float d = DistanceBetween(source, neighbor);
            float dA = _distancesBetweenSpots[sourceIndex, tA];
            float dB = _distancesBetweenSpots[sourceIndex, tB];
            d += (dA < dB) ? dA : dB;

            if (d < minDist)
            {
                minDist = d;
                best = neighbor;
            }
        }

        return best;
    }

    MapSpot StepToTarget(MoveBehavior source, MapSpot target)
    {
        if (target.Edges.Contains(source.CurrentEdge))
        {
            return target;
        }

        MapSpot sA = source.CurrentEdge.A;
        MapSpot sB = source.CurrentEdge.B;

        float distA = Vector2.Distance(source.transform.position, sA.transform.position) + DistanceBetween(sA, target);
        float distB = Vector2.Distance(source.transform.position, sB.transform.position) + DistanceBetween(sB, target);
        return (distA < distB) ? sA : sB;
    }

    MapSpot StepToTarget(MoveBehavior source, MoveBehavior target)
    {
        MapEdge sourceEdge = source.CurrentEdge;
        MapEdge targetEdge = target.CurrentEdge;

        if (sourceEdge == targetEdge)
        {
            MapSpot spot = target.Step == targetEdge.A ? targetEdge.A : targetEdge.B;
            Vector2 spotPos = spot.transform.position;            
            return Tools2D.Distance(target.transform.position, spotPos) < Tools2D.Distance(source.transform.position, spotPos) ? spot : targetEdge.Neighbor(spot);
        }

        MapSpot targetSpot = target.Step;
        float distA = Tools2D.Distance(source.transform.position, sourceEdge.A.transform.position) + DistanceBetween(sourceEdge.A, targetSpot);
        float distB = Tools2D.Distance(source.transform.position, sourceEdge.B.transform.position) + DistanceBetween(sourceEdge.B, targetSpot);

        return distA < distB ? sourceEdge.A : sourceEdge.B;
    }

    float DistanceBetween(MapSpot source, MapSpot target)
    {
        return _distancesBetweenSpots[source.Index, target.Index];
    }

    float DistanceBetween(MapSpot source, MoveBehavior target)
    {
        return Mathf.Min(DistanceBetween(source, target.CurrentEdge.A) + Tools2D.Distance(target.transform.position, target.CurrentEdge.A.transform.position),
            DistanceBetween(source, target.CurrentEdge.B) + Tools2D.Distance(target.transform.position, target.CurrentEdge.B.transform.position));
    }

    void ReCalculate(int sourceIndex, List<GraphSpot> graph)
    {
        List<GraphSpot> unvisited = new List<GraphSpot>();

        for (int i = 0; i < graph.Count; i++)
        {
            unvisited.Add(graph[i]);
            graph[i].Dist = int.MaxValue;
        }

        for (int i = 0; i < Graph.SpotCount; i++)
        {
            MapSpot spot = Graph.Spots[i];

            for (int j = 0; j < spot.Edges.Count; j++)
            {
                MapSpot neighbor = spot.Edges[j].Neighbor(spot);
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
            _distancesBetweenSpots[sourceIndex, i] = graph[i].Dist;
        }
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

    private class GraphSpot
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
