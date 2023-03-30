using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPongBall : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float xSpeed = 5;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;
    AudioSource localAudioSource;
    [SerializeField] RemoteAudioSource remoteAudioSource;
    [SerializeField] RemoteSound remoteSound;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        localAudioSource = GetComponent<AudioSource>();
        if (localAudioSource.clip == null) {
            Debug.LogWarning("No clip set on ball's Audio Source");
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new(xSpeed, 0, 0);
    }

    private void OnCollisionEnter(Collision other) {
        xSpeed = -xSpeed;
        if (other.gameObject == leftWall) {
            Debug.Log("Left wall!");
            localAudioSource.Play();
        } else if (other.gameObject == rightWall) {
            Debug.Log("Right wall!");
            remoteAudioSource.Play(remoteSound);
        }
    }
}
