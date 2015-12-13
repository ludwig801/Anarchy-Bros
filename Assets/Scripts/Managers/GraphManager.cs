﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class GraphManager : MonoBehaviour
    {
        public static GraphManager Instance { get; private set; }

        public Transform SpotsObj, EdgesObj;
        public GameObject TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab, SpotPrefab, TargetPrefab;
        public List<Spot> Spots;
        public List<Edge> Edges;
        public SpotTypes CurrentMode;

        GameObject _targetObj;
        Vector2 _source, _target;
        Edge _refEdge;
        Spot _refSource, _refTarget;
        bool _targeting;
        float[,] _distancesMatrix;
        int _enemySpots, _towerSpots;

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

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameObject instance = Instantiate(EdgePrefab);
            instance.name = "Ref. Edge";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refEdge = instance.GetComponent<Edge>();

            instance = Instantiate(SpotPrefab);
            instance.name = "Ref. Source";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refSource = instance.GetComponent<Spot>();

            instance = Instantiate(SpotPrefab);
            instance.name = "Ref. Target";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refTarget = instance.GetComponent<Spot>();

            _targetObj = Instantiate(TargetPrefab);
            _targetObj.SetActive(false);
            _targetObj.name = "Target";
            _targetObj.transform.parent = transform;

            _enemySpots = 0;
            _towerSpots = 0;

            GetSpots();

            Targeting = false;

            CurrentMode = SpotTypes.Connection;
        }

        void Update()
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Edit))
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

                    case SpotTypes.EnemySpawn:
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
            }
        }

        void GetSpots()
        {
            if (Spots == null)
            {
                Spots = new List<Spot>();
            }
            else
            {
                Spots.Clear();
            }

            for (int i = 0; i < SpotsObj.childCount; i++)
            {
                Spot n = SpotsObj.GetChild(i).GetComponent<Spot>();
                Spots.Add(n);

                switch (n.Type)
                {
                    case SpotTypes.TowerSpot:
                        _towerSpots++;
                        break;

                    case SpotTypes.EnemySpawn:
                        _enemySpots++;
                        break;

                    default:
                        break;
                }
            }

            for (int i = 0; i < EdgesObj.childCount; i++)
            {
                Edge e = EdgesObj.GetChild(i).GetComponent<Edge>();
                Edges.Add(e);
            }
        }

        void CreateLink(Vector2 source)
        {
            Spot node;
            Edge hitEdge;

            if (!GetHitSpot(source, out node))
            {
                node = CreateSpot(source, CurrentMode);
                if (GetHitEdge(source, out hitEdge))
                {
                    SplitEdge(hitEdge, node);
                }
            }
        }

        void CreateLink(Vector2 source, Vector2 target)
        {
            Spot nodeA, nodeB;
            Edge hitEdge;

            if (!GetHitSpot(source, out nodeA))
            {
                nodeA = CreateSpot(source, CurrentMode);
                if (GetHitEdge(source, out hitEdge))
                {
                    SplitEdge(hitEdge, nodeA);
                }
            }

            if (!GetHitSpot(target, out nodeB))
            {
                nodeB = CreateSpot(target, CurrentMode);
                if (GetHitEdge(target, out hitEdge))
                {
                    SplitEdge(hitEdge, nodeB);
                }
            }

            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].GetNeighbor(nodeA) == nodeB)
                {
                    return;
                }
            }

            CreateEdge(nodeA, nodeB);
        }

        void SplitEdge(Edge hitEdge, Spot spliterSpot)
        {
            Vector2 spotA = hitEdge.A.transform.position;
            hitEdge.A.RemoveEdge(hitEdge);

            hitEdge.SetNodes(spliterSpot, hitEdge.B);
            spliterSpot.AddEdge(hitEdge);

            CreateEdge(spliterSpot, GetHitSpot(spotA));
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
                    _towerSpots++;
                    break;

                case SpotTypes.EnemySpawn:
                    obj = Instantiate(EnemySpawnPrefab);
                    obj.name = "Enemy Spot";
                    _enemySpots++;
                    break;

                default:
                    obj = Instantiate(SpotPrefab);
                    obj.name = "<Undefined>";
                    break;
            }

            obj.layer = LayerMask.NameToLayer("Spots");
            obj.transform.position = worldPos;
            obj.transform.parent = SpotsObj;

            Spot n = obj.GetComponent<Spot>();
            n.Type = type;

            Spots.Add(n);
            return n;
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
            GameObject obj = Instantiate(EdgePrefab);
            obj.transform.parent = EdgesObj;
            obj.name = "Edge";

            Edge e = obj.GetComponent<Edge>();
            e.SetNodes(a, b);
            Edges.Add(e);
            a.AddEdge(e);
            b.AddEdge(e);

            return e;
        }

        void RemoveSpot(Spot spot)
        {
            for (int i = spot.Edges.Count - 1; i >= 0; i--)
            {
                RemoveEdge(spot.Edges[i]);
            }

            switch (spot.Type)
            {
                case SpotTypes.EnemySpawn:
                    _enemySpots--;
                    break;

                case SpotTypes.TowerSpot:
                    _towerSpots--;
                    break;

                default: break;
            }

            Spots.Remove(spot);
            Destroy(spot.gameObject);
        }

        void RemoveEdge(Edge edge)
        {
            Edges.Remove(edge);
            edge.A.RemoveEdge(edge);
            edge.B.RemoveEdge(edge);
            Destroy(edge.gameObject);
        }

        bool GetHitSpot(Vector2 pos, out Spot hit)
        {
            hit = null;
            for (int i = 0; i < Spots.Count; i++)
            {
                if (Spots[i].Collider.OverlapPoint(pos))
                {
                    hit = Spots[i];
                    return true;
                }
            }

            return false;
        }

        bool GetHitEdge(Vector2 pos, out Edge hit)
        {
            hit = null;
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Collider.OverlapPoint(pos))
                {
                    hit = Edges[i];
                    return true;
                }
            }

            return false;
        }

        public Edge GetHitEdge(Spot a, Spot b)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].GetNeighbor(a) == b)
                {
                    return Edges[i];
                }
            }

            return null;
        }

        public Spot GetHitSpot(Vector2 pos)
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

        public void OnGroundClick(BaseEventData baseData)
        {
            PointerEventData eventData = baseData as PointerEventData;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                switch (CurrentMode)
                {
                    case SpotTypes.Connection:
                        if (Targeting)
                        {
                            _target = eventData.pointerCurrentRaycast.worldPosition;
                            CreateLink(_source, _target);

                            // For continuos targeting
                            _source = _target;
                            // For one time targeting
                            // Targeting = false;
                        }
                        else
                        {
                            Targeting = true;
                            _source = eventData.pointerCurrentRaycast.worldPosition;
                        }
                        break;

                    case SpotTypes.EnemySpawn:
                        Targeting = false;
                        CreateSpot(eventData.pointerCurrentRaycast.worldPosition, SpotTypes.EnemySpawn);
                        break;

                    case SpotTypes.TowerSpot:
                        Targeting = false;
                        CreateSpot(eventData.pointerCurrentRaycast.worldPosition, SpotTypes.TowerSpot);
                        break;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Targeting = false;
            }
        }

        public void OnSpotClick(PointerEventData eventData, Spot spot)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Edit))
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    //Debug.Log("Clicked spot of type: " + spot.Type.ToString() + " while in mode: + " + CurrentMode.ToString());
                    switch (CurrentMode)
                    {
                        case SpotTypes.Connection:
                            if (Targeting)
                            {
                                _target = spot.transform.position;
                                CreateLink(_source, _target);

                                // For continuos targeting
                                _source = _target;
                                // For one time targeting
                                // Targeting = false;
                            }
                            else
                            {
                                //spot = ReplaceSpot(spot, SpotTypes.TowerSpot);
                                Targeting = true;
                                _source = spot.transform.position;
                                _target = _source;
                            }
                            break;

                        case SpotTypes.TowerSpot:
                            Targeting = false;
                            ReplaceSpot(spot, SpotTypes.TowerSpot);
                            break;

                        case SpotTypes.EnemySpawn:
                            Targeting = false;
                            ReplaceSpot(spot, SpotTypes.EnemySpawn);
                            break;
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    switch (spot.Type)
                    {
                        case SpotTypes.Connection:
                            RemoveSpot(spot);
                            break;

                        case SpotTypes.EnemySpawn:
                            ReplaceSpot(spot, SpotTypes.Connection);
                            break;

                        case SpotTypes.TowerSpot:
                            ReplaceSpot(spot, SpotTypes.Connection);
                            break;
                    }
                }
            }
            else if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                TowerManager.Instance.OnNodeClicked(spot);
            }
            else if (GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                TowerManager.Instance.OnNodeClicked(spot);
            }
        }

        public void OnSpotDrag(PointerEventData eventData, Spot node)
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Edit))
            {
                return;
            }

            for (int i = 0; i < Edges.Count; i++)
            {
                Edges[i].ReEvaluate();
            }
        }

        public void OnEdgeClick(PointerEventData eventData, Edge edge)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                switch (CurrentMode)
                {
                    case SpotTypes.Connection:
                        if (Targeting)
                        {
                            _target = eventData.pointerCurrentRaycast.worldPosition;
                            CreateLink(_source, _target);

                            // For continuos targeting
                            _source = _target;
                            // For one time targeting
                            // Targeting = false;
                        }
                        else
                        {
                            Targeting = true;
                            _source = eventData.pointerCurrentRaycast.worldPosition;
                        }
                        break;

                    case SpotTypes.TowerSpot:
                        Targeting = false;
                        _source = eventData.pointerCurrentRaycast.worldPosition;
                        CreateLink(_source);
                        break;

                    case SpotTypes.EnemySpawn:
                        Targeting = false;
                        _source = eventData.pointerCurrentRaycast.worldPosition;
                        CreateLink(_source);
                        break;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveEdge(edge);
            }
        }

        public void OnModeChanged(int newMode)
        {
            Targeting = false;
            CurrentMode = (SpotTypes)newMode;
        }

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
                Spot a = GetHitSpot(edge.a.ToVector2);
                Spot b = GetHitSpot(edge.b.ToVector2);

                CreateEdge(a, b);
            }
        }

        public void DestroyGraph()
        {
            for (int i = 0; i < SpotsObj.childCount; i++)
            {
                RemoveSpot(SpotsObj.GetChild(i).GetComponent<Spot>());
            }

            for (int i = 0; i < EdgesObj.childCount; i++)
            {
                Destroy(EdgesObj.GetChild(i).gameObject);
            }

            Spots.Clear();
            Edges.Clear();
        }

        public void ReEvaluate()
        {
            switch (GameManager.Instance.CurrentState)
            {
                case GameStates.Place:
                    //_targetObj.SetActive(false);
                    break;

                case GameStates.Play:
                    _targetObj.SetActive(false);
                    break;
            }
            if (GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                _distancesMatrix = new float[Spots.Count, Spots.Count];
                List<GraphSpot> graph = new List<GraphSpot>();

                for (int i = 0; i < Spots.Count; i++)
                {
                    Spot n = Spots[i];
                    n.Index = i;
                    GraphSpot gNode = new GraphSpot(n.transform.position, float.MaxValue);
                    graph.Add(gNode);
                }

                for (int i = 0; i < Spots.Count; i++)
                {
                    CalcDistances(i, graph);
                }

                graph.Clear();
            }
        }

        public Spot GetRandomSpot(SpotTypes type = SpotTypes.Any)
        {
            int rand;
            switch (type)
            {
                case SpotTypes.Connection:
                    rand = Random.Range(0, Spots.Count - (_enemySpots + _towerSpots));
                    for (int i = 0; i < Spots.Count; i++)
                    {
                        if (Spots[i].Type == SpotTypes.Connection)
                        {
                            if (rand <= 0)
                            {
                                return Spots[i];
                            }
                            rand--;
                        }
                    }
                    break;

                case SpotTypes.EnemySpawn:
                    rand = Random.Range(0, _enemySpots);
                    for (int i = 0; i < Spots.Count; i++)
                    {
                        if (Spots[i].Type == SpotTypes.EnemySpawn)
                        {
                            if (rand <= 0)
                            {
                                return Spots[i];
                            }
                            rand--;
                        }
                    }
                    break;

                case SpotTypes.TowerSpot:
                    rand = Random.Range(0, _towerSpots);
                    for (int i = 0; i < Spots.Count; i++)
                    {
                        if (Spots[i].Type == SpotTypes.TowerSpot)
                        {
                            if (rand <= 0)
                            {
                                return Spots[i];
                            }
                            rand--;
                        }
                    }
                    break;

                default:
                    rand = Random.Range(0, Spots.Count);
                    return Spots[rand];
                    //break;
            }

            return null;
        }

        public Spot GetBestSpot(Spot current, Spot objective)
        {
            if (current == objective)
            {
                return objective;
            }

            int target = objective.Index;
            float min = float.MaxValue;
            int best = 0;

            for (int i = 0; i < current.Edges.Count; i++)
            {
                int neighbor = current.GetNeighbor(i).Index;
                if (_distancesMatrix[neighbor, target] < min)
                {
                    min = _distancesMatrix[neighbor, target];
                    best = neighbor;
                }
            }

            return Spots[best];
        }

        public Spot GetBestSpot(Vector2 currentPos, Spot a, Spot b, Spot objective)
        {
            float distA = Vector2.Distance(currentPos, a.transform.position) + _distancesMatrix[a.Index, objective.Index];
            float distB = Vector2.Distance(currentPos, b.transform.position) + _distancesMatrix[b.Index, objective.Index];
            return (distA < distB) ? a : b;
        }

        #region PathEval

        void CalcDistances(int sourceIndex, List<GraphSpot> graph)
        {
            List<GraphSpot> unvisited = new List<GraphSpot>();

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
                GraphSpot node = unvisited[GetNextBest(unvisited)];
                unvisited.Remove(node);

                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    GraphSpot neighbor = node.Neighbors[i];
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
                _distancesMatrix[sourceIndex, i] = graph[i].Dist;
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

        class GraphSpot
        {
            public Vector2 position;
            public float Dist;

            public List<GraphSpot> Neighbors;

            public GraphSpot(Vector2 pos, float dist)
            {
                position = pos;
                Dist = dist;

                Neighbors = new List<GraphSpot>();
            }
        }

        #endregion
    }
}