using UnityEngine;

/// <summary>
/// A RemoteAudioSource allows sounds to be played from connected Remotes.
/// </summary>
public class RemoteAudioSource : RemoteComponent {

    AudioSource fallbackAudioSource;

    protected override void RemoteComponentAwake() {
        moduleName = "audio";
    }

    public override void ActivateFallback() {
        fallbackAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void DeactivateFallback() {
        Destroy(fallbackAudioSource);
    }
    
    // Play a sound from the remote device.
    public void Play (RemoteSound sound) {
        if (fallbackMode) {
            // If we're in fallback mode, just play the sound through the attached audio source.
            AudioClip clip = sound.clip;
            fallbackAudioSource.PlayOneShot(clip);
            Debug.Log("playing de sound " + clip + fallbackAudioSource);
            return;
        }

        SendCommand("play", sound);
    }

    public void SetAudioConfig(RemoteAudioConfig config) {
        throw new System.NotImplementedException();
    }

}