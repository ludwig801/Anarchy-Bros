﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Spot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Color ColorInEditor, ColorInGame;
        public int Index { get; set; }
        public SpotTypes Type;
        public Tower Tower;
        public bool Occupied { get { return Tower != null; } }
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

        MapManager _mapManager;
        SpriteRenderer _renderer;
        Color _colorTo;
        Collider2D _collider;
        bool _justScrolled;

        void Start()
        {
            _mapManager = MapManager.Instance;
            _renderer = GetComponent<SpriteRenderer>();
            OnGameStateChanged(GameManager.Instance.CurrentState);
        }

        void Update()
        {
            _renderer.color = Color.Lerp(_renderer.color, _colorTo, Time.deltaTime * 8f);
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
            return Edges[index].Neighbor(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_justScrolled)
            {
                _mapManager.OnSpotClick(eventData, this);
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

            _mapManager.OnSpotDrag(eventData, this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _mapManager.OnSpotDrag(eventData, this);
        }

        public void OnGameStateChanged(GameStates newState)
        {
            switch (newState)
            {
                case GameStates.Edit:
                    _colorTo = ColorInEditor;
                    break;

                case GameStates.Place:
                    _colorTo = ColorInEditor;
                    break;

                case GameStates.Play:
                    _colorTo = ColorInGame;
                    break;
            }
        }
    }
}