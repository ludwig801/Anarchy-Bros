using UnityEngine;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class Edge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Color ColorDefault = Color.black, ColorHighlight = Color.white;

        public Node A { get; private set; }
        public Node B { get; private set; }
        public Collider2D Collider
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider2D>();
                }

                return _collider;
            }
        }

        Collider2D _collider;
        SpriteRenderer _sprite;
        bool _dirty, _highlight;

        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            UpdateSprite();
        }

        void Update()
        {
            _sprite.color = Color.Lerp(_sprite.color, _highlight ? ColorHighlight : ColorDefault, Time.deltaTime * 20f);

            if (_dirty)
            { 
                UpdateSprite();
                _dirty = false;
            }
        }

        void UpdateSprite()
        {
            if (A == null || B == null)
            {
                return;
            }
            Vector2 delta = (B.transform.position - A.transform.position);
            transform.position = (Vector2)A.transform.position + (0.5f * delta);
            transform.localScale = new Vector3(transform.localScale.x, delta.magnitude, transform.localScale.z);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public Node GetNeighbor(Node n)
        {
            return (n == A) ? B : (n == B) ? A : null;
        }

        public bool HasNode(Node n)
        {
            return (n == A) || (n == B);
        }

        public void SetNodes(Node a, Node b)
        {
            A = a;
            B = b;
            _dirty = true;
        }

        public void SetNodesPositions(Vector2 posA, Vector2 posB)
        {
            Vector2 delta = posB - posA;
            transform.position = posA + (0.5f * delta);
            transform.localScale = new Vector3(transform.localScale.x, delta.magnitude, transform.localScale.z);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _highlight = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _highlight = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NodeManager.Instance.OnEdgeClick(eventData, this);
        }
    }
}