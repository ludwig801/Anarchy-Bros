using UnityEngine;

public class Wound : MonoBehaviour
{
    public float Speed;
    public Transform Follow;

    Animator Animator { get { if (_animator == null) { _animator = GetComponent<Animator>(); } return _animator; } }
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

    public void Restart()
    {
        Animator.SetBool("Hurt", false);
        Follow = null;     
    }

    public void Die()
    {
        gameObject.SetActive(true);
        Animator.SetBool("Hurt", true);
    }
}
