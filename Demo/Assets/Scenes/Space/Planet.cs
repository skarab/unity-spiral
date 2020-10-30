using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Transform Ship;

    void Start()
    {
        float s = (Random.value+0.1f)*20.0f;
        transform.localScale = new Vector3(s, s, s);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0.5f, 1.0f, 0.2f), Time.deltaTime*20.0f);

        if (transform.position.z>=Ship.position.z+2.0f)
            gameObject.SetActive(false);
    }
}
