using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemoteObjectIdentificationUIItem : MonoBehaviour {
    RemoteObjectIdentificationHandler handler;
    RemoteObject remote; // The remote this UI item represents.
    public void Initialise (RemoteObjectIdentificationHandler handler, RemoteObject remote) {
        this.handler = handler;
        this.remote = remote;
    }

    public void OnSelect () {
        handler.ItemSelected(remote, this);
    }
}