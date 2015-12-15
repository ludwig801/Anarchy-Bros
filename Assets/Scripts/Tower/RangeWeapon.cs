using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class RangeWeapon : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public Transform GunPoint;
        public int RoundsPerMinute, Magazine;
        public float ReloadTime, BulletSpeed, Damage;
        public Enemy EnemyTarget;
        public Color ColorDefault, ColorReloading;
        public List<Bullet> Bullets;
        public bool Enabled;

        SpriteRenderer _renderer;
        float _shootingDelay, _deltaTime;
        int _bulletsLeft;
        bool _reloading;

        void Start()
        {
            _renderer = GunPoint.GetComponent<SpriteRenderer>();
            _shootingDelay = 60f / RoundsPerMinute;
            _deltaTime = 0;
            _bulletsLeft = Magazine;
            _reloading = false;
        }

        void Update()
        {
            if (!Enabled || EnemyTarget == null)
            {
                return;
            }

            transform.rotation = Tools2D.LookAt(transform.position, EnemyTarget.transform.position);

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
                        _bulletsLeft = Magazine;
                    }

                    _deltaTime = 0;
                }
            }
        }

        void Shoot()
        {
            Bullet b = GetBullet();
            b.transform.position = GunPoint.position;
            b.transform.rotation = Tools2D.LookAt(GunPoint.position, EnemyTarget.transform.position);
            b.Direction = (Tools2D.Subtract(EnemyTarget.transform.position, GunPoint.position)).normalized;
            b.Speed = BulletSpeed;
            b.Damage = Damage;
            //b.transform.parent = transform;
            b.gameObject.SetActive(true);

            Bullets.Add(b);
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

            return b;
        }
    }
}