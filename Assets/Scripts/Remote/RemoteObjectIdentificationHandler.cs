using UnityEngine;
using System.Collections.Generic;

public class RemoteObjectIdentificationHandler : MonoBehaviour {
    
    [SerializeField] List<string> ipAddresses = new List<string>();
    [SerializeField] List<RemoteObjectIdentificationUIItem> uiItems = new List<RemoteObjectIdentificationUIItem>();

    [SerializeField] Canvas remoteIdentificationCanvas;
    [SerializeField] RectTransform identificationUIPanel;
    [SerializeField] GameObject identificationUIItemPrefab;


    int currentIp = 0;
    RemotePi currentRemote;


    void Update () {
        // TODO: debug F9 to start id'ing
        if (Input.GetKeyDown(KeyCode.F9)) StartIdentificationFlow(RemoteManager.GetRemotes(), ipAddresses);
    }

    void ClearUI () {
        foreach(Transform t in identificationUIPanel) {
            Destroy(t.gameObject);
        }
    }

    RemoteObjectIdentificationUIItem CreateUIItem (string remoteName) {
        GameObject newItem = Instantiate(identificationUIItemPrefab, identificationUIPanel);
        RemoteObjectIdentificationUIItem item = newItem.GetComponent<RemoteObjectIdentificationUIItem>();
        return item;
    }

    public void StartIdentificationFlow(List<RemoteObject> remotes, List<string> ips) {
        
        // UI - generate and show
        ClearUI();
        List<RemoteObjectIdentificationUIItem> items = new List<RemoteObjectIdentificationUIItem>();
        foreach (RemoteObject remote in remotes) {
            items.Add(CreateUIItem(remote.gameObject.name));
        }

        foreach (RemoteObjectIdentificationUIItem item in items) {
            Debug.Log(item.name);
        }

        currentIp = 0;

        // for each IP - send identify message, ask player to select the relevant object.

        // hide UI
    }

    void IdentifyItem(string ip) {
        RemoteNetHandler.SendNetMessage(currentRemote, "/id/");
    }

    void NextItem () {
        currentIp++;

        if (currentIp >= ipAddresses.Count) {
            currentIp = -1;
            return;
        }

        currentRemote = new RemotePi(ipAddresses[currentIp]);

    }

    public void ItemSelected (RemoteObject remote, RemoteObjectIdentificationUIItem ui) {
        remote.remote = currentRemote;
    }

}