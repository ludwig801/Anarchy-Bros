﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class Edge : MonoBehaviour, IPointerClickHandler
    {
        public Node A { get; private set; }
        public Node B { get; private set; }
        public Collider2D Collider { get; private set; }

        void Start()
        {
            Collider = GetComponent<Collider2D>();
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

            SetVertices(A.transform.position, B.transform.position);
        }

        public void SetVertices(Vector2 posA, Vector2 posB)
        {
            Vector2 delta = posB - posA;
            transform.position = posA + (0.5f * delta);
            transform.localScale = new Vector3(transform.localScale.x, delta.magnitude, transform.localScale.z);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NodeManager.Instance.OnEdgeClick(eventData, this);
        }

        public void ReEvaluate()
        {
            SetVertices(A.transform.position, B.transform.position);
        }
    }
}