﻿using UnityEngine;

[RequireComponent(typeof(Piece))]
public class RangedPiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
    public Transform GunPoint;

    RangeWeapon _rangeWeapon;
    Piece _piece;
    float _animAttackSpeed;

    void Start()
    {
        _piece = GetComponent<Piece>();

        _rangeWeapon = Instantiate(WeaponPrefab).GetComponent<RangeWeapon>();
        _rangeWeapon.transform.position = transform.position;
        _rangeWeapon.transform.parent = transform;
        _rangeWeapon.name = WeaponPrefab.name;
        _rangeWeapon.BulletCan = TowerManager.Instance.BulletCan;
        _rangeWeapon.GunPoint = GunPoint;

        _animAttackSpeed = 2f / _rangeWeapon.FireDelay;
    }

    void Update()
    {
        _piece.Animator.SetFloat("AttackingSpeed", _animAttackSpeed);
        _piece.Animator.SetBool("Attacking", _rangeWeapon.Firing);
        _rangeWeapon.CanFire = !_piece.IsMoving && _piece.Alive;
        if (!_piece.IsMoving)
        {
            _rangeWeapon.Target = TowerManager.Instance.GetNearestEnemy(transform);
            transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(GunPoint.position, _rangeWeapon.AimAt), Time.deltaTime * 5f);
        }
    }
}