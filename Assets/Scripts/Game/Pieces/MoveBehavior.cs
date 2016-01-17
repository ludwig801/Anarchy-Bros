using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    // Public vars
    public float Speed;
    [ReadOnly] public MapSpot Target;
    [ReadOnly] public MapSpot Step;
    [ReadOnly] public MapSpot CurrentSpot;
    [ReadOnly] public MapEdge CurrentEdge;
    [ReadOnly] public bool CanMove;
    // Properties
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }
    public bool Moving
    {
        get
        {
            return _isMoving;
        }

        set
        {
            _isMoving = value;
            Animator.SetBool("Moving", value);
        }
    }
    public bool HasCurrentSpot { get { return CurrentSpot != null; } }
    public bool HasCurrentEdge { get { return CurrentEdge != null; } }
    public Vector2 Direction { get { return Tools2D.DirectionFromRotation(transform.rotation); } }
    // Private vars
    Animator _animator;
    GameManager _gameManager;
    float _animSpeed;
    bool _isMoving;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _animSpeed = 0.5f * Speed;

        CanMove = true;
    }

    void Update()
    {
        if (!CanMove || !_gameManager.IsCurrentState(GameStates.Play) || Target == null)
        {
            Moving = false;
            return;
        }

        Animator.SetFloat("Speed", _animSpeed);

        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (Step != null && Tools2D.SamePos(transform.position, Step.transform.position))
        {
            CurrentSpot = Step;
            CurrentEdge = null;
        }

        if (CurrentSpot == Target)
        {
            Moving = false;
            Step = null;
        }
        else if (_gameManager.StepToTarget(this, out Step))
        {
            Moving = true;
            if (CurrentSpot != null)
            {
                CurrentSpot.FindEdge(Step, out CurrentEdge);
                CurrentSpot = null;
            }
            transform.position = Tools2D.MoveTowards(transform.position, Step.transform.position, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(transform.position, Step.transform.position), Time.deltaTime * 5f);
        }
    }
}
