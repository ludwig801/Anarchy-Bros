using Enums;
using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public float Speed, MaxHealth, Health, DeathSpeed;
    public Transform Target, MoveTo;
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            return _animator;
        }
    }
    public Tags.Tag CollisionTag;
    public Vector2 Direction { get { return (MoveTo.position - transform.position); } }
    public bool Alive { get { return ((Health > 0f) && gameObject.activeSelf); } }
    public bool IsMoving, IsAttacking;
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

    float _deltaTime, _animDeathSpeed, _animSpeed;
    Animator _animator;
    MapManager _mapManager;
    HealthElement _healthElement;
    Collider2D _collider;

    void Start()
    {
        _mapManager = MapManager.Instance;

        _animDeathSpeed = 1f / DeathSpeed;
        _animSpeed = 0.5f * Speed; 

        Health = MaxHealth;
    }

    void Update()
    {
        Animator.SetFloat("DeathSpeed", _animDeathSpeed);
        Animator.SetFloat("Speed", _animSpeed);

        Piece piece;
        _mapManager.PieceAt(Target, CollisionTag, out piece);
        if (piece == null || (piece != null && !piece.Alive))
        {
            Target = _mapManager.NewTarget(CollisionTag);
        }

        if (Target == null)
        {
            Die();
            return;
        }

        if (IsAttacking && !IsMoving)
        {
            return;
        }

        if (Alive)
        {
            if (_mapManager.SpotAt(transform.position))
            {
                if (Tools2D.At(transform.position, MoveTo.position))
                {
                    SetIsMoving(true);
                    transform.position = MoveTo.position;
                    MoveTo = _mapManager.NextStep(transform, Target);
                }
            }
            else
            {
                SetIsMoving(true);
                MoveTo = _mapManager.NextStep(transform, Target);
            }

            if (IsMoving)
            {
                transform.rotation = Tools2D.LookAt(Direction);
                transform.position = Tools2D.MoveTowards(transform.position, MoveTo.transform.position, Time.deltaTime * Speed);
            }
        }
    }

    public void SetIsMoving(bool value)
    {
        IsMoving = value;
        Animator.SetBool("IsMoving", value);
    }

    public void SetIsAttacking(bool value)
    {
        if (value == true)
        {
            SetIsMoving(false);
        }     
        IsAttacking = value;
        Animator.SetBool("Attacking", value);
    }

    public IEnumerator Die()
    {
        Collider.enabled = false;
        Health = 0;
        Animator.SetTrigger("Die");
        Animator.SetBool("Alive", false);
        _healthElement.gameObject.SetActive(false);

        yield return new WaitForSeconds(DeathSpeed);

        gameObject.SetActive(false);
    }

    public void Live()
    {   
        Collider.enabled = true;
        Health = MaxHealth;     
        SetIsAttacking(false);
        SetIsMoving(false);
        Animator.ResetTrigger("Die");
        Animator.SetBool("Alive", Alive);
        _healthElement.gameObject.SetActive(true);
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void SetHealthElement(HealthElement elem)
    {
        _healthElement = elem;
        _healthElement.Target = this;
    }
}
