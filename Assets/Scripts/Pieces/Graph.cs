using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Graph : MonoBehaviour
    {
        public Transform ObjSpots, ObjEdges;
        public List<Spot> Spots;
        public List<Edge> Edges;
        public int SpotCount { get { return Spots.Count; } }

        float[,] _distances;

        void Start()
        {
            BuildGraph();
        }

        public void BuildGraph()
        {
            for (int i = 0; i < ObjSpots.childCount; i++)
            {
                Spot n = ObjSpots.GetChild(i).GetComponent<Spot>();
                Spots.Add(n);
            }

            for (int i = 0; i < ObjEdges.childCount; i++)
            {
                Edge e = ObjEdges.GetChild(i).GetComponent<Edge>();
                Edges.Add(e);
            }
        }

        public void OnGameStateChanged(GameStates newState)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edges[i].OnGameStateChanged(newState);
            }

            for (int i = 0; i < Spots.Count; i++)
            {
                Spots[i].OnGameStateChanged(newState);
            }

            switch (newState)
            {
                case GameStates.Edit:
                    break;

                case GameStates.Place:
                    break;

                case GameStates.Play:
                    ReCalculateDistances();
                    break;
            }
        }

        public int GetSpotCountOfType(SpotTypes type)
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

        public Spot SpotOverlaping(Vector2 pos)
        {
            for (int i = 0; i < Spots.Count; i++)
            {
                if (Spots[i].Collider.OverlapPoint(pos))
                {
                    return Spots[i];
                }
            }

            return null;
        }

        public Edge EdgeOverlaping(Vector2 pos)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Collider.OverlapPoint(pos))
                {
                    return Edges[i];
                }
            }

            return null;
        }

        public Edge EdgeConnecting(Spot a, Spot b)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Neighbor(a) == b)
                {
                    return Edges[i];
                }
            }

            return null;
        }

        public Spot RandomSpot(int rand, SpotTypes type)
        {
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

        public bool AreNeighbors(Spot a, Spot b)
        {
            return (EdgeConnecting(a, b) != null);
        }

        public void AddSpot(Spot s)
        {
            Spots.Add(s);
            s.transform.parent = ObjSpots;
        }

        public void AddEdge(Edge e)
        {
            if (AreNeighbors(e.A, e.B))
            {
                return;
            }

            Edges.Add(e);
            e.A.AddEdge(e);
            e.B.AddEdge(e);
            e.transform.parent = ObjEdges;
        }

        public void RemoveSpot(Spot s)
        {
            for (int i = s.Edges.Count - 1; i >= 0; i--)
            {
                RemoveEdge(s.Edges[i]);
            }

            Spots.Remove(s);
            Destroy(s.gameObject);
        }

        public void RemoveEdge(Edge e)
        {
            Edges.Remove(e);
            e.A.RemoveEdge(e);
            e.B.RemoveEdge(e);
            Destroy(e.gameObject);
        }

        public void OnSpotsPositionChanged()
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                Edges[i].OnSpotsPositionChanged();
            }
        }

        public void Clear()
        {
            for (int i = Spots.Count - 1; i >= 0; i--)
            {
                RemoveSpot(Spots[i]);
            }

            for (int i = Edges.Count - 1; i >= 0; i--)
            {
                RemoveEdge(Edges[i]);
            }

            Spots.Clear();
            Edges.Clear();
        }

        public void ReCalculateDistances()
        {
            _distances = new float[Spots.Count, Spots.Count];
            List<GraphNode> graph = new List<GraphNode>();

            for (int i = 0; i < Spots.Count; i++)
            {
                Spot n = Spots[i];
                n.Index = i;
                GraphNode gNode = new GraphNode(n.transform.position, float.MaxValue);
                graph.Add(gNode);
            }

            for (int i = 0; i < Spots.Count; i++)
            {
                CalcDistances(i, graph);
            }

            graph.Clear();
        }

        public Spot NextStep(Spot current, Spot objective)
        {
            float minDist = int.MaxValue;
            Spot best = null;

            int target = objective.Index;

            for (int i = 0; i < current.Edges.Count; i++)
            {
                Spot neighbor = current.Edges[i].Neighbor(current);
                int source = neighbor.Index;
                float d = _distances[source, target];

                if (d < minDist)
                {
                    minDist = d;
                    best = neighbor;
                }
            }

            return best;
        }

        public float DistanceBetween(Spot a, Spot b)
        {
            return _distances[a.Index, b.Index];
        }

        void CalcDistances(int sourceIndex, List<GraphNode> graph)
        {
            List<GraphNode> unvisited = new List<GraphNode>();

            for (int i = 0; i < graph.Count; i++)
            {
                unvisited.Add(graph[i]);
                graph[i].Dist = int.MaxValue;
            }

            for (int i = 0; i < Spots.Count; i++)
            {
                Spot node = Spots[i];

                for (int j = 0; j < node.Edges.Count; j++)
                {
                    Spot neighbor = node.GetNeighbor(j);
                    for (int w = 0; w < unvisited.Count; w++)
                    {
                        if (Tools2D.IsPositionEqual(unvisited[w].position, neighbor.transform.position))
                        {
                            unvisited[i].Neighbors.Add(unvisited[w]);
                        }
                    }
                }
            }

            unvisited[sourceIndex].Dist = 0;

            while (unvisited.Count > 0)
            {
                GraphNode node = unvisited[GetNextBest(unvisited)];
                unvisited.Remove(node);

                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    GraphNode neighbor = node.Neighbors[i];
                    if (unvisited.Contains(neighbor))
                    {
                        float d = node.Dist + Vector2.Distance(neighbor.position, node.position);
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

        int GetNextBest(List<GraphNode> graph)
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

        class GraphNode
        {
            public Vector2 position;
            public float Dist;

            public List<GraphNode> Neighbors;

            public GraphNode(Vector2 pos, float dist)
            {
                position = pos;
                Dist = dist;

                Neighbors = new List<GraphNode>();
            }
        }
    }
}