using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 Direction;
    public float Speed;
    public int Damage;
    public Tags.Tag CollisionTag;
    public bool Fire;

    GameManager _gameController;

    float _delta;

    void Start()
    {
        _gameController = GameManager.Instance;

        _delta = 0;
    }

    void Update()
    {
        if (!_gameController.IsCurrentState(GameStates.Play))
        {
            return;
        }

        if (!Fire)
        {
            return;
        }

        transform.position = Tools2D.MoveInDirection(transform.position, Direction, Time.deltaTime * Speed);

        _delta += Time.deltaTime;

        if (_delta > 0.03f)
        {
            _delta = 0;
            if (_gameController.Map.OutOfMap(transform.position, transform.localScale))
            {
                Die();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != Tags.GetStringTag(CollisionTag)) return;

        Piece piece = other.GetComponent<Piece>();
        piece.TakeDamage(Damage);
        _gameController.CreateWound(piece.Movement, (piece.transform.position - transform.position).normalized);
        Die();
    }

    void Die()
    {
        Fire = false;
        gameObject.SetActive(false);
        transform.position = new Vector3(100, 100, transform.position.z);
    }
}