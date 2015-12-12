using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health;
        public Spot Spot, LocalObjective;
        public bool IsAlive { get { return Health > 0f; } }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (Spot == null)
            {
                Debug.LogWarning("CurrentSpot is null! [Pawn at " + transform.position + "]");
                return;
            }

            if (Spot.Tower != this)
            {
                Spot.Tower = this;
            }

            if (Mathf.Approximately(Vector2.Distance(transform.position, LocalObjective.transform.position), 0f))
            {
                if (!Tools2D.IsPositionEqual(LocalObjective.transform.position, Spot.transform.position))
                {
                    LocalObjective = GraphManager.Instance.NextStep(LocalObjective, Spot);
                }
            }

            Vector2 delta = Tools2D.Subtract(LocalObjective.transform.position, transform.position);
            float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Tools2D.MoveTowards(transform.position, LocalObjective.transform.position, Time.deltaTime * Speed);
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
            Debug.Log("The tower died");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TowerManager.Instance.OnTowerClicked(eventData, this);
        }
    }
}