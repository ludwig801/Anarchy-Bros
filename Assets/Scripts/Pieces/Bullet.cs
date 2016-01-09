using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Tags.Tag CollisionTag;
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

        if (_timePassedSinceLastOutOfMapCheck > 0.03f)
        {
            _timePassedSinceLastOutOfMapCheck = 0;
            if (_gameManager.Map.OutOfMap(transform.position, transform.localScale))
            {
                Die();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != Tags.GetStringTag(CollisionTag)) return;

        Piece otherPiece = other.GetComponent<Piece>();
        if (otherPiece.Alive)
        {
            otherPiece.TakeDamage(Damage);          
        }
        _gameManager.CreateWound(otherPiece.Movement, -Direction.normalized);
        Die();
    }

    void Die()
    {
        gameObject.SetActive(false);
        transform.position = new Vector3(100, 100, transform.position.z);
    }
}