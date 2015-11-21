using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    [RequireComponent(typeof(Node))]
    public class PlayerSpot : MonoBehaviour
    {
        public Collider2D Collider;
        public bool Occupied;

        void Start()
        {
            Collider = GetComponent<Collider2D>();
        }
    }
}
