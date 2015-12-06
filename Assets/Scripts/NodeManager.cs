using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using AnarchyBros.Strings;

namespace AnarchyBros
{
    public class NodeManager : MonoBehaviour
    {
        public static NodeManager Instance { get; private set; }

        public GameObject PlayerSpotPrefab, SpawnPointPrefab, EdgePrefab, NodePrefab;
        public List<Node> Nodes;
        public List<Edge> Edges;
        public List<PlayerSpot> PlayerSpots;

        Transform _playerSpots, _spawnPoints, _nodes, _edges;
        Vector2 _origin, _target;
        bool _targeting;
        Edge _refEdge, _hitEdge;
        Node _refNode, _hitNode;

        public bool Targeting
        {
            get { return _targeting; }

            private set
            {
                _targeting = value;
                _refEdge.gameObject.SetActive(value);
                _refNode.gameObject.SetActive(value);
            }
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameObject instance = Instantiate(EdgePrefab);
            instance.name = "Reference Edge";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refEdge = instance.GetComponent<Edge>();

            instance = Instantiate(NodePrefab);
            instance.name = "Reference Node";
            instance.GetComponent<Collider2D>().enabled = false;
            instance.transform.parent = transform;
            _refNode = instance.GetComponent<Node>();

            _playerSpots = GameObject.FindGameObjectWithTag("Player Spots").transform;
            _spawnPoints = GameObject.FindGameObjectWithTag("Spawn Points").transform;
            _nodes = GameObject.FindGameObjectWithTag("Nodes").transform;
            _edges = GameObject.FindGameObjectWithTag("Edges").transform;

            GetAllNodes();

            Targeting = false;
        }

        void Update()
        {
            if (GameManager.Instance.IsEdit)
            {
                if (Targeting)
                {
                    _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _refEdge.SetNodesPositions(_origin, _target);
                    _refNode.transform.position = _target;
                }
            }
            else
            {
                Targeting = false;
            }
        }

        public void CreateLink()
        {
            Node nodeA, nodeB;
            Edge hitEdge;

            if (!GetHitNode(_origin, out nodeA))
            {
                nodeA = CreateNode(_origin, Node.NodeType.Node);
                if (GetHitEdge(_origin, out hitEdge))
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
                    obj.transform.parent = _nodes;
                    break;

                case Node.NodeType.PlayerSpot:
                    obj = Instantiate(PlayerSpotPrefab);
                    obj.name = "Player Spot";
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = _playerSpots;
                    break;

                case Node.NodeType.SpawnPoint:
                    obj = Instantiate(SpawnPointPrefab);
                    obj.name = "Spawn Point";
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = _spawnPoints;
                    break;

                default:
                    obj = Instantiate(NodePrefab);
                    obj.name = "Node";
                    obj.layer = LayerMask.NameToLayer("Spots");
                    obj.transform.position = worldPos;
                    obj.transform.parent = _nodes;
                    break;
            }

            Node n = obj.GetComponent<Node>();
            n.Type = type;

            Nodes.Add(n);
            return n;
        }

        public Edge CreateEdge(Node a, Node b)
        {
            Edge e = Instantiate(EdgePrefab).GetComponent<Edge>();
            e.name = "Edge";
            e.transform.parent = _edges;
            e.SetNodes(a, b);
            Edges.Add(e);

            a.Edges.Add(e);
            b.Edges.Add(e);

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
                if (Targeting)
                {
                    _target = eventData.pointerCurrentRaycast.worldPosition;
                    CreateLink();

                    // For continuos targeting
                    _origin = _target;
                    // For one time targeting
                    // Targeting = false;
                }
                else
                {
                    Targeting = true;
                    _origin = eventData.pointerCurrentRaycast.worldPosition;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Targeting = false;
            }
        }

