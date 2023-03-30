/// <summary>
/// A RemoteAudioSource allows sounds to be played from connected Remotes.
/// </summary>
public class RemoteAudioSource : RemoteComponent {

    protected override void RemoteComponentAwake() {
        moduleName = "audio";
    }
    
    // Play a sound from the remote device.
    public void Play (RemoteSound sound) {
        SendCommand("play", sound);
    }

    public void SetAudioConfig(RemoteAudioConfig config) {

    }

}