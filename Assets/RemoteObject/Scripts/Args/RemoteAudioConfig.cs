using UnityEngine;

[CreateAssetMenu(fileName = "RemoteAudioConfig", menuName = "RemoteObjectSystem/RemoteAudioConfig", order = 0)]
public class RemoteAudioConfig : RemoteArgs {
    [SerializeField] int sampleRate;
    [SerializeField] int bufferSize;

    public override string[] AsArgs() {
        return new string[] {sampleRate.ToString(), bufferSize.ToString()};
    }
}