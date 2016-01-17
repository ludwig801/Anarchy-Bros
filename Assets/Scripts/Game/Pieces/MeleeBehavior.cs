using UnityEngine;

[RequireComponent(typeof(PieceBehavior))]
public class MeleeBehavior : MonoBehaviour
{
    public PieceBehavior Target;
    public PieceBehavior Piece
    {
        get
        {
            if (_piece == null)
            {
                _piece = GetComponent<PieceBehavior>();
            }
            return _piece;
        }
    }
    public float AttackDelay;
    public int Damage;

    GameManager _gameManager;
    PieceBehavior _piece;
    float _animAttackSpeed;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _animAttackSpeed = 1f / AttackDelay;    
    }

    void Update()
    {
        Piece.Animator.SetFloat("AttackingSpeed", _animAttackSpeed);

        if (!Piece.Alive || (Target != null && !Target.Alive))
        {
            Target = null;
            Piece.Attacking = false;
        }
        else if (Target != null && Target.Alive)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Tools2D.LookAt(Piece.transform.position, Target.transform.position), Time.deltaTime * Piece.RotationSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Piece.TargetTag.ToString())
        {
            PieceBehavior otherPiece = other.GetComponent<PieceBehavior>();  
            if (Target == null)
            {
                Piece.Attacking = true;
                Target = otherPiece;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == Piece.TargetTag.ToString())
        {
            PieceBehavior otherPiece = other.GetComponent<PieceBehavior>();
            if (Target == otherPiece)
            {
                Piece.Attacking = false;
                Target = null;
            }
        }
    }

    public void Attack()
    {
        if (Target != null)
        {
            Target.TakeDamage(Damage);
            _gameManager.Wounds.CreateWound(Piece.transform, Target.transform);
        }
    }
}
