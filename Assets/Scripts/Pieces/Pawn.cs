using UnityEngine;
using UnityEngine.EventSystems;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class Pawn : MonoBehaviour, IKillable, IPointerClickHandler
    {
        public float Speed, Health;
        public Node CurrentSpot;
        public bool IsAlive { get { return Health > 0f; } }

        Vector3 _moveTo;

        void Update()
        {
            if (!GameManager.Instance.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (CurrentSpot == null)
            {
                Debug.LogWarning("CurrentSpot is null! [Pawn at " + transform.position + "]");
                return;
            }
 
            if (CurrentSpot.Pawn != this)
            {
                CurrentSpot.Pawn = this;
            }
            _moveTo = new Vector3(CurrentSpot.transform.position.x, CurrentSpot.transform.position.y, _moveTo.z);

            if (!Mathf.Approximately(Vector3.Distance(transform.position, _moveTo), 0f))
            {
                transform.position = Vector3.MoveTowards(transform.position, _moveTo, Time.deltaTime * Speed);
            }
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
            Debug.Log("The pawn died");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PawnManager.Instance.OnPawnClicked(this);
        }
    }
}