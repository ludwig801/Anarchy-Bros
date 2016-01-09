using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MoveBehavior))]
public class PieceBehavior : MonoBehaviour
{
    public int MaxHealth, Health;
    public float DeathSpeed;
    public Tags.Tag TargetTag;
    public bool Alive { get { return (Health > 0) && gameObject.activeSelf; } }
    public bool Reciclable;
    public bool Attacking
    {
        get
        {
            return _isAttacking;
        }

        set
        {
            _isAttacking = value;
            Animator.SetBool("Attacking", value);
            Movement.CanMove = !value && Alive;
        }
    }
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
    public MoveBehavior Movement
    {
        get
        {
            if (_movement == null)
            {
                _movement = GetComponent<MoveBehavior>();
            }
            return _movement;
        }
    }

    MoveBehavior _movement;
    float _deltaTime, _animDeathSpeed;
    Animator _animator;
    HealthElement _healthElement;
    bool _isAttacking;

    void Start()
    {
        _animDeathSpeed = 1f / DeathSpeed;

        Health = MaxHealth;
    }

    void Update()
    {
        Animator.SetFloat("DeathSpeed", _animDeathSpeed);
    }

    public IEnumerator Die()
    {
        Health = 0;
        Animator.SetTrigger("Die");
        Animator.SetBool("Alive", Alive);
        Movement.CanMove = Alive;
        //_healthElement.gameObject.SetActive(false);

        yield return new WaitForSeconds(DeathSpeed);

        Reciclable = true;
    }

    public void Live()
    {   
        Health = MaxHealth;
        Attacking = false;
        Animator.ResetTrigger("Die");
        Animator.SetBool("Alive", Alive);
        Movement.CanMove = Alive;
        Reciclable = false;
        //_healthElement.gameObject.SetActive(true);
    }

    public void TakeDamage(int amount)
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
