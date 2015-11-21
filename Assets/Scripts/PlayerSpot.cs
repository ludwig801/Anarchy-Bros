using UnityEngine;

namespace AnarchyBros
{
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
