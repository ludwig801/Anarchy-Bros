using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PieceBehavior))]
public class RangedBehavior : MonoBehaviour
{
    // Public vars
    public GameObject BulletPrefab;
    public int RoundsPerMinute;
    [Range(0, 25)] public int Range;
    [Range(0, 100)] public int Damage;
    [Range(5, 25)] public float BulletSpeed;
    public Transform GunPoint, RangeMarker;
    [ReadOnly] public Transform BulletCan;
    [ReadOnly] public MoveBehavior Target;
    public bool PredictiveShooting;
    [ReadOnly] public bool CanFire;
    [ReadOnly] public List<BulletBehavior> Bullets;
    // Properties
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
    // Private vars
    GameManager _gameManager;
    PieceBehavior _piece;
    Vector2 _aimAt;
    float _animAttackSpeed, _timeSinceLastFired, _fireDelay;

    void Start()
    {
        _gameManager = GameManager.Instance;

        RangeMarker.localScale = new Vector3(Range / transform.localScale.x, Range / transform.localScale.y, 1);

        _fireDelay = 60f / RoundsPerMinute;
        _animAttackSpeed = 1f / _fireDelay;
        _timeSinceLastFired = 0;
    }

    void Update()
    {
        Piece.Animator.SetFloat("AttackingSpeed", _animAttackSpeed);

        CanFire = Piece.Alive && !Piece.Movement.Moving;

        if (!CanFire)
        {
            Piece.Attacking = false;
            _timeSinceLastFired = 0;
            return;
        }

        if (!_gameManager.ProvideRangedPieceWithEnemyTarget(this))
        {
            Target = null;
            Piece.Attacking = false;
        }

        if (Target != null && CanFire)
        {
            UpdateAimAt();

            _timeSinceLastFired += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(transform.position, _aimAt), Time.deltaTime * 12f);

            if (_timeSinceLastFired >= _fireDelay)
            {
                StartCoroutine(Fire());
                _timeSinceLastFired = 0;
            }
        }
    }

    void UpdateAimAt()
    {
        _aimAt = Target.transform.position;
        if (PredictiveShooting && Target.Moving)
        {
            Vector2 targetDirection = Target.Direction;
            if (targetDirection.magnitude < 0.25f)
            {
                return;
            }

            Vector2 targetPos = Target.transform.position;
            Vector2 gunPosition = GunPoint.position;
            Vector2 targetVelocity = Target.Speed * targetDirection.normalized;

            // Inteligent shooting: using the enemy's know velocity and the bullet's speed, predict where to should we fire.
            float k = targetVelocity.x + targetVelocity.y;
            float w = (targetPos.x - gunPosition.x) + (targetPos.y - gunPosition.y);
            float h = 2 * (targetPos.x * (targetPos.y - gunPosition.y) - gunPosition.x * (targetPos.y - gunPosition.y));
            float a = (k * k) + (2 * targetVelocity.x * targetVelocity.y) - (BulletSpeed * BulletSpeed);
            float b = 2 * (k * w - targetVelocity.x * (targetPos.y - gunPosition.y) - targetVelocity.y * (targetPos.x - gunPosition.x));
            float c = w * w - h;

            float t1 = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

            if (t1 > 0 || t2 > 0)
            {
                float t = 0;

                if (t1 > 0)
                {
                    if (t2 > 0)
                    {
                        t = Mathf.Min(t1, t2);
                    }
                    else
                    {
                        t = t1;
                    }
                }
                else
                {
                    t = t2;
                }

                Vector2 P = new Vector2(targetPos.x + targetVelocity.x * t, targetPos.y + targetVelocity.y * t);
                _aimAt = P;
            }
        }
    }

    IEnumerator Fire()
    {
        BulletBehavior bullet = GetBullet();
        bullet.transform.position = GunPoint.position;
        bullet.transform.rotation = Tools2D.LookAt(GunPoint.position, _aimAt);
        bullet.Direction = (Tools2D.Subtract(_aimAt, GunPoint.position)).normalized;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.transform.parent = BulletCan;
        bullet.gameObject.SetActive(true);
        Piece.Attacking = true;

        yield return new WaitForSeconds(0.9f * _fireDelay);

        Piece.Attacking = false;
    }

    BulletBehavior GetBullet()
    {
        for (int i = 0; i < Bullets.Count; i++)
        {
            if (!Bullets[i].gameObject.activeSelf)
            {
                return Bullets[i];
            }
        }

        GameObject obj = Instantiate(BulletPrefab);
        obj.transform.rotation = Quaternion.identity;
        obj.name = "Bullet";

        BulletBehavior b = obj.GetComponent<BulletBehavior>();
        Bullets.Add(b);

        return b;
    }
}