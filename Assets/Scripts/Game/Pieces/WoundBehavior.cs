using UnityEngine;

public class WoundBehavior : MonoBehaviour
{
    // Public vars
    [Range(0f, 2f)] public float Speed;
    [ReadOnly] public Transform Follow;
    // Properties
    Animator Animator
    {
        get
        {
            if(_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }
    // Private vars
    Animator _animator;

    void Update()
    {
        if (Follow == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Animator.SetFloat("Speed", Speed);
            transform.position = Follow.position;
        }
    }

    public void Die()
    {
        Animator.SetBool("Hurt", false);
        Follow = null;
    }

    public void Live()
    {
        gameObject.SetActive(true);
        Animator.SetBool("Hurt", true);
    }
}
