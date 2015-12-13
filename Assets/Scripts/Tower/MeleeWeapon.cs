using UnityEngine;

namespace AnarchyBros
{
    public class MeleeWeapon : MonoBehaviour
    {
        public float Damage, Range, AttackDelay, FadeSpeed;
        public Color ColorAttack, ColorFaded;

        SpriteRenderer _renderer;
        float _deltaTime;

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = ColorFaded;
            _deltaTime = 0;
        }

        void Update()
        {
            transform.localScale = Tools2D.Multiply(Vector3.one, Range);

            if (!Tools.AreColorsEqual(_renderer.color, ColorFaded))
            {
                _renderer.color = Color.Lerp(_renderer.color, ColorFaded, Time.deltaTime * FadeSpeed);
            }
            else
            {
                _renderer.color = ColorFaded;
            }
            _deltaTime += Time.deltaTime;
        }

        void Attack(Enemy e)
        {
            e.TakeDamage(Damage);
            _renderer.color = ColorAttack;
        }

        public void OnTriggerEnter2D(Collider2D data)
        {
            if (data.tag == "Enemy")
            {
                if (_deltaTime >= AttackDelay)
                {
                    Attack(data.GetComponent<Enemy>());
                    _deltaTime = 0f;
                }
            }
        }
    }
}