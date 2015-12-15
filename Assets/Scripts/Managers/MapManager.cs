using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        public Transform ObjGraph, ObjGround;
        public GameObject TowerSpotPrefab, EnemySpawnPrefab, EdgePrefab, SpotPrefab, TargetPrefab;
        public SpotTypes CurrentMode;
        public Graph Graph;
        public int EnemySpotCount, TowerSpotCount;


        GameObject _targetObj;
        Vector2 _source, _target, _mapBottomLeft, _mapTopRight;
        Edge _refEdge;
        Spot _refSource, _refTarget;
        bool _targeting;

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


            Graph = ObjGraph.GetComponent<Graph>();
            EnemySpotCount = Graph.GetSpotCountOfType(SpotTypes.EnemySpot);
            TowerSpotCount = Graph.GetSpotCountOfType(SpotTypes.EnemySpot);

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
            if (SpotAt(source) == null)
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

            if (Graph.AreNeighbors(spotA, spotB))
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

            obj.layer = LayerMask.NameToLayer("Spots");
            obj.transform.position = worldPos;

            Spot n = obj.GetComponent<Spot>();
            n.Type = type;

            Graph.AddSpot(n);
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
            obj.name = "Edge";

            Edge e = obj.GetComponent<Edge>();
            e.SetNodes(a, b);
            Graph.AddEdge(e);

            return e;
        }

        void RemoveSpot(Spot s)
        {
            Graph.RemoveSpot(s);
            EnemySpotCount = Graph.GetSpotCountOfType(SpotTypes.EnemySpot);
            TowerSpotCount = Graph.GetSpotCountOfType(SpotTypes.EnemySpot);
        }

        void RemoveEdge(Edge e)
        {
            Graph.RemoveEdge(e);
        }

        bool SpotAt(Vector2 pos, out Spot hit)
        {
            hit = Graph.SpotOverlaping(pos);

            return (hit != null);
        }

        bool EdgeAt(Vector2 pos, out Edge hit)
        {
            hit = Graph.EdgeOverlaping(pos);
            return (hit != null);
        }

        public Edge EdgeAt(Spot a, Spot b)
        {
            return Graph.EdgeConnecting(a, b);
        }

        public Spot SpotAt(Vector2 pos)
        {
            return Graph.SpotOverlaping(pos);
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

                    case SpotTypes.EnemySpot:
                        Targeting = false;
                        CreateSpot(eventData.pointerCurrentRaycast.worldPosition, SpotTypes.EnemySpot);
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

                        case SpotTypes.EnemySpot:
                            Targeting = false;
                            ReplaceSpot(spot, SpotTypes.EnemySpot);
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

                        case SpotTypes.EnemySpot:
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

            Graph.UpdateEdges();
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

                    case SpotTypes.EnemySpot:
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
                Spot a = SpotAt(edge.a.ToVector2);
                Spot b = SpotAt(edge.b.ToVector2);

                CreateEdge(a, b);
            }
        }

        public void DestroyGraph()
        {
            Graph.Clear();
        }

        public void OnGameStateChanged(GameStates newState)
        {
            Graph.OnGameStateChanged(newState);

            if (newState == GameStates.Play)
            {
                _targetObj.SetActive(false);
                Graph.ReCalculateDistances();
            }
        }

        public Spot GetRandomSpot(SpotTypes type)
        {
            int rand = int.MinValue;
            switch (type)
            {
                case SpotTypes.Connection:
                    rand = Random.Range(0, Graph.SpotCount - (EnemySpotCount + TowerSpotCount));
                    break;

                case SpotTypes.EnemySpot:
                    rand = Random.Range(0, EnemySpotCount);
                    break;

                case SpotTypes.TowerSpot:
                    rand = Random.Range(0, TowerSpotCount);
                    break;
            }

            return Graph.RandomSpot(rand, type);
        }

        public Spot NextStep(Spot current, Spot objective)
        {
            if (current == objective)
            {
                return objective;
            }

            return Graph.NextStep(current, objective);
        }

        public Spot NextStep(Vector2 currentPos, Edge e, Spot objective)
        {
            float distA = Vector2.Distance(currentPos, e.A.transform.position) + Graph.DistanceBetween(e.A, objective);
            float distB = Vector2.Distance(currentPos, e.B.transform.position) + Graph.DistanceBetween(e.B, objective);
            return (distA < distB) ? e.A : e.B;
        }

        public float DistanceBetween(Tower t, Enemy e)
        {
            if (t.Spot != null)
            {
                if (e.Spot != null)
                {
                    return Graph.DistanceBetween(t.Spot, e.Spot);
                }
                else if(e.Edge != null)
                {
                    float d1 = Vector2.Distance(e.transform.position, e.Edge.A.transform.position);
                    float d2 = Vector2.Distance(e.transform.position, e.Edge.B.transform.position);
                    return Mathf.Min(Graph.DistanceBetween(t.Spot, e.Edge.A) + d1, Graph.DistanceBetween(t.Spot, e.Edge.B) + d2);
                }
            }
            else if(t.Edge != null)
            {
                if (e.Spot != null)
                {
                    float d1 = Vector2.Distance(t.transform.position, t.Edge.A.transform.position);
                    float d2 = Vector2.Distance(t.transform.position, t.Edge.B.transform.position);
                    return Mathf.Min(Graph.DistanceBetween(t.Edge.A, e.Spot) + d1, Graph.DistanceBetween(t.Edge.B, e.Spot) + d2);
                }
                else if (e.Edge != null)
                {
                    float d1 = Vector2.Distance(t.transform.position, t.Edge.A.transform.position);
                    float d2 = Vector2.Distance(t.transform.position, t.Edge.B.transform.position);
                    float d3 = Vector2.Distance(e.transform.position, e.Edge.A.transform.position);
                    float d4 = Vector2.Distance(e.transform.position, e.Edge.B.transform.position);
                    float d5 = Mathf.Min(Graph.DistanceBetween(t.Edge.A, e.Edge.A) + d1 + d2, Graph.DistanceBetween(t.Edge.B, e.Edge.A) + d2 + d3);
                    float d6 = Mathf.Min(Graph.DistanceBetween(t.Edge.B, e.Edge.A) + d2 + d3, Graph.DistanceBetween(t.Edge.B, e.Edge.B) + d2 + d4);
                    return Mathf.Min(d5, d6); ;
                }
            }

            return float.MaxValue;
        }

        public bool OutOfMap(Vector2 position, Vector2 margin)
        {
            Vector2 objBottomLeft = position - margin;
            Vector2 objTopRight = position + margin;

            return Tools2D.NotInside(objBottomLeft, objTopRight, _mapBottomLeft, _mapTopRight);
        }
    }
}