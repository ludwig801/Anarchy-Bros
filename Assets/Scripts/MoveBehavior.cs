using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    public float Speed;
    public Spot Target;
    public Spot Step;
    public Spot CurrentSpot;
    public Edge CurrentEdge;
    public bool CanMove;
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
    public bool IsMoving
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
        if (!_gameManager.IsCurrentState(GameStates.Play))
        {
            IsMoving = false;
            return;
        }

        if (!CanMove)
        {
            IsMoving = false;
            return;
        }

        if (Target == null)
        {
            IsMoving = false;
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
            IsMoving = false;
            Step = null;
        }
        else if (_gameManager.StepToTarget(this, out Step))
        {
            IsMoving = true;
            if (CurrentSpot != null)
            {
                CurrentSpot.GetEdge(Step, out CurrentEdge);
                CurrentSpot = null;
            }
            transform.position = Tools2D.MoveTowards(transform.position, Step.transform.position, Time.deltaTime * Speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Tools2D.LookAt(transform.position, Step.transform.position), Time.deltaTime * 8f);
        }
    }
}
