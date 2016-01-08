﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Piece))]
public class MeleePiece : MonoBehaviour
{
    public GameObject WeaponPrefab;
    public Piece Target;

    GameManager _gameController;
    MeleeWeapon _meleeWeapon;
    Piece _piece;
    float _animAttackSpeed;

    void Start()
    {
        _gameController = GameManager.Instance;

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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != Tags.GetStringTag(_piece.TargetTag)) return;

        Piece otherPiece = other.GetComponent<Piece>();
        _piece.IsAttacking = true;
        if (Target == null || (Target != null && !Target.Alive))
        {
            Target = otherPiece;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        while (Target != null && Target.Alive)
        {
            yield return new WaitForSeconds(0.5f * _meleeWeapon.AttackDelay);

            if (_piece.Alive)
            {
                if (Target != null)
                {
                    Target.TakeDamage(_meleeWeapon.Damage);
                    _gameController.CreateWound(Target.transform, (Target.transform.position - transform.position).normalized);
                }
            }
            else
            {
                yield break;
            }

            yield return new WaitForSeconds(0.5f * _meleeWeapon.AttackDelay);
        }

        _piece.IsAttacking = false;
        Target = null; 
    }
}
