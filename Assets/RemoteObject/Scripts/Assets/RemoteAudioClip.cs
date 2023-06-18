using UnityEngine;

[CreateAssetMenu(fileName = "RemoteAudioClip", menuName = "RemoteObjectSystem/RemoteAudioClip", order = 0)]
public class RemoteAudioClip : RemoteAsset {
    [SerializeField] public AudioClip localClip;
    [SerializeField] string clipName;

    public override string[] AsArgs() {
        return new string[] {clipName};
    }
}