using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    public float RotationSpeed;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime));
    }
}