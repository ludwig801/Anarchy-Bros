using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health;
        public Spot Objective, MoveTo, CurrentSpot;
        public Edge CurrenteEdge;
        public bool IsAlive { get { return Health > 0f; } }

        float _initialHealth;

        void Start()
        {
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrentSpot.Tower = this;
            CurrenteEdge = null;
            transform.position = MoveTo.transform.position;
            _initialHealth = Health;
        }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            MoveTowardsObjective();
        }

        void MoveTowardsObjective()
        {
            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                CurrentSpot = MoveTo;
                CurrenteEdge = null;
            }

            if (CurrentSpot == Objective)
            {
                return;
            }

            if (CurrentSpot != null)
            {
                MoveTo = GraphManager.Instance.GetNextSpot(CurrentSpot, Objective);
                CurrenteEdge = GraphManager.Instance.GetHitEdge(CurrentSpot, MoveTo);
                CurrentSpot = null;
            }

            if (CurrenteEdge != null)
            {
                MoveTo = GraphManager.Instance.GetNextSpot(CurrenteEdge.A, CurrenteEdge.B, Objective);
            }

            Vector2 delta = Tools2D.Subtract(MoveTo.transform.position, transform.position);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
        }

        public void TakeDamage(float amount)
        {
            Health -= amount;

            if (!IsAlive)
            {
                Kill();
            }
        }

        public void Kill()
        {
            gameObject.SetActive(false);
            Health = _initialHealth;
            TowerManager.Instance.OnTowerKill();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TowerManager.Instance.OnTowerClicked(eventData, this);
        }
    }
}