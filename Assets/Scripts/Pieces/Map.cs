using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    public Transform ObjGround, ObjSpots, ObjEdges;
    public SpotTypes CurrentMode;
    public SpotManager Spots;
    public EdgeManager Edges;
    public bool Targeting
    {
        get { return _targeting; }

        private set
        {
            _targeting = value;
        }
    }

    GameController _gameController;
    Vector2 _editSource, _editTarget, _mapBottomLeft, _mapTopRight;
    bool _targeting;

    void Start()
    {
        _gameController = GameController.Instance;

        Spots = ObjSpots.GetComponent<SpotManager>();
        Edges = ObjEdges.GetComponent<EdgeManager>();

        Targeting = false;
        CurrentMode = SpotTypes.Connection;
    }

    void Update()
    {
        _mapBottomLeft = ObjGround.transform.position - 0.5f * ObjGround.transform.localScale;
        _mapTopRight = ObjGround.transform.position + 0.5f * ObjGround.transform.localScale;
    }

    void CreateLink(Vector2 source)
    {
        Spot spot;
        if (!Spots.Find(source, out spot))
        {
            spot = Spots.Create(source, CurrentMode);
            Edge hitEdge;

            if (Edges.Find(source, out hitEdge))
            {
                SplitEdge(hitEdge, spot);
            }
        }
    }

    void CreateLink(Vector2 source, Vector2 target)
    {
        Spot spotA, spotB;
        Edge hitEdge;

        if (!Spots.Find(source, out spotA))
        {
            spotA = Spots.Create(source, CurrentMode);
            if (Edges.Find(source, out hitEdge))
            {
                SplitEdge(hitEdge, spotA);
            }
        }

        if (!Spots.Find(target, out spotB))
        {
            spotB = Spots.Create(target, CurrentMode);
            if (Edges.Find(target, out hitEdge))
            {
                SplitEdge(hitEdge, spotB);
            }
        }

        if (Edges.AreNeighbors(spotA, spotB))
        {
            return;
        }

        Edges.CreateEdge(spotA, spotB);
    }

    void RemoveSpotsAndEdges(Spot spot)
    {
        Edges.Remove(spot);
        Spots.Remove(spot);
    }

    void SplitEdge(Edge hitEdge, Spot spliterSpot)
    {
        Vector2 spotA = hitEdge.A.transform.position;
        hitEdge.A.RemoveEdge(hitEdge);

        hitEdge.SetNodes(spliterSpot, hitEdge.B);
        spliterSpot.AddEdge(hitEdge);

        Edges.CreateEdge(spliterSpot, Spots.Find(spotA));
    }

    public void OnModeChanged(int newMode)
    {
        Targeting = false;
        CurrentMode = (SpotTypes)newMode;
    }

    public bool OutOfMap(Vector2 position, Vector2 margin)
    {
        Vector2 objBottomLeft = position - margin;
        Vector2 objTopRight = position + margin;

        return Tools2D.NotInside(objBottomLeft, objTopRight, _mapBottomLeft, _mapTopRight);
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        PointerEventData pEventData = eventData as PointerEventData;
        Vector2 worldPos = pEventData.pointerCurrentRaycast.worldPosition;

        bool leftBtn = pEventData.button == PointerEventData.InputButton.Left;
        //bool rightBtn = pEventData.button == PointerEventData.InputButton.Right;

        Spot spot;
        Edge edge;

        switch (_gameController.CurrentState)
        {
            case GameStates.Edit:
                #region Edit
                if (Spots.Find(worldPos, out spot))
                {
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (leftBtn)
                            {
                                if (Targeting)
                                {
                                    _editTarget = spot.transform.position;
                                    CreateLink(_editSource, _editTarget);
                                    _editSource = _editTarget;  // For continuos targeting
                                }
                                else
                                {
                                    Targeting = true;
                                    _editSource = spot.transform.position;
                                    _editTarget = _editSource;
                                }
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    if (spot.Type != SpotTypes.Connection)
                                    {
                                        Spots.ChangeType(spot, SpotTypes.Connection);
                                    }
                                    else
                                    {
                                        RemoveSpotsAndEdges(spot);
                                    }
                                }
                            }
                            break;

                        case SpotTypes.EnemySpawn:
                            if (leftBtn)
                            {
                                Targeting = false;
                                Spots.ChangeType(spot, SpotTypes.EnemySpawn);
                            }
                            else
                            {
                                if (spot.Type == SpotTypes.Connection)
                                {
                                    if (Targeting)
                                    {
                                        Targeting = false;
                                    }
                                    else
                                    {
                                        RemoveSpotsAndEdges(spot);
                                    }
                                }
                                else
                                {
                                    Spots.ChangeType(spot, SpotTypes.Connection);
                                }
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                Spots.ChangeType(spot, SpotTypes.TowerSpot);
                            }
                            else
                            {
                                if (spot.Type == SpotTypes.Connection)
                                {
                                    if (Targeting)
                                    {
                                        Targeting = false;
                                    }
                                    else
                                    {
                                        RemoveSpotsAndEdges(spot);
                                    }
                                }
                                else
                                {
                                    Spots.ChangeType(spot, SpotTypes.Connection);
                                }
                            }
                            break;
                    }

                    //Debug.Log("Touched spot: " + spot.Type.ToString());
                }
                else if (Edges.Find(worldPos, out edge))
                {
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (leftBtn)
                            {
                                if (Targeting)
                                {
                                    _editTarget = worldPos;
                                    CreateLink(_editSource, _editTarget);

                                    // For continuos targeting
                                    _editSource = _editTarget;
                                    // For one time targeting
                                    // Targeting = false;
                                }
                                else
                                {
                                    Targeting = true;
                                    _editSource = worldPos;
                                }
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    Edges.Remove(edge);
                                }
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                _editSource = worldPos;
                                CreateLink(_editSource);
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    Edges.Remove(edge);
                                }
                            }
                            break;

                        case SpotTypes.EnemySpawn:
                            if (leftBtn)
                            {
                                Targeting = false;
                                _editSource = worldPos;
                                CreateLink(_editSource);
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    Edges.Remove(edge);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (leftBtn)
                            {
                                if (Targeting)
                                {
                                    _editTarget = worldPos;
                                    CreateLink(_editSource, _editTarget);
                                    _editSource = _editTarget;  // For continuos targeting
                                }
                                else
                                {
                                    Targeting = true;
                                    _editSource = worldPos;
                                }
                            }
                            else
                            {
                                Targeting = false;
                            }
                            break;

                        case SpotTypes.EnemySpawn:
                            if (leftBtn)
                            {
                                Targeting = false;
                                Spots.Create(worldPos, SpotTypes.EnemySpawn);
                            }
                            else
                            {
                                Targeting = false;
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                Spots.Create(worldPos, SpotTypes.TowerSpot);
                            }
                            else
                            {
                                Targeting = false;
                            }
                            break;
                    }
                }
                #endregion
                break;

            case GameStates.Place:
                #region Place
                if (Spots.Find(worldPos, out spot))
                {
                    _gameController.OnSpotClicked(spot);
                }
                else if (Edges.Find(worldPos, out edge))
                {
                    Debug.Log("Touched edge");
                }
                else
                {
                    Debug.Log("Touched ground");
                }
                #endregion
                break;

            case GameStates.Play:
                #region Play
                #endregion
                break;
        }
    }

    #region Graph
    float[,] _distances;

    public void DestroySpotsAndEdges()
    {
        Spots.RemoveAll();
        Edges.RemoveAll();
    }

    public void ReCalculateDistances()
    {
        int spotCount = Spots.SpotCount;

        _distances = new float[spotCount, spotCount];
        List<GraphNode> graph = new List<GraphNode>();

        for (int i = 0; i < spotCount; i++)
        {
            Spot n = Spots.Objects[i];
            n.Index = i;
            GraphNode gNode = new GraphNode(n.transform.position, float.MaxValue);
            graph.Add(gNode);
        }

        for (int i = 0; i < graph.Count; i++)
        {
            ReCalculateDistances(i, graph);
        }

        graph.Clear();
    }

    void ReCalculateDistances(int sourceIndex, List<GraphNode> graph)
    {
        List<GraphNode> unvisited = new List<GraphNode>();

        for (int i = 0; i < graph.Count; i++)
        {
            unvisited.Add(graph[i]);
            graph[i].Dist = int.MaxValue;
        }

        for (int i = 0; i < Spots.SpotCount; i++)
        {
            Spot node = Spots.Objects[i];

            for (int j = 0; j < node.Edges.Count; j++)
            {
                Spot neighbor = node.GetNeighbor(j);
                for (int w = 0; w < unvisited.Count; w++)
                {
                    if (Tools2D.At(unvisited[w].position, neighbor.transform.position))
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

    public Transform NextStep(Transform piece, Transform target)
    {
        Spot pieceSpot, objectiveSpot;
        Edge pieceEdge, objectiveEdge;
        Transform result = null;

        if (Spots.Find(piece.position, out pieceSpot))
        {
            if (Spots.Find(target.position, out objectiveSpot))
            {
                result = NextStep(pieceSpot, objectiveSpot);
            }
            else if (Edges.Find(target.position, out objectiveEdge))
            {
                result = NextStep(pieceSpot, objectiveEdge, target);
            }
        }
        else if (Edges.Find(piece.position, out pieceEdge))
        {
            if (Spots.Find(target.position, out objectiveSpot))
            {
                result = NextStep(pieceEdge, objectiveSpot, piece);
            }
            else if (Edges.Find(target.position, out objectiveEdge))
            {
                result = NextStep(pieceEdge, objectiveEdge, piece, target);
            }
        }

        return result;
    }

    public Transform NextStep(Spot spot, Spot objective)
    {
        if (spot == objective)
        {
            return objective.transform;
        }

        float minDist = int.MaxValue;
        Spot best = null;

        int target = objective.Index;

        for (int i = 0; i < spot.Edges.Count; i++)
        {
            Spot neighbor = spot.Edges[i].Neighbor(spot);
            int source = neighbor.Index;
            float d = _distances[source, target] + DistanceBetween(spot, neighbor);

            if (d < minDist)
            {
                minDist = d;
                best = neighbor;
            }
        }

        return best.transform;
    }

    public Transform NextStep(Spot spot, Edge objective, Transform target)
    {
        if (spot.Edges.Contains(objective))
        {
            return target;
        }

        float minDist = float.MaxValue;
        Spot best = null;

        int tA = objective.A.Index;
        int tB = objective.B.Index;

        for (int i = 0; i < spot.Edges.Count; i++)
        {
            Spot neighbor = spot.Edges[i].Neighbor(spot);
            int source = neighbor.Index;
            float d = DistanceBetween(spot, neighbor);
            float dA = _distances[source, tA];
            float dB = _distances[source, tB];
            d += (dA < dB) ? dA : dB;

            if (d < minDist)
            {
                minDist = d;
                best = neighbor;
            }
        }

        return best.transform;
    }

    public Transform NextStep(Edge edge, Spot objective, Transform piece)
    {
        if (objective.Edges.Contains(edge))
        {
            return objective.transform;
        }

        float distA = Vector2.Distance(piece.position, edge.A.transform.position) + DistanceBetween(edge.A, objective);
        float distB = Vector2.Distance(piece.position, edge.B.transform.position) + DistanceBetween(edge.B, objective);
        return (distA < distB) ? edge.A.transform : edge.B.transform;
    }

    public Transform NextStep(Edge edge, Edge objective, Transform piece, Transform target)
    {
        if (edge == objective)
        {
            return target;
        }

        Spot a = edge.A;
        Spot b = edge.B;
        Spot c = objective.A;
        Spot d = objective.B;

        float dAC = Tools2D.Distance(piece.position, a.transform.position) + DistanceBetween(a, c)
                        + Tools2D.Distance(target.position, c.transform.position);
        float dAD = Tools2D.Distance(piece.position, a.transform.position) + DistanceBetween(a, d)
                        + Tools2D.Distance(target.position, d.transform.position);
        float dBC = Tools2D.Distance(piece.position, b.transform.position) + DistanceBetween(b, c)
                        + Tools2D.Distance(target.position, c.transform.position);
        float dBD = Tools2D.Distance(piece.position, b.transform.position) + DistanceBetween(b, d)
                        + Tools2D.Distance(target.position, d.transform.position);

        if (dAC < dAD)
        {
            if (dAC < dBC && dAC < dBD)
            {
                return a.transform;
            }
            else
            {
                return b.transform;
            }
        }
        else if (dAD < dBC && dAD < dBD)
        {
            return a.transform;
        }
        else
        {
            return b.transform;
        }
    }

    public float DistanceBetween(Transform piece, Transform target)
    {
        Spot pieceSpot, objectiveSpot;
        Edge pieceEdge, objectiveEdge;
        float result = float.MaxValue;

        if (Spots.Find(piece.position, out pieceSpot))
        {
            if (Spots.Find(target.position, out objectiveSpot))
            {
                result = DistanceBetween(pieceSpot, objectiveSpot);
            }
            else if (Edges.Find(target.position, out objectiveEdge))
            {
                result = DistanceBetween(pieceSpot, objectiveEdge, target);
            }
        }
        else if (Edges.Find(piece.position, out pieceEdge))
        {
            if (Spots.Find(target.position, out objectiveSpot))
            {
                result = DistanceBetween(pieceEdge, objectiveSpot, piece);
            }
            else if (Edges.Find(target.position, out objectiveEdge))
            {
                result = DistanceBetween(pieceEdge, objectiveEdge, piece, target);
            }
        }

        if (result == float.MaxValue)
        {
            Debug.LogWarning("Bad result");
        }

        return result;
    }

    public float DistanceBetween(Spot spot, Spot objective)
    {
        return _distances[spot.Index, objective.Index];
    }

    public float DistanceBetween(Spot spot, Edge objective, Transform target)
    {
        if (spot.Edges.Contains(objective))
        {
            return Tools2D.Distance(spot.transform.position, target.transform.position);
        }

        float minDist = float.MaxValue;

        int tA = objective.A.Index;
        int tB = objective.B.Index;

        for (int i = 0; i < spot.Edges.Count; i++)
        {
            Spot neighbor = spot.Edges[i].Neighbor(spot);
            int source = neighbor.Index;
            float d = DistanceBetween(spot, neighbor);
            float dA = _distances[source, tA];
            float dB = _distances[source, tB];
            d += (dA < dB) ? dA : dB;

            if (d < minDist)
            {
                minDist = d;
            }
        }

        return minDist;
    }

    public float DistanceBetween(Edge edge, Spot objective, Transform piece)
    {
        if (objective.Edges.Contains(edge))
        {
            return Tools2D.Distance(piece.transform.position, objective.transform.position);
        }

        float distA = Vector2.Distance(piece.position, edge.A.transform.position) + DistanceBetween(edge.A, objective);
        float distB = Vector2.Distance(piece.position, edge.B.transform.position) + DistanceBetween(edge.B, objective);
        return Mathf.Min(distA, distB);
    }

    public float DistanceBetween(Edge edge, Edge objective, Transform piece, Transform target)
    {
        if (edge == objective)
        {
            return Tools2D.Distance(piece.transform.position, target.transform.position);
        }

        Spot a = edge.A;
        Spot b = edge.B;
        Spot c = objective.A;
        Spot d = objective.B;

        float dAC = Tools2D.Distance(piece.position, a.transform.position) + DistanceBetween(a, c)
                        + Tools2D.Distance(target.position, c.transform.position);
        float dAD = Tools2D.Distance(piece.position, a.transform.position) + DistanceBetween(a, d)
                        + Tools2D.Distance(target.position, d.transform.position);
        float dBC = Tools2D.Distance(piece.position, b.transform.position) + DistanceBetween(b, c)
                        + Tools2D.Distance(target.position, c.transform.position);
        float dBD = Tools2D.Distance(piece.position, b.transform.position) + DistanceBetween(b, d)
                        + Tools2D.Distance(target.position, d.transform.position);

        return Mathf.Min(dAC, Mathf.Min(dAD, Mathf.Min(dBC, dBD)));
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
        internal Vector2 position;
        internal float Dist;

        internal List<GraphNode> Neighbors;

        internal GraphNode(Vector2 pos, float dist)
        {
            position = pos;
            Dist = dist;

            Neighbors = new List<GraphNode>();
        }
    }
    #endregion
}