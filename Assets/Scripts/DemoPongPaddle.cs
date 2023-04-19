using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RemoteAudioSource), typeof(RemoteGPIO))]
public class DemoPongPaddle : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode down;
    float axis;

    [SerializeField] float speed;

    RemoteAudioSource remoteAudioSource;
    [SerializeField] RemoteSound hitSound;
    RemoteGPIO remoteGPIO;
    [SerializeField] int LEDPin;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        remoteAudioSource = GetComponent<RemoteAudioSource>();
        remoteGPIO = GetComponent<RemoteGPIO>();
    }

    private void Update() {
        axis = 0;
        if (Input.GetKey(up)) {
            axis += 1;
        }
        if (Input.GetKey(down)) {
            axis -= 1;
        }
    }

    void FixedUpdate() {
        rb.velocity = new(0, axis * speed, 0);
    }

    public void Hit () {
        remoteAudioSource.Play(hitSound);
        StartCoroutine(Blink());
    }

    IEnumerator Blink () {
        remoteGPIO.SetOutputPin(LEDPin, "high");
        yield return new WaitForSeconds(0.5f);
        remoteGPIO.SetOutputPin(LEDPin, "low");
    }
}
