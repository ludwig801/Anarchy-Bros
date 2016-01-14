using UnityEngine;
using System.Collections;

public class ScalingButton : MonoBehaviour
{
    public float ScaleFactor, ScaleSpeed;
    Vector3 _initialScale, _scaledUp, _scaledDown;
    bool _scalingUp;

    // Use this for initialization
    void Start()
    {
        //_initialScale = transform.localScale;
        //_scaledUp = ScaleFactor * _initialScale;
        //_scaledDown = 1f / ScaleFactor * _initialScale;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
