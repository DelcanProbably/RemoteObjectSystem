using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RemoteAudioSource), typeof(RemoteArduino))]
public class DemoPongPaddle : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode down;
    float axis;

    [SerializeField] float speed;

    RemoteAudioSource remoteAudioSource;
    [SerializeField] RemoteAudioClip hitSound;
    RemoteArduino remoteArduino;
    [SerializeField] int LEDPin;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        remoteAudioSource = GetComponent<RemoteAudioSource>();
        remoteArduino = GetComponent<RemoteArduino>();
    }

    private void Start() {
    }

    private void Update() {
        axis = 0;
        if (Input.GetKey(up)) {
            axis += 1;
        }
        if (Input.GetKey(down)) {
            axis -= 1;
        }
        if (Input.GetKeyDown(KeyCode.F11)) {
            OnLinkedDeviceUpdated();
        }
    }

    void FixedUpdate() {
        // rb.velocity = new(0, axis * speed, 0);
        rb.MovePosition(rb.position + Vector3.up * axis * speed * Time.fixedDeltaTime);
    }

    public void Hit () {
        remoteAudioSource.Play(hitSound);
        StartCoroutine(Blink());
    }

    public void OnLinkedDeviceUpdated() {
        remoteArduino.SetAllPinsOutput();       
    }

    IEnumerator Blink () {
        remoteArduino.DigitalWrite(LEDPin, 1);
        yield return new WaitForSeconds(2);
        remoteArduino.DigitalWrite(LEDPin, 0);
    }
}
