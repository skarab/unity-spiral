using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Cube : MonoBehaviour
{
    public VisualEffect ParticlesDeath;

    private float _Timer = 0.0f;

    void Update()
    {
        _Timer += Time.deltaTime;
        if (_Timer>=5.0f)
        {
            ParticlesDeath.SetVector3("position", transform.position);
            DestroyImmediate(gameObject);
        }
    }
}
