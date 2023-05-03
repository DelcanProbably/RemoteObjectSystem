using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPongBall : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float xSpeed = 5;
    float ySpeed = 0;
    [SerializeField] float yBouncePoint;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.R)) transform.position = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.y) > yBouncePoint) {
            ySpeed = -ySpeed;
        }

        rb.velocity = new(xSpeed, ySpeed, 0);

    }

    private void OnCollisionEnter(Collision other) {
        xSpeed = -xSpeed;
        ySpeed = Random.Range(xSpeed, -xSpeed);

        DemoPongPaddle paddle = other.gameObject.GetComponent<DemoPongPaddle>();
        if (paddle) {
            paddle.Hit();
        }
    }
}
