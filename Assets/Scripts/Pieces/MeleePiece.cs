using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Piece))]
public class MeleePiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
    public Piece Target;
    public Piece Piece
    {
        get
        {
            if (_piece == null)
            {
                _piece = GetComponent<Piece>();
            }
            return _piece;
        }
    }

    GameManager _gameManager;
    MeleeWeapon _meleeWeapon;
    Piece _piece;
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

        if (Target != null && !Target.Alive)
        {
            Target = null;
            Piece.Attacking = false;
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
            Piece otherPiece = other.GetComponent<Piece>();  
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
            Piece otherPiece = other.GetComponent<Piece>();
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
            _gameManager.CreateWound(Target.Movement, (Piece.transform.position - Target.transform.position).normalized);
        }
    }
}
