using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Vector3 _RollAxis;
    private float _Speed;

    void Awake()
    {
        do
        {
            _RollAxis = new Vector3(Random.value*2.0f-1.0f, Random.value*2.0f-1.0f, Random.value*2.0f-1.0f);
        }
        while (_RollAxis.magnitude<0.00001f);
        _RollAxis.Normalize();

        _Speed = Random.value+1.0f;

        float s = (Random.value+0.1f);
        transform.localScale = new Vector3(s, s, s);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_RollAxis, Time.deltaTime*_Speed*100.0f);
    }
}
