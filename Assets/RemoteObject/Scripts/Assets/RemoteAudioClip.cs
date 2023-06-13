using UnityEngine;

[CreateAssetMenu(fileName = "RemoteSound", menuName = "RemoteObjectSystem/RemoteSound", order = 0)]
public class RemoteAudioClip : RemoteAsset {
    [SerializeField] public AudioClip localClip;
    [SerializeField] string clipName;

    public override string[] AsArgs() {
        return new string[] {clipName};
    }
}