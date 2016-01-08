using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RangeWeapon : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Transform GunPoint, BulletCan;
    public int RoundsPerMinute, Damage;
    public float BulletSpeed;
    public bool PredictiveShooting, CanFire;
    public Piece Target;
    public List<Bullet> Bullets;
    public Vector2 AimAt;
    public bool Firing;
    public float FireDelay
    {
        get
        {
            if (_fireDelay == float.MaxValue)
            {
                _fireDelay = 60f / RoundsPerMinute;
            }
            return _fireDelay;
        }
    }

    float _fireDelay = float.MaxValue, _deltaTime;

    void Start()
    {
        _deltaTime = 0;

        CanFire = false;
    }

    void Update()
    {
        if (!CanFire)
        {
            return;
        }

        UpdateTarget();

        transform.rotation = Tools2D.LookAt(transform.position, AimAt);

        if (Target != null && Target.Alive)
        {
            _deltaTime += Time.deltaTime;

            if (_deltaTime >= FireDelay)
            {
                StartCoroutine(Fire());

                _deltaTime = 0;
            }
        }
    }

    void UpdateTarget()
    {
        if (Target == null)
        {
            return;
        }

        AimAt = Target.transform.position;
        if (PredictiveShooting && Target.Movement.IsMoving)
        {
            Vector2 dir = Target.Movement.Direction;
            if (dir.magnitude < 1f)
            {
                return;
            }

            Vector2 E = Target.transform.position;
            Vector2 B = GunPoint.position;
            Vector2 vE = Target.Movement.Speed * dir.normalized;

            // Inteligent shooting: using the enemy's know velocity and the bullet's speed, predict where to should we fire.
            float k = vE.x + vE.y;
            float w = (E.x - B.x) + (E.y - B.y);
            float h = 2 * (E.x * (E.y - B.y) - B.x * (E.y - B.y));
            float a = (k * k) + (2 * vE.x * vE.y) - (BulletSpeed * BulletSpeed);
            float b = 2 * (k * w - vE.x * (E.y - B.y) - vE.y * (E.x - B.x));
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

                Vector2 P = new Vector2(E.x + vE.x * t, E.y + vE.y * t);
                AimAt = P;
            }
        }
    }

    IEnumerator Fire()
    {
        Bullet bullet = GetBullet();
        bullet.transform.position = GunPoint.position;
        bullet.transform.rotation = Tools2D.LookAt(GunPoint.position, AimAt);
        bullet.Direction = (Tools2D.Subtract(AimAt, GunPoint.position)).normalized;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.transform.parent = BulletCan;
        bullet.gameObject.SetActive(true);
        bullet.Fire = true;
        Firing = true;

        yield return new WaitForSeconds(0.9f * FireDelay);

        Firing = false;
    }

    Bullet GetBullet()
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

        Bullet b = obj.GetComponent<Bullet>();
        Bullets.Add(b);

        return b;
    }
}