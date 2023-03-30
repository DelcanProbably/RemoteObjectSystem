using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKeyPlayAudioThing : MonoBehaviour
{
    [SerializeField] RemoteSound remoteSound;
    [SerializeField] RemoteAudioSource audioSource;    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) { 
            audioSource.Play(remoteSound);
        }
    }
}
