using UnityEngine;

/// <summary>
/// A RemoteAudioSource allows sounds to be played from connected Remotes.
/// </summary>
public class RemoteAudioSource : RemoteComponent {

    AudioSource fallbackAudioSource;
    // The overall source's last set volume.
    public float volume {get; private set;}

    protected override void RemoteComponentAwake() {
        moduleKeyword = "audio";
    }

    public override void ActivateFallback() {
        fallbackAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void DeactivateFallback() {
        Destroy(fallbackAudioSource);
    }
    
    // Play a sound from the remote device.
    public void Play (RemoteAudioClip clip) {
        if (fallbackMode) {
            // If we're in fallback mode, just play the sound through the attached audio source.
            AudioClip localClip = clip.localClip;
            fallbackAudioSource.PlayOneShot(localClip);
            return;
        }

        SendCommand("play", clip.AsArgs());
    }

    // Sets the volume to f (0.0 - 1.0)
    public void SetVolume (float f) {
        volume = f;
        if (fallbackMode) {
            fallbackAudioSource.volume = f;
        } else {
            SendCommand("volume", f.ToString());
        }
    }

    public void SetAudioConfig(RemoteAudioConfig config) {
        throw new System.NotImplementedException();
    }

}