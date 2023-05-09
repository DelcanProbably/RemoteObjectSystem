using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RemoteObjectIdentificationUIItem : MonoBehaviour {
    RemoteObjectIdentificationHandler handler;
    RemoteObject remote; // The remote this UI item represents.
    [SerializeField] TMP_Text nameText;

    public void Initialise (RemoteObjectIdentificationHandler handler, RemoteObject remote) {
        this.handler = handler;
        this.remote = remote;

        nameText.text = remote.remoteName;
    }

    public void OnSelect () {
        handler.ItemSelected(remote, this);
        GetComponentInChildren<Button>().interactable = false;
    }
}