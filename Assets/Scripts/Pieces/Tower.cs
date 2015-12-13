using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Tower : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health;
        public Spot Objective, LocalObjective, LastSpot;
        public bool IsAlive { get { return Health > 0f; } }

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (Objective == null)
            {
                Debug.LogWarning("CurrentSpot is null! [Pawn at " + transform.position + "]");
                return;
            }

            if (LastSpot == null)
            {
                LastSpot = LocalObjective;
            }

            if (Objective.Tower != this)
            {
                Objective.Tower = this;
            }

            if (Mathf.Approximately(Vector2.Distance(transform.position, LocalObjective.transform.position), 0f))
            {
                LastSpot = LocalObjective;

                if (!Tools2D.IsPositionEqual(LocalObjective.transform.position, Objective.transform.position))
                {
                    LocalObjective = GraphManager.Instance.NextStep(LocalObjective, Objective);
                }
                else
                {
                    LocalObjective = Objective;
                }
            }
            else
            {
                LocalObjective = GraphManager.Instance.NextStep(LastSpot, Objective);
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