        public void OnNodeClick(PointerEventData eventData, Node n)
        {
            if (GameManager.Instance.IsEdit)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (Targeting)
                    {
                        _target = n.transform.position;
                        CreateLink();

                        // For continuos targeting
                        _origin = _target;
                        // For one time targeting
                        // Targeting = false;
                    }
                    else
                    {
                        Targeting = true;
                        _origin = n.transform.position;
                        _target = _origin;
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    RemoveNode(n);
                }
            }
            else if (GameManager.Instance.IsPlay)
            {
                PlayerManager.Instance.OnNodeClicked(n);
            }
        }

        public void OnEdgeClick(PointerEventData eventData, Edge e)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Targeting)
                {
                    _target = eventData.pointerCurrentRaycast.worldPosition;
                    CreateLink();

                    // For continuos targeting
                    _origin = _target;
                    // For one time targeting
                    // Targeting = false;
                }
                else
                {
                    Targeting = true;
                    _origin = eventData.pointerCurrentRaycast.worldPosition;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveEdge(e);
            }
        }

        static Transform NextStep(Node current, Node objective)
        {
            // TODO : make algorithm to get next step
            return null;
        }

        static List<Transform> GetPath(Node begin, Node objective)
        {
            // TODO
            return null;
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

            if (PlayerSpots == null)
            {
                PlayerSpots = new List<PlayerSpot>();
            }
            else
            {
                PlayerSpots.Clear();
            }

            for (int i = 0; i < _playerSpots.childCount; i++)
            {
                Node n = _playerSpots.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.PlayerSpot;

                if (n != null)
                {
                    Nodes.Add(n);
                    PlayerSpots.Add(n.GetComponent<PlayerSpot>());
                }
            }

            for (int i = 0; i < _spawnPoints.childCount; i++)
            {
                Node n = _spawnPoints.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.SpawnPoint;

                if (n != null)
                {
                    Nodes.Add(n);
                }
            }

            for (int i = 0; i < _nodes.childCount; i++)
            {
                Node n = _nodes.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.Node;

                Nodes.Add(n);
            }

            for (int i = 0; i < _edges.childCount; i++)
            {
                Edge e = _edges.GetChild(i).GetComponent<Edge>();

                Edges.Add(e);
            }
        }

        public PlayerSpot GetPlayerSpot(Vector2 pos)
        {
            for (int i = 0; i < PlayerSpots.Count; i++)
            {
                if ((Vector2)PlayerSpots[i].transform.position == pos)
                {
                    return PlayerSpots[i];
                }
            }

            return null;
        }

        public void RebuildGraph(IOManager.GameGraph newGraph)
        {
            DestroyGraph();

            for (int i = 0; i < newGraph.Nodes.Count; i++)
            {
                IOManager.GameNode node = newGraph.Nodes[i];

                Node n = CreateNode(node.Position.ToVector2, node.Type);

                if (n.Type == Node.NodeType.PlayerSpot)
                {
                    PlayerSpots.Add(n.GetComponent<PlayerSpot>());
                }
            }

            for (int i = 0; i < newGraph.Edges.Count; i++)
            {
                IOManager.GameEdge edge = newGraph.Edges[i];
                Node a = null, b = null;
                GetHitNode(edge.a.ToVector2, out a);
                GetHitNode(edge.b.ToVector2, out b);

                CreateEdge(a, b);
            }
        }

        void DestroyGraph()
        {
            for (int i = 0; i < _spawnPoints.childCount; i++)
            {
                Destroy(_spawnPoints.GetChild(i).gameObject);
            }

            for (int i = 0; i < _playerSpots.childCount; i++)
            {
                Destroy(_playerSpots.GetChild(i).gameObject);
            }

            for (int i = 0; i < _nodes.childCount; i++)
            {
                Destroy(_nodes.GetChild(i).gameObject);
            }

            for (int i = 0; i < _edges.childCount; i++)
            {
                Destroy(_edges.GetChild(i).gameObject);
            }

            PlayerSpots.Clear();
            Nodes.Clear();
            Edges.Clear();
        }
    }
}