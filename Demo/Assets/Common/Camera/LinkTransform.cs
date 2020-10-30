using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkTransform : MonoBehaviour
{
    public Transform Source;

    void Start()
    {
        SetTRS();
    }

    // Update is called once per frame
    void Update()
    {
        SetTRS();
    }

    void SetTRS()
    {
        transform.position = Source.position;
        transform.rotation = Source.rotation;
        transform.localScale = Source.localScale;
    }
}
