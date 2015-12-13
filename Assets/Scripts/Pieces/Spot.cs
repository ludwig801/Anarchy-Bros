using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Spot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public int Index;
        public SpotTypes Type;
        public Tower Tower;
        public bool Occupied { get { return Tower != null; } }
        public List<Edge> Edges;
        public Collider2D Collider
        {
            get
            {
                _col = (_col == null) ? GetComponent<Collider2D>() : _col;
                return _col;
            }
        }

        Collider2D _col;
        bool _justScrolled;

        void Start()
        {
            Tower = null;
        }

        void Update()
        {
            if (Edges == null)
            {
                Debug.Log("null edges");
            }
        }

        public void AddEdge(Edge e)
        {
            Edges.Add(e);
        }

        public void RemoveEdge(Edge e)
        {
            Edges.Remove(e);
        }

        public Spot GetNeighbor(int index)
        {
            return Edges[index].GetNeighbor(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_justScrolled)
            {
                GraphManager.Instance.OnSpotClick(eventData, this);
            }
            _justScrolled = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _justScrolled = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 newPos = Camera.main.ScreenToWorldPoint(eventData.position);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            GraphManager.Instance.OnSpotDrag(eventData, this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GraphManager.Instance.OnSpotDrag(eventData, this);
        }
    }
}