using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PieceBehavior))]
public class MeleePiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
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

    GameManager _gameManager;
    MeleeWeapon _meleeWeapon;
    PieceBehavior _piece;
    float _animAttackSpeed;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _meleeWeapon = Instantiate(WeaponPrefab).GetComponent<MeleeWeapon>();
        _meleeWeapon.transform.position = transform.position;
        _meleeWeapon.transform.parent = transform;
        _meleeWeapon.name = WeaponPrefab.name;

        _animAttackSpeed = 1f / _meleeWeapon.AttackDelay;    
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
                Tools2D.LookAt(Piece.transform.position, Target.transform.position), Time.deltaTime * 10f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Piece.tag)
        {
            // Prevent movement from one end
        }
        else if (other.tag == Piece.TargetTag.ToString())
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
        if (other.tag == Piece.tag)
        {
            // Conceded movement from one end
        }
        else if (other.tag == Piece.TargetTag.ToString())
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
            Target.TakeDamage(_meleeWeapon.Damage);
            _gameManager.Wounds.CreateWound(Piece.transform, Target.transform);
        }
    }
}
