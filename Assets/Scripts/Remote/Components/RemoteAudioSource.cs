using UnityEngine;

/// <summary>
/// A RemoteAudioSource allows sounds to be played from connected Remotes.
/// </summary>
public class RemoteAudioSource : RemoteComponent {

    protected override void RemoteComponentAwake() {
        moduleName = "audio";
    }

    public override void ActivateFallback() {
        if (fallbackMode) return;
        fallbackMode = true;
        
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    // Play a sound from the remote device.
    public void Play (RemoteSound sound) {
        if (fallbackMode) {
            // If we're in fallback mode, just play the sound through the attached audio source.
            GetComponent<AudioSource>().PlayOneShot(sound.clip);
        }

        SendCommand("play", sound);
    }

    public void SetAudioConfig(RemoteAudioConfig config) {
        throw new System.NotImplementedException();
    }

}