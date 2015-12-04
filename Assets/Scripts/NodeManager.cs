using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class NodeManager : MonoBehaviour
    {
        public static NodeManager Instance { get; private set; }

        public GameObject EdgePrefab;
        public GameObject NodePrefab;
        public List<Node> Nodes;
        public List<Edge> Edges;

        Vector2 _origin, _target;
        bool _targeting;
        Edge _refEdge, _hitEdge;
        Node _refNode, _hitNode;

        bool Targeting
        {
            get { return _targeting; }

            set
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
            GetAllNodes();

            GameObject instance = Instantiate(EdgePrefab);
            instance.name = "Reference Edge";
            instance.GetComponent<Collider2D>().enabled = false;
            _refEdge = instance.GetComponent<Edge>();

            instance = Instantiate(NodePrefab);
            instance.name = "Reference Node";
            instance.GetComponent<Collider2D>().enabled = false;
            _refNode = instance.GetComponent<Node>();

            Targeting = false;
        }

        void Update()
        {
            if (Targeting)
            {
                _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _refEdge.SetNodesPositions(_origin, _target);
                _refNode.transform.position = _target;
            }

            //if (GameManager.Instance.CurrentState == GameManager.State.Editing)
            //{
            //    if (_hitEdge != null)
            //    {
            //        _hitEdge.Highlight = false;
            //        _hitEdge = null;
            //    }

            //    if (_hitNode != null)
            //    {
            //        _hitNode.Highlight = false;
            //        _hitNode = null;
            //    }

            //    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //    if (GetHitNode(mousePos, out _hitNode))
            //    {
            //        _hitNode.Highlight = true;
            //    }
            //    else
            //    {
            //        GetHitEdge(mousePos, out _hitEdge);
            //    }

            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        if (!Targeting)
            //        {
            //            Targeting = true;

            //            _origin = (_hitNode == null) ? mousePos : (Vector2)_hitNode.transform.position;
            //            _refEdge.SetNodesPositions(_origin, _origin);
            //        }
            //        else
            //        {
            //            Targeting = false;
            //            _target = (_hitNode == null) ? mousePos : (Vector2)_hitNode.transform.position;
            //            CreateLink();
            //        }
            //    }
            //    else if (Input.GetMouseButtonDown(1))
            //    {
            //        if (_hitNode != null)
            //        {
            //            RemoveNode(_hitNode);
            //        }
            //        else if (_hitEdge != null)
            //        {
            //            RemoveEdge(_hitEdge);
            //        }
            //        Targeting = false;
            //    }
            //    else
            //    {
            //        _target = (_hitNode == null) ? mousePos : (Vector2)_hitNode.transform.position;

            //        if (_hitEdge != null)
            //        {
            //            _hitEdge.Highlight = true;
            //        }

            //        _refEdge.SetNodesPositions(_origin, _target);
            //    }
            //}
        }

        public void CreateLink()
        {
            Node nodeA, nodeB;
            Edge hitEdge;

            if (!GetHitNode(_origin, out nodeA))
            {
                nodeA = CreateNode(_origin);
                if (GetHitEdge(_origin, out hitEdge))
                {
                    SplitEdge(hitEdge, nodeA);
                }
            }

            if (!GetHitNode(_target, out nodeB))
            {
                nodeB = CreateNode(_target);
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

        public Node CreateNode(Vector2 worldPos)
        {
            GameObject obj = Instantiate(NodePrefab);
            obj.name = "Node";
            obj.layer = LayerMask.NameToLayer("Spots");
            obj.transform.position = worldPos;
            obj.transform.parent = transform;

            Node n = obj.GetComponent<Node>();
            n.Type = Node.NodeType.Node;

            Nodes.Add(n);
            return n;
        }

        public Edge CreateEdge(Node a, Node b)
        {
            Edge e = Instantiate(EdgePrefab).GetComponent<Edge>();
            e.name = "Edge";
            e.transform.parent = transform;
            e.SetNodes(a, b);
            Edges.Add(e);

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
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveNode(n);
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

            Transform _spawnPoints = GameObject.FindGameObjectWithTag("Spawn Points").transform;
            Transform _playerSpots = GameObject.FindGameObjectWithTag("Player Spots").transform;

            for (int i = 0; i < _playerSpots.childCount; i++)
            {
                Node n = _playerSpots.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.PlayerSpot;

                if (n != null)
                {
                    Nodes.Add(n);
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

            for (int i = 0; i < transform.childCount; i++)
            {
                Node n = transform.GetChild(i).GetComponent<Node>();
                n.Type = Node.NodeType.Node;

                if (n != null)
                {
                    Nodes.Add(n);
                }
            }
        }
    }
}