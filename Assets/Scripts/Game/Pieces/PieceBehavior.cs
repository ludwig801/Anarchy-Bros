using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MoveBehavior))]
public class PieceBehavior : MonoBehaviour
{
    [Range(0, 100)]
    public int MaxHealth;
    [ReadOnly]
    public int Health;
    [Range(5, 10)]
    public float RotationSpeed;
    [Range(0, 2)]
    public float DeathSpeed;
    public Tags TargetTag;
    [ReadOnly]
    public bool Reciclable;
    public bool Alive { get { return (Health > 0) && gameObject.activeSelf; } }
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
    public SpriteRenderer Renderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
            return _renderer;
        }
    }

    GameManager _gameManager;
    SpriteRenderer _renderer;
    MoveBehavior _movement;
    Animator _animator;
    float _animDeathSpeed;
    bool _isAttacking;

    void Start()
    {
        _gameManager = GameManager.Instance;

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
        GetComponent<Collider2D>().enabled = false;
        Movement.CanMove = Alive;
        Movement.CurrentEdge = null;
        Movement.CurrentSpot = null;
        Movement.Target = null;
        Renderer.sortingOrder = 0;
        _gameManager.OnPieceKilled(this);

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
        GetComponent<Collider2D>().enabled = true;
    }

    public void TakeDamage(int amount)
    {
        if (Alive)
        {
            Health -= amount;
            if (Health <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }
}
