using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class GraphManager : MonoBehaviour
    {
        public static GraphManager Instance { get; private set; }

        public Transform TowerSpotsObj, EnemySpawnsObj, NodesObj, EdgesObj;
        public GameObject TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab, NodePrefab, TargetPrefab;
        public List<Node> Nodes;
        public List<Edge> Edges;
        public Mode CurrentMode;

        public enum Mode
        {
            Edge = 0,
            TowerSpot = 1,
            EnemySpawn = 2
        }

        Vector2 _source, _target;
        Edge _refEdge, _hitEdge;
        Node _refSource, _refTarget, _hitNode;
        bool _targeting;
        float[,] Distances;
        GameObject _targetObj;

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
            instance.name = "Ref Edge";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refEdge = instance.GetComponent<Edge>();

            instance = Instantiate(NodePrefab);
            instance.name = "Ref Source";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refSource = instance.GetComponent<Node>();

            instance = Instantiate(NodePrefab);
            instance.name = "Ref Target";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refTarget = instance.GetComponent<Node>();

            _targetObj = Instantiate(TargetPrefab);
            _targetObj.SetActive(false);
            _targetObj.name = "Target";
            _targetObj.transform.parent = transform;

            GetAllNodes();

            Targeting = false;

            CurrentMode = Mode.Edge;
        }

        void Update()
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Edit))
            {
                switch (CurrentMode)
                {
                    case Mode.Edge:
                        _targetObj.SetActive(false);
                        if (Targeting)
                        {
                            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            _refEdge.SetVertices(_source, _target);
                            _refSource.transform.position = _target;
                            _refTarget.transform.position = _source;
                        }
                        break;

                    case Mode.EnemySpawn:
                        _targetObj.SetActive(true);
                        _targetObj.transform.position = Tools2D.ConvertKeepZ(_targetObj.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        break;

                    case Mode.TowerSpot:
                        _targetObj.SetActive(true);
                        _targetObj.transform.position = Tools2D.ConvertKeepZ(_targetObj.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                        break;
                }             
            }
            else
            {
                Targeting = false;
            }
        }

        void GetAllNodes()
        {
            if (Nodes == null)
            {
                Nodes = new List<Node>();
            }
            else
            {
                Nodes.Clear();
            }

            for (int i = 0; i < TowerSpotsObj.childCount; i++)
            {
                Node n = TowerSpotsObj.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.TowerSpot;

                if (n != null)
                {
                    Nodes.Add(n);
                }
            }

            for (int i = 0; i < EnemySpawnsObj.childCount; i++)
            {
                Node n = EnemySpawnsObj.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.EnemySpawn;

                if (n != null)
                {
                    Nodes.Add(n);
                }
            }

            for (int i = 0; i < NodesObj.childCount; i++)
            {
                Node n = NodesObj.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.Node;

                Nodes.Add(n);
            }

            for (int i = 0; i < EdgesObj.childCount; i++)
            {
                Edge e = EdgesObj.GetChild(i).GetComponent<Edge>();

                Edges.Add(e);
            }
        }

        public void CreateLink()
        {
            Node nodeA, nodeB;
            Edge hitEdge;

            if (!GetHitNode(_source, out nodeA))
            {
                nodeA = CreateNode(_source, Node.NodeType.Node);
                if (GetHitEdge(_source, out hitEdge))
                {
                    SplitEdge(hitEdge, nodeA);
                }
            }

            if (!GetHitNode(_target, out nodeB))
            {
                nodeB = CreateNode(_target, Node.NodeType.Node);
                if (GetHitEdge(_target, out hitEdge))
                {
                    SplitEdge(hitEdge, nodeB);
                }
            }

            if (NodesAreNeighbors(nodeA, nodeB))
            {
                return;
            }

            CreateEdge(nodeA, nodeB);
        }

        public void SplitEdge(Edge e, Node spliter)
        {
            Node oldA = e.A;
            e.SetNodes(spliter, e.B);

            Edges.Add(CreateEdge(spliter, oldA));
        }

        public Node CreateNode(Vector2 worldPos, Node.NodeType type)
        {
            GameObject obj;

            switch (type)
            {
                case Node.NodeType.Node:
                    obj = Instantiate(NodePrefab);
                    obj.name = "Node";
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = NodesObj;
                    break;

                case Node.NodeType.TowerSpot:
                    obj = Instantiate(TowerSpotPrefab);
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = TowerSpotsObj;
                    break;

                case Node.NodeType.EnemySpawn:
                    obj = Instantiate(EnemySpawnPrefab);
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = EnemySpawnsObj;
                    break;

                default:
                    obj = Instantiate(NodePrefab);
                    obj.name = "Node";
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = NodesObj;
                    break;
            }

            Node n = obj.GetComponent<Node>();
            n.Type = type;

            Nodes.Add(n);
            return n;
        }

        public Edge CreateEdge(Node a, Node b)
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

        public void RemoveNode(Node n)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].HasNode(n))
                {
                    RemoveEdge(Edges[i]);
                    i--;
                }
            }
            Nodes.Remove(n);
            Destroy(n.gameObject);
        }

        public void RemoveEdge(Edge e)
        {
            e.A.Edges.Remove(e);
            e.B.Edges.Remove(e);
            Edges.Remove(e);
            Destroy(e.gameObject);
        }

        public bool GetHitNode(Vector2 pos, out Node hit)
        {
            hit = null;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Collider.OverlapPoint(pos))
                {
                    hit = Nodes[i];
                    return true;
                }
            }

            return false;
        }

        public T GetHitNode<T>(Vector2 pos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Collider.OverlapPoint(pos))
                {
                    return Nodes[i].GetComponent<T>();
                }
            }

            return default(T);
        }

        public bool GetHitEdge(Vector2 pos, out Edge hit)
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

        public bool NodesAreNeighbors(Node a, Node b)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].GetNeighbor(a) == b)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnGroundClick(BaseEventData baseData)
        {
            PointerEventData eventData = baseData as PointerEventData;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                switch (CurrentMode)
                {
                    case Mode.Edge:
                        if (Targeting)
                        {
                            _target = eventData.pointerCurrentRaycast.worldPosition;
                            CreateLink();

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

                    case Mode.EnemySpawn:
                        Targeting = false;
                        CreateNode(eventData.pointerCurrentRaycast.worldPosition, Node.NodeType.EnemySpawn);
                        break;

                    case Mode.TowerSpot:
                        Targeting = false;
                        CreateNode(eventData.pointerCurrentRaycast.worldPosition, Node.NodeType.TowerSpot);
                        break;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Targeting = false;
            }
        }

        public void OnNodeClick(PointerEventData eventData, Node node)
        {
            if (GameManager.Instance.IsCurrentState(GameStates.Edit))
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    switch (CurrentMode)
                    {
                        case Mode.Edge:
                            if (Targeting)
                            {
                                _target = node.transform.position;
                                CreateLink();

                                // For continuos targeting
                                _source = _target;
                                // For one time targeting
                                // Targeting = false;
                            }
                            else
                            {
                                Targeting = true;
                                _source = node.transform.position;
                                _target = _source;
                            }
                            break;

                        default:
                            break;
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    switch (CurrentMode)
                    {
                        case Mode.Edge:
                            if (Targeting)
                            {
                                Targeting = false;
                            }
                            else
                            {
                                RemoveNode(node);
                            }
                            break;

                        case Mode.EnemySpawn:
                            RemoveNode(node);
                            break;

                        case Mode.TowerSpot:
                            RemoveNode(node);
                            break;
                    }
                }
            }
            else if (GameManager.Instance.IsCurrentState(GameStates.Place))
            {
                TowerManager.Instance.OnNodeClicked(node);
            }
            else if (GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                TowerManager.Instance.OnNodeClicked(node);
            }
        }

        public void OnNodeDrag(PointerEventData eventData, Node node)
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
                if (Targeting)
                {
                    _target = eventData.pointerCurrentRaycast.worldPosition;
                    CreateLink();

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
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveEdge(edge);
            }
        }

        public void OnModeChanged(int newMode)
        {
            Targeting = false;
            CurrentMode = (Mode)newMode;
        }

        public void RebuildGraph(IOManager.GameGraph newGraph)
        {
            DestroyAll();

            for (int i = 0; i < newGraph.Nodes.Count; i++)
            {
                IOManager.GameNode node = newGraph.Nodes[i];

                CreateNode(node.Position.ToVector2, node.Type);
            }

            for (int i = 0; i < newGraph.Edges.Count; i++)
            {
                IOManager.GameEdge edge = newGraph.Edges[i];
                Node a = GetHitNode<Node>(edge.a.ToVector2);
                Node b = GetHitNode<Node>(edge.b.ToVector2);

                CreateEdge(a, b);
            }
        }

        public Node GetTowerSpot(Vector2 position)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Type == Node.NodeType.TowerSpot && Tools2D.IsPositionEqual(Nodes[i].transform.position, position))
                {
                    return Nodes[i];
                }
            }

            return null;
        }

        public Vector2 NextStep(Node current, Node objective)
        {
            int target = objective.Index;
            float min = float.MaxValue;
            int best = 0;

            for (int i = 0; i < current.Edges.Count; i++)
            {
                int neighbor = current.GetNeighbor(i).Index;
                if (Distances[neighbor, target] < min)
                {
                    min = Distances[neighbor, target];
                    best = neighbor;
                }
            }

            return Nodes[best].transform.position;
        }

        public Vector2 NextStep(Vector2 current, Vector2 objective)
        {
            return NextStep(GetHitNode<Node>(current), GetHitNode<Node>(objective));
        }

        public void DestroyNodes()
        {
            for (int i = 0; i < NodesObj.childCount; i++)
            {
                RemoveNode(NodesObj.GetChild(i).GetComponent<Node>());
            }
            Nodes.Clear();
        }

        public void DestroyAll()
        {
            for (int i = 0; i < EnemySpawnsObj.childCount; i++)
            {
                Destroy(EnemySpawnsObj.GetChild(i).gameObject);
            }

            for (int i = 0; i < TowerSpotsObj.childCount; i++)
            {
                Destroy(TowerSpotsObj.GetChild(i).gameObject);
            }

            DestroyNodes();

            for (int i = 0; i < EdgesObj.childCount; i++)
            {
                Destroy(EdgesObj.GetChild(i).gameObject);
            }

            Edges.Clear();
        }

        public void OnScaleChanged()
        {
        }

        #region PathEval

        public void ReEvaluate()
        {
            Distances = new float[Nodes.Count, Nodes.Count];
            List<GraphNode> graph = new List<GraphNode>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                Node n = Nodes[i];
                n.Index = i;
                GraphNode gNode = new GraphNode(n.transform.position, float.MaxValue);
                graph.Add(gNode);
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                CalculateDistancesForNode(i, graph);
            }

            //PrintMatrix(Distances);
        }

        void CalculateDistancesForNode(int sourceIndex, List<GraphNode> graph)
        {
            List<GraphNode> unvisited = new List<GraphNode>();

            for (int i = 0; i < graph.Count; i++)
            {
                unvisited.Add(graph[i]);
                graph[i].Dist = int.MaxValue;
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                Node node = Nodes[i];

                for (int j = 0; j < node.Edges.Count; j++)
                {
                    Node neighbor = node.GetNeighbor(j);
                    for (int w = 0; w < unvisited.Count; w++)
                    {
                        if (unvisited[w].position == (Vector2)neighbor.transform.position)
                        {
                            unvisited[i].Neighbors.Add(unvisited[w]);
                        }
                    }
                }
            }

            unvisited[sourceIndex].Dist = 0;

            while (unvisited.Count > 0)
            {
                GraphNode node = unvisited[GetMinDistNode(unvisited)];
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
                Distances[sourceIndex, i] = graph[i].Dist;
            }
        }

        int GetMinDistNode(List<GraphNode> graph)
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

        void PrintMatrix(float[,] m)
        {
            string output = "";

            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    output += (int)m[i, j] + " ";
                }
                output += '\n';
            }

            Debug.Log(output);
        }

        #endregion
    }
}