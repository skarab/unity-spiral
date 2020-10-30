using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baby : MonoBehaviour
{
    private const float _Timeout = 4.0f;
    private const float _Speed = 12.0f;
    private const float _LerpSpeed = 2.0f;

    private float _Timer = 0.0f;
    private Vector3 _Target;
    private Vector3 _Direction = Vector3.forward;

    void Start()
    {
        _Target = Vector3.Lerp(new Vector3(-2.0f, 0.0f, 1.0f), new Vector3(2.0f, 0.0f, 1.0f), Random.value).normalized;
    }

    void Update()
    {
        _Timer += Time.deltaTime;

        _Direction = Vector3.Lerp(_Direction, _Target, Time.deltaTime*_LerpSpeed).normalized;

        Vector3 old_position = transform.position;
        transform.position += _Direction*Time.deltaTime*_Speed;
        transform.rotation = Quaternion.LookRotation((transform.position-old_position).normalized, Vector3.up);

        if (_Timer>=_Timeout)
            DestroyImmediate(gameObject);
    }
}
