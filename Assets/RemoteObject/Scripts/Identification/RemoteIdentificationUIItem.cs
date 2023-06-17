using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RemoteIdentificationUIItem : MonoBehaviour {
    RemoteIdentificationHandler handler;
    RemoteObject remote; // The remote this UI item represents.
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image iconImage;

    public void Initialise (RemoteIdentificationHandler handler, RemoteObject remote) {
        this.handler = handler;
        this.remote = remote;

        // Setup the UI for both the name and icon if present.
        nameText.text = remote.remoteName;
        if (remote.remoteIcon != null) {
            iconImage.sprite = remote.remoteIcon;
        }
    }

    public void OnSelect () {
        handler.ItemSelected(remote, this);
        GetComponentInChildren<Button>().interactable = false;
    }
}