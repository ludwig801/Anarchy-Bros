using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Enums;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public Transform ObjGround, ObjSpots, ObjEdges, ObjWounds;
    public GameObject TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab, SpotPrefab, TargetPrefab, WoundPrefab;
    public SpotTypes CurrentMode;
    public int SpotCount, EnemySpotCount, TowerSpotCount;
    public bool Targeting
    {
        get { return _targeting; }

        private set
        {
            _targeting = value;
            _refEdge.gameObject.SetActive(value);
            _refSource.gameObject.SetActive(value);
            _refTarget.gameObject.SetActive(value);
        }
    }
    public List<Spot> Spots;
    public List<Edge> Edges;
    public List<WoundBehavior> Wounds;

    GameManager _gameManager;
    TowerManager _towerManager;
    EnemyManager _enemyManager;
    GameObject _targetObj;
    Vector2 _source, _target, _mapBottomLeft, _mapTopRight;
    Edge _refEdge;
    Spot _refSource, _refTarget;
    bool _targeting;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _towerManager = TowerManager.Instance;
        _enemyManager = EnemyManager.Instance;

        // Reference Edges
        GameObject instance = Instantiate(EdgePrefab);
        instance.name = "Ref. Edge";
        instance.GetComponent<Collider2D>().enabled = false;
        instance.transform.parent = transform;
        _refEdge = instance.GetComponent<Edge>();
        // Reference Source
        instance = Instantiate(SpotPrefab);
        instance.name = "Ref. Source";
        instance.GetComponent<Collider2D>().enabled = false;
        instance.transform.parent = transform;
        _refSource = instance.GetComponent<Spot>();
        // Reference Target
        instance = Instantiate(SpotPrefab);
        instance.name = "Ref. Target";
        instance.GetComponent<Collider2D>().enabled = false;
        instance.transform.parent = transform;
        _refTarget = instance.GetComponent<Spot>();
        // Target Object
        _targetObj = Instantiate(TargetPrefab);
        _targetObj.SetActive(false);
        _targetObj.name = "Target";
        _targetObj.transform.parent = transform;

        EnemySpotCount = GetSpotCountOfType(SpotTypes.EnemySpot);
        TowerSpotCount = GetSpotCountOfType(SpotTypes.TowerSpot);
        SpotCount = Spots.Count;

        Targeting = false;
        CurrentMode = SpotTypes.Connection;
    }

    void Update()
    {
        if (_gameManager.IsCurrentState(GameStates.Edit))
        {
            switch (CurrentMode)
            {
                case SpotTypes.Connection:
                    _targetObj.SetActive(false);
                    if (Targeting)
                    {
                        _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _refEdge.SetVertices(_source, _target);
                        _refSource.transform.position = _target;
                        _refTarget.transform.position = _source;
                    }
                    break;

                case SpotTypes.EnemySpot:
                    _targetObj.SetActive(true);
                    _targetObj.transform.position = Tools2D.Convert(_targetObj.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;

                case SpotTypes.TowerSpot:
                    _targetObj.SetActive(true);
                    _targetObj.transform.position = Tools2D.Convert(_targetObj.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    break;
            }
        }
        else
        {
            Targeting = false;
            _mapBottomLeft = ObjGround.transform.position - 0.5f * ObjGround.transform.localScale;
            _mapTopRight = ObjGround.transform.position + 0.5f * ObjGround.transform.localScale;
        }
    }

    void CreateLink(Vector2 source)
    {
        Spot spot;
        if (!SpotAt(source, out spot))
        {
            spot = CreateSpot(source, CurrentMode);
            Edge hitEdge;

            if (EdgeAt(source, out hitEdge))
            {
                SplitEdge(hitEdge, spot);
            }
        }
    }

    void CreateLink(Vector2 source, Vector2 target)
    {
        Spot spotA, spotB;
        Edge hitEdge;

        if (!SpotAt(source, out spotA))
        {
            spotA = CreateSpot(source, CurrentMode);
            if (EdgeAt(source, out hitEdge))
            {
                SplitEdge(hitEdge, spotA);
            }
        }

        if (!SpotAt(target, out spotB))
        {
            spotB = CreateSpot(target, CurrentMode);
            if (EdgeAt(target, out hitEdge))
            {
                SplitEdge(hitEdge, spotB);
            }
        }

        if (AreNeighbors(spotA, spotB))
        {
            return;
        }

        CreateEdge(spotA, spotB);
    }

    void SplitEdge(Edge hitEdge, Spot spliterSpot)
    {
        Vector2 spotA = hitEdge.A.transform.position;
        hitEdge.A.RemoveEdge(hitEdge);

        hitEdge.SetNodes(spliterSpot, hitEdge.B);
        spliterSpot.AddEdge(hitEdge);

        CreateEdge(spliterSpot, SpotAt(spotA));
    }

    Spot CreateSpot(Vector2 worldPos, SpotTypes type)
    {
        GameObject obj;

        switch (type)
        {
            case SpotTypes.Connection:
                obj = Instantiate(SpotPrefab);
                obj.name = "Connection";
                break;

            case SpotTypes.TowerSpot:
                obj = Instantiate(TowerSpotPrefab);
                obj.name = "Tower Spot";
                TowerSpotCount++;
                break;

            case SpotTypes.EnemySpot:
                obj = Instantiate(EnemySpawnPrefab);
                obj.name = "Enemy Spot";
                EnemySpotCount++;
                break;

            default:
                obj = Instantiate(SpotPrefab);
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

    Spot ReplaceSpot(Spot from, SpotTypes toType)
    {
        Spot to = CreateSpot(from.transform.position, toType);
        for (int i = from.Edges.Count - 1; i >= 0; i--)
        {
            Edge e = from.Edges[i];
            e.ReplaceNeighbor(from, to);
            to.AddEdge(e);
            from.RemoveEdge(from.Edges[i]);
        }

        RemoveSpot(from);

        return to;
    }

    Edge CreateEdge(Spot a, Spot b)
    {
        if (AreNeighbors(a, b))
        {
            return null;
        }

        GameObject obj = Instantiate(EdgePrefab);
        obj.name = "Edge";

        Edge e = obj.GetComponent<Edge>();
        e.transform.parent = ObjEdges;
        e.SetNodes(a, b);
        e.A.AddEdge(e);
        e.B.AddEdge(e);
        Edges.Add(e);

        return e;
    }

    void RemoveSpot(Spot s)
    {
        for (int i = s.Edges.Count - 1; i >= 0; i--)
        {
            RemoveEdge(s.Edges[i]);
        }

        switch (s.Type)
        {
            case SpotTypes.EnemySpot:
                EnemySpotCount--;
                break;

            case SpotTypes.TowerSpot:
                TowerSpotCount--;
                break;

            default:
                break;
        }

        Spots.Remove(s);
        Destroy(s.gameObject);
    }

    void RemoveEdge(Edge e)
    {
        Edges.Remove(e);
        e.A.RemoveEdge(e);
        e.B.RemoveEdge(e);

        Destroy(e.gameObject);
    }

    public bool PieceAt(Transform t, Tags.Tag tag, out Piece p)
    {
        p = null;
        if (t != null)
        {
            if (tag == Tags.Tag.Tower)
            {
                p = _towerManager.TowerAt(t.position);
            }
        }

        return p != null;
    }

    public bool EdgeAt(Vector2 pos, out Edge hit)
    {
        hit = null;

        for (int i = 0; i < Edges.Count; i++)
        {
            if (Edges[i].Collider.OverlapPoint(pos))
            {
                hit = Edges[i];
                break;
            }
        }

        return (hit != null);
    }

    public Edge EdgeAt(Vector2 pos)
    {
        Edge hit = null;

        EdgeAt(pos, out hit);

        return hit;
    }

    public Edge EdgeAt(Spot a, Spot b, out Edge hit)
    {
        hit = null;
        for (int i = 0; i < Edges.Count; i++)
        {
            if (Edges[i].Neighbor(a) == b)
            {
                hit = Edges[i];
                break;
            }
        }

        return hit;
    }

    public bool SpotAt(Vector2 pos, out Spot hit)
    {
        hit = null;

        for (int i = 0; i < Spots.Count; i++)
        {
            if (Spots[i].Collider.OverlapPoint(pos))
            {
                hit = Spots[i];
                break;
            }
        }

        return (hit != null);
    }

    public Spot SpotAt(Vector2 pos)
    {
        Spot hit;
        SpotAt(pos, out hit);
        return hit;
    }

    public void OnSpotDrag(PointerEventData eventData, Spot node)
    {
        switch (_gameManager.CurrentState)
        {
            case GameStates.Edit:
                for (int i = 0; i < Edges.Count; i++)
                {
                    Edges[i].OnSpotsPositionChanged();
                }
                break;

            case GameStates.Place:
                break;

            case GameStates.Play:
                break;
        }
    }

    public void OnModeChanged(int newMode)
    {
        Targeting = false;
        CurrentMode = (SpotTypes)newMode;
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
                _targetObj.SetActive(false);
                break;
        }
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

        switch (_gameManager.CurrentState)
        {
            case GameStates.Edit:
                #region Edit
                if (SpotAt(worldPos, out spot))
                {
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (leftBtn)
                            {
                                if (Targeting)
                                {
                                    _target = spot.transform.position;
                                    CreateLink(_source, _target);
                                    _source = _target;  // For continuos targeting
                                }
                                else
                                {
                                    Targeting = true;
                                    _source = spot.transform.position;
                                    _target = _source;
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
                                        ReplaceSpot(spot, SpotTypes.Connection);
                                    }
                                    else
                                    {
                                        RemoveSpot(spot);
                                    }
                                }
                            }
                            break;

                        case SpotTypes.EnemySpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                ReplaceSpot(spot, SpotTypes.EnemySpot);
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
                                        RemoveSpot(spot);
                                    }
                                }
                                else
                                {
                                    ReplaceSpot(spot, SpotTypes.Connection);
                                }
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                ReplaceSpot(spot, SpotTypes.TowerSpot);
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
                                        RemoveSpot(spot);
                                    }
                                }
                                else
                                {
                                    ReplaceSpot(spot, SpotTypes.Connection);
                                }
                            }
                            break;
                    }

                    //Debug.Log("Touched spot: " + spot.Type.ToString());
                }
                else if (EdgeAt(worldPos, out edge))
                {
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (leftBtn)
                            {
                                if (Targeting)
                                {
                                    _target = worldPos;
                                    CreateLink(_source, _target);

                                    // For continuos targeting
                                    _source = _target;
                                    // For one time targeting
                                    // Targeting = false;
                                }
                                else
                                {
                                    Targeting = true;
                                    _source = worldPos;
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
                                    RemoveEdge(edge);
                                }
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                _source = worldPos;
                                CreateLink(_source);
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    RemoveEdge(edge);
                                }
                            }
                            break;

                        case SpotTypes.EnemySpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                _source = worldPos;
                                CreateLink(_source);
                            }
                            else
                            {
                                if (Targeting)
                                {
                                    Targeting = false;
                                }
                                else
                                {
                                    RemoveEdge(edge);
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
                                    _target = worldPos;
                                    CreateLink(_source, _target);
                                    _source = _target;  // For continuos targeting
                                }
                                else
                                {
                                    Targeting = true;
                                    _source = worldPos;
                                }
                            }
                            else
                            {
                                Targeting = false;
                            }
                            break;

                        case SpotTypes.EnemySpot:
                            if (leftBtn)
                            {
                                Targeting = false;
                                CreateSpot(worldPos, SpotTypes.EnemySpot);
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
                                CreateSpot(worldPos, SpotTypes.TowerSpot);
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
                if (SpotAt(worldPos, out spot))
                {
                    _towerManager.OnSpotClicked(spot);
                }
                else if (EdgeAt(worldPos, out edge))
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

    public Spot RandomSpot(SpotTypes type)
    {
        int rand = int.MinValue;
        switch (type)
        {
            case SpotTypes.Connection:
                rand = Random.Range(0, SpotCount - (EnemySpotCount + TowerSpotCount));
                break;

            case SpotTypes.EnemySpot:
                rand = Random.Range(0, EnemySpotCount);
                break;

            case SpotTypes.TowerSpot:
                rand = Random.Range(0, TowerSpotCount);
                break;
        }

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
        Edge hit;
        return EdgeAt(a, b, out hit);
    }

    public Transform NewTarget(Tags.Tag tag)
    {
        Transform newTarget = null;
        if (tag == Tags.Tag.Enemy)
        {

        }
        else if (tag == Tags.Tag.Tower)
        {
            Piece newObjective;
            if (_enemyManager.GetNewObjective(out newObjective))
            {
                newTarget = newObjective.transform;
            }
        }

        return newTarget;
    }

    public void CreateWound(Transform t, Vector2 direction)
    {
        WoundBehavior wound = GetWound();
        wound.Follow = t;
        wound.transform.position = t.position;
        wound.transform.rotation = Tools2D.LookAt(direction);
        wound.Die();
    }

    public WoundBehavior GetWound()
    {
        for (int i = 0; i < Wounds.Count; i++)
        {
            if (!Wounds[i].gameObject.activeSelf)
            {
                return Wounds[i];
            }
        }

        WoundBehavior wound = Instantiate(WoundPrefab).GetComponent<WoundBehavior>();
        wound.transform.SetParent(ObjWounds);
        wound.name = "Wound";
        Wounds.Add(wound);

        return wound;
    }

    #region Graph
    float[,] _distances;

    public void RebuildGraph(IOManager.GameGraph newGraph)
    {
        DestroyGraph();

        for (int i = 0; i < newGraph.Spots.Count; i++)
        {
            IOManager.IOSpot node = newGraph.Spots[i];

            CreateSpot(node.Position.ToVector2, node.Type);
        }

        for (int i = 0; i < newGraph.Edges.Count; i++)
        {
            IOManager.IOEdge edge = newGraph.Edges[i];
            Spot a = SpotAt(edge.a.ToVector2);
            Spot b = SpotAt(edge.b.ToVector2);

            CreateEdge(a, b);
        }
    }

    public void DestroyGraph()
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

        for (int i = 0; i < Spots.Count; i++)
        {
            Spot node = Spots[i];

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

        if (SpotAt(piece.position, out pieceSpot))
        {
            if (SpotAt(target.position, out objectiveSpot))
            {
                result = NextStep(pieceSpot, objectiveSpot);
            }
            else if (EdgeAt(target.position, out objectiveEdge))
            {
                result = NextStep(pieceSpot, objectiveEdge, target);
            }
        }
        else if (EdgeAt(piece.position, out pieceEdge))
        {
            if (SpotAt(target.position, out objectiveSpot))
            {
                result = NextStep(pieceEdge, objectiveSpot, piece);
            }
            else if (EdgeAt(target.position, out objectiveEdge))
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

        if (SpotAt(piece.position, out pieceSpot))
        {
            if (SpotAt(target.position, out objectiveSpot))
            {
                result = DistanceBetween(pieceSpot, objectiveSpot);
            }
            else if (EdgeAt(target.position, out objectiveEdge))
            {
                result = DistanceBetween(pieceSpot, objectiveEdge, target);
            }
        }
        else if (EdgeAt(piece.position, out pieceEdge))
        {
            if (SpotAt(target.position, out objectiveSpot))
            {
                result = DistanceBetween(pieceEdge, objectiveSpot, piece);
            }
            else if (EdgeAt(target.position, out objectiveEdge))
            {
                result = DistanceBetween(pieceEdge, objectiveEdge, piece, target);
            }
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
    #endregion
}