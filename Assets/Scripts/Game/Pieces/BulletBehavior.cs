using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public Tags CollisionTag;
    public float Speed;
    public int Damage;
    public Vector2 Direction;

    GameManager _gameManager;

    float _timePassedSinceLastOutOfMapCheck;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _timePassedSinceLastOutOfMapCheck = 0;
    }

    void Update()
    {
        if (!_gameManager.IsCurrentState(GameStates.Play))
        {
            return;
        }

        transform.position = Tools2D.MoveInDirection(transform.position, Direction, Time.deltaTime * Speed);

        _timePassedSinceLastOutOfMapCheck += Time.deltaTime;

        if (_timePassedSinceLastOutOfMapCheck > 0.0075f)
        {
            _timePassedSinceLastOutOfMapCheck = 0;
            if (_gameManager.Map.OutOfBounds(transform))
            {
                Die();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != CollisionTag.ToString()) return;

        PieceBehavior otherPiece = other.GetComponent<PieceBehavior>();
        if (otherPiece.Alive)
        {
            otherPiece.TakeDamage(Damage);
            _gameManager.Wounds.CreateWound(otherPiece.transform, transform);
            Die();
        }      
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}