using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    Rigidbody rb;
    float speed = 3.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.I)) // ‘O•ûˆÚ“®
            rb.velocity = transform.forward * speed;

        if (Input.GetKey(KeyCode.K)) // Œã•ûˆÚ“®
            rb.velocity = -transform.forward * speed;

        if (Input.GetKey(KeyCode.L)) // ‰EˆÚ“®
            rb.velocity = transform.right * speed;

        if (Input.GetKey(KeyCode.J)) // ¶ˆÚ“®
            rb.velocity = -transform.right * speed;
    }
}
