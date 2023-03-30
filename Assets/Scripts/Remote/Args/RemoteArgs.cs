using UnityEngine;

public abstract class RemoteArgs : ScriptableObject {

    public virtual string[] AsArgs () {
        return new string[0];
    }
}