using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCamera : MonoBehaviour
{
    public Transform Target;
    public float Speed = 2.0f;
    private float _distance;

    void Start()
    {
        _distance = Target.position.z - transform.position.z;
    }
    void Update()
    {
        Vector3 position = transform.position;
        float interpolation = Speed * Time.deltaTime;
        position.z = Mathf.Lerp(transform.position.z, Target.position.z - _distance, interpolation);

        transform.position = position;
    }
}
