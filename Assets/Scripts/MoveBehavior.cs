﻿using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    public float Speed;
    public Spot Target;
    public Spot Step;
    public Spot CurrentSpot;
    public Edge CurrentEdge;
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
    }

    void Update()
    {
        if (_gameManager.CurrentState != GameStates.Play)
        {
            IsMoving = false;
            return;
        }

        if (Target == null || Step == null)
        {
            IsMoving = false;
            return;
        }

        Animator.SetFloat("Speed", _animSpeed);

        if (Tools2D.SamePos(transform.position, Step.transform.position))
        {
            IsMoving = false;
            CurrentSpot = Step;
            CurrentEdge = null;
        }

        if (CurrentSpot != Target && _gameManager.GetNextStep(this, out Step))
        {
            IsMoving = true;
            if (CurrentSpot != null)
            {
                CurrentSpot.GetEdge(Step, out CurrentEdge);
                CurrentSpot = null;
            }
            transform.position = Tools2D.MoveTowards(transform.position, Step.transform.position, Time.deltaTime * Speed);
            transform.rotation = Tools2D.LookAt(transform.position, Step.transform.position);
        }
    }
}
