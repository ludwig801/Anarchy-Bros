using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    public float RotationSpeed;
    public float SquizeSpeed, SquizeFactor;

    Vector3 _startScale, _squizeScale;

    void Start()
    {
        _startScale = transform.localScale;       
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, RotationSpeed * Time.deltaTime));

        bool clicking = Input.GetMouseButton(0) || Input.GetMouseButton(1);

        _squizeScale = new Vector3(SquizeFactor * _startScale.x, SquizeFactor * _startScale.y, _startScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, clicking ? _squizeScale : _startScale, Time.deltaTime * SquizeSpeed);
    }
}