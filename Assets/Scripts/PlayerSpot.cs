using UnityEngine;

namespace AnarchyBros
{
    [RequireComponent(typeof(Node))]
    public class PlayerSpot : MonoBehaviour
    {
        public Collider2D Collider;
        public bool Occupied { get { return Player != null; } }
        public Player Player;

        void Start()
        {
            Collider = GetComponent<Collider2D>();
        }
    }
}
