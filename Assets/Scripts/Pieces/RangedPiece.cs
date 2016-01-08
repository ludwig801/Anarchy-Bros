using UnityEngine;

[RequireComponent(typeof(Piece))]
public class RangedPiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
    public Transform GunPoint;
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
    RangeWeapon _rangeWeapon;
    Piece _piece;
    float _animAttackSpeed;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _rangeWeapon = Instantiate(WeaponPrefab).GetComponent<RangeWeapon>();
        _rangeWeapon.transform.position = transform.position;
        _rangeWeapon.transform.parent = transform;
        _rangeWeapon.name = WeaponPrefab.name;
        _rangeWeapon.BulletCan = _gameManager.Towers.ObjBullets;
        _rangeWeapon.GunPoint = GunPoint;

        _animAttackSpeed = 2f / _rangeWeapon.FireDelay;
    }

    void Update()
    {
        Piece.Animator.SetFloat("AttackingSpeed", _animAttackSpeed);
        Piece.Animator.SetBool("Attacking", _rangeWeapon.Firing);
        _rangeWeapon.CanFire = !_piece.Movement.IsMoving && _piece.Alive;
        if (_rangeWeapon.CanFire)
        {
            //if(_gameManager.GetTarget(this, out _rangeWeapon.Target))
            //transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(GunPoint.position, _rangeWeapon.AimAt), Time.deltaTime * 5f);
        }
    }
}