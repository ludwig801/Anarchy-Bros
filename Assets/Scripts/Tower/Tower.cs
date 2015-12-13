using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health;
        public Spot Objective, MoveTo, CurrentSpot;
        public Edge CurrentEdge;
        public bool IsAlive { get { return Health > 0f && gameObject.activeSelf; } }

        float _initialHealth;

        void Start()
        {
            CurrentSpot = GraphManager.Instance.GetHitSpot(transform.position);
            CurrentSpot.Tower = this;
            CurrentEdge = null;
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
            if (Tools2D.IsPositionEqual(transform.position, Objective.transform.position))
            {
                return;
            }

            if (Tools2D.IsPositionEqual(transform.position, MoveTo.transform.position))
            {
                CurrentSpot = MoveTo;
                CurrentEdge = null;
            }

            if (CurrentSpot != null)
            {
                MoveTo = GraphManager.Instance.GetBestSpot(CurrentSpot, Objective);
                CurrentEdge = GraphManager.Instance.GetHitEdge(CurrentSpot, MoveTo);
                CurrentSpot = null;
            }
            else if (CurrentEdge != null)
            {
                MoveTo = GraphManager.Instance.GetBestSpot(transform.position, CurrentEdge.A, CurrentEdge.B, Objective);
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