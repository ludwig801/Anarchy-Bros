using UnityEngine;
using System.Collections;

public class WoundBehavior : MonoBehaviour
{
    public float Speed;
    public Transform Follow;

    Animator Animator { get { if (_animator == null) { _animator = GetComponent<Animator>(); } return _animator; } }
    Animator _animator;

    void Update()
    {
        Animator.SetFloat("Speed", Speed);
        transform.position = Follow.position;
    }

    public void Restart()
    {
        Animator.SetBool("Hurt", false);
        gameObject.SetActive(false);
    }

    public void Die()
    {
        gameObject.SetActive(true);
        Animator.SetBool("Hurt", true);
    }
}
