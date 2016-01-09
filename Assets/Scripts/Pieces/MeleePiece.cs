using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Piece))]
public class MeleePiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
    public Piece Target;

    GameManager _gameManager;
    MeleeWeapon _meleeWeapon;
    Piece _piece;
    float _animAttackSpeed;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _piece = GetComponent<Piece>();

        _meleeWeapon = Instantiate(WeaponPrefab).GetComponent<MeleeWeapon>();
        _meleeWeapon.transform.position = transform.position;
        _meleeWeapon.transform.parent = transform;
        _meleeWeapon.name = WeaponPrefab.name;

        _animAttackSpeed = 1f / _meleeWeapon.AttackDelay;    
    }

    void Update()
    {
        _piece.Animator.SetFloat("AttackingSpeed", _animAttackSpeed);

        if (Target != null && !Target.Alive)
        {
            Target = null;
            _piece.Attacking = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _piece.tag)
        {
            // Prevent movement from one end
        }
        else if (other.tag == _piece.TargetTag.ToString())
        {
            Piece otherPiece = other.GetComponent<Piece>();  
            if (Target == null)
            {
                _piece.Attacking = true;
                Target = otherPiece;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == _piece.tag)
        {
            // Conceded movement from one end
        }
        else if (other.tag == _piece.TargetTag.ToString())
        {
            Piece otherPiece = other.GetComponent<Piece>();
            if (Target == otherPiece)
            {
                _piece.Attacking = false;
                Target = null;
            }
        }
    }

    public void Attack()
    {
        if (Target != null)
        {
            Target.TakeDamage(_meleeWeapon.Damage);
            _gameManager.CreateWound(Target.Movement, (_piece.transform.position - Target.transform.position).normalized);
        }
    }
}
