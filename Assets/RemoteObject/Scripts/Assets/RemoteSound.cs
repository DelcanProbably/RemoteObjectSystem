using UnityEngine;

[CreateAssetMenu(fileName = "RemoteSound", menuName = "RemoteObjectSystem/RemoteSound", order = 0)]
public class RemoteSound : RemoteAsset {
    [SerializeField] public AudioClip clip;
    [SerializeField] string clipName;

    public override string[] AsArgs() {
        return new string[] {clipName};
    }
}