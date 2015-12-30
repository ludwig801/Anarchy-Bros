using UnityEngine;
using System.Collections.Generic;
using AnarchyBros.Enums;

namespace AnarchyBros
{
    public class RangeWeapon : MonoBehaviour
    {
        public GameObject BulletPrefab, TargetPrefab;
        public Transform GunPoint, GunSprite, BulletsObj;
        public int RoundsPerMinute, MagazineSize;
        public float ReloadTime, BulletSpeed, Damage;
        public Tower Tower;
        public Enemy EnemyTarget;
        public Color ColorDefault, ColorReloading;
        public List<Bullet> Bullets;
        public bool PredictiveShooting;

        GameManager _gameManager;
        SpriteRenderer _renderer;
        float _shootingDelay, _deltaTime;
        int _bulletsLeft;
        bool _reloading;
        Vector2 _aimTarget;
        SpriteRenderer _targetSprite;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _renderer = GunSprite.GetComponent<SpriteRenderer>();
            _shootingDelay = 60f / RoundsPerMinute;
            _deltaTime = 0;
            _bulletsLeft = MagazineSize;
            _reloading = false;

            _targetSprite = Instantiate(TargetPrefab).GetComponent<SpriteRenderer>();
            _targetSprite.name = "Aim";
            _targetSprite.transform.parent = transform;
            _targetSprite.transform.localScale = 0.5f * _targetSprite.transform.localScale;
            _targetSprite.gameObject.SetActive(false);
        }

        void Update()
        {
            if (!_gameManager.IsCurrentState(GameStates.Play))
            {
                return;
            }

            if (EnemyTarget == null)
            {
                _targetSprite.gameObject.SetActive(false);
                return;
            }

            _targetSprite.gameObject.SetActive(true);

            UpdateTarget();

            transform.rotation = Tools2D.LookAt(transform.position, _aimTarget);
            _targetSprite.transform.position = _aimTarget;

            _deltaTime += Time.deltaTime;

            if (_reloading)
            {
                _renderer.color = Color.Lerp(_renderer.color, ColorReloading, Time.deltaTime * 8f);
                _reloading = (_deltaTime < ReloadTime);
            }
            else
            {
                _renderer.color = Color.Lerp(_renderer.color, ColorDefault, Time.deltaTime * 8f);
                if (_deltaTime >= _shootingDelay)
                {
                    Shoot();

                    _bulletsLeft--;
                    if (_bulletsLeft == 0)
                    {
                        _reloading = true;
                        _bulletsLeft = MagazineSize;
                    }

                    _deltaTime = 0;
                }
            }
        }

        void UpdateTarget()
        {
            _aimTarget = EnemyTarget.transform.position;
            if (PredictiveShooting)
            {
                Vector2 E = EnemyTarget.transform.position;
                Vector2 B = GunPoint.position;
                Vector2 vE = EnemyTarget.Speed * EnemyTarget.Direction;

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
                    Edge e;
                    if (MapManager.Instance.EdgeAt(P, out e) && e == EnemyTarget.Edge)
                    {
                        _aimTarget = P;
                    }
                    else
                    {
                        _aimTarget = EnemyTarget.MoveTo.transform.position;
                    }
                }
            }
        }

        void Shoot()
        {
            Bullet bullet = GetBullet();
            bullet.transform.position = GunPoint.position;
            bullet.transform.rotation = Tools2D.LookAt(GunPoint.position, _aimTarget);
            bullet.Direction = (Tools2D.Subtract(_aimTarget, GunPoint.position)).normalized;
            bullet.Speed = BulletSpeed;
            bullet.Damage = Damage;
            bullet.transform.parent = BulletsObj;
            bullet.gameObject.SetActive(true);
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
}