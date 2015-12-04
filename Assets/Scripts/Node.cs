﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace AnarchyBros
{
    public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public enum NodeType { PlayerSpot = 0, SpawnPoint = 1, Node = 2 }

        public NodeType Type;
        public Color ColorDefault = Color.black, ColorHighlight = Color.white;
        public List<Edge> Edges;
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
        bool _highlight;

        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _sprite.color = Color.Lerp(_sprite.color, _highlight ? ColorHighlight : ColorDefault, Time.deltaTime * 20f);
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
            NodeManager.Instance.OnNodeClick(eventData, this);
        }
    }
}