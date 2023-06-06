using UnityEngine;

public abstract class RemoteAsset : ScriptableObject {
    public virtual string[] AsArgs() { return new string[0]; }
}