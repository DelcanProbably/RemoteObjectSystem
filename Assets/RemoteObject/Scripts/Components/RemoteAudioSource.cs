using UnityEngine;

/// <summary>
/// A RemoteAudioSource allows sounds to be played from connected Remotes.
/// </summary>
public class RemoteAudioSource : RemoteComponent {

    AudioSource fallbackAudioSource;
    // The overall source's last set volume.
    public float volume {get; private set;}

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
            return;
        }

        SendCommand("play", sound);
    }

    // Sets the volume to f (0.0 - 1.0)
    public void SetVolume (float f) {
        volume = f;
        if (fallbackMode) {
            fallbackAudioSource.volume = f;
        } else {
            SendCommand("volume", new string[] {f.ToString()});
        }
    }

    public void SetAudioConfig(RemoteAudioConfig config) {
        throw new System.NotImplementedException();
    }

}