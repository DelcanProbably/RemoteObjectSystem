using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RemoteObjectIdentificationHandler : MonoBehaviour {

    enum State {Idle, Identification};

    State state = State.Idle;
    
    [SerializeField] List<string> ipAddresses = new List<string>();
    [SerializeField] List<RemoteObjectIdentificationUIItem> uiItems = new List<RemoteObjectIdentificationUIItem>();

    [SerializeField] Canvas remoteIdentificationCanvas;
    [SerializeField] RectTransform identificationUIPanel;
    [SerializeField] GameObject identificationUIItemPrefab;


    int currentIp = 0;
    RemotePi currentRemote;

    private void Start() {
        // Ensure canvas begins disabled
        remoteIdentificationCanvas.enabled = false;
    }

    void Update () {
        // TODO: debug F9 to start id'ing
        if (Input.GetKeyDown(KeyCode.F9)) StartIdentificationFlow(RemoteManager.GetRemotes(), ipAddresses);
    }

    void ClearUI () {
        foreach(Transform t in identificationUIPanel) {
            Destroy(t.gameObject);
        }
    }

    RemoteObjectIdentificationUIItem CreateUIItem (RemoteObject remote) {
        GameObject itemObject = Instantiate(identificationUIItemPrefab, identificationUIPanel);
        RemoteObjectIdentificationUIItem item = itemObject.GetComponent<RemoteObjectIdentificationUIItem>();
        item.Initialise(this, remote);
        return item;
    }

    public void StartIdentificationFlow(List<RemoteObject> remoteObjects, List<string> ips) {
        
        // Check state before starting flow.
        if (state == State.Identification) {
            return;
        } else {
            state = State.Identification;
        }

        // UI - generate and show
        ClearUI();
        remoteIdentificationCanvas.enabled = true;
        List<RemoteObjectIdentificationUIItem> items = new List<RemoteObjectIdentificationUIItem>();
        foreach (RemoteObject remoteObject in remoteObjects) {
            RemoteObjectIdentificationUIItem item = CreateUIItem(remoteObject);
            items.Add(item);
        }

        // Make sure we're at the start of the list and then call the coroutine to start working through each remote.
        currentIp = -1; // FIXME: wtf this should probably be a function
        StartCoroutine(IdentificationCoroutine());

    }

    void FinishIdentificationFlow() {
        Debug.Log("Identification Complete");
        remoteIdentificationCanvas.enabled = false;
        state = State.Idle;
    }


    IEnumerator IdentificationCoroutine() {
        
        RemotePi currentRemote = NextRemote();
        while (currentRemote != null) {
            Debug.Log("Identifying remote at IP " + currentRemote.ip + "[" + currentIp + "]");
            IdentifyItem(currentRemote.ip);
            float timer = 0; // timer for repeated identification pings.
            while (currentRemote.state != RemoteState.Assigned) {
                timer += Time.deltaTime;
                // FIXME: hard-coded identification repeat time
                if (timer >= 3) {
                    IdentifyItem(currentRemote.ip);
                    timer -= 3;
                }
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // skip this remote
                    break;
                }
                yield return null;
            }
            // Delay one frame to ensure skip GetKeyDown only runs once.
            yield return null;
            // Get next remote
            currentRemote = NextRemote();
        }
        FinishIdentificationFlow();
    }

    // Sends ping to identify to given ip
    // 'Identify' - plays test sound, activates test lighting etc. depending on active modules
    void IdentifyItem(string ip) {
        RemoteNetHandler.SendNetMessage(currentRemote, "/id/");
    }

    // Called when a remote object/pi pair is confirmed or skipped in identification flow
    RemotePi NextRemote () {
        currentIp++;

        // Have gone through all remtoes, return null.
        if (currentIp >= ipAddresses.Count) {
            currentIp = -1;
            return null; 
        }

        // Get the next remote and return it.
        currentRemote = new RemotePi(ipAddresses[currentIp]);
        return currentRemote;
    }

    // Called when a RemoteObjectIdentificationUIItem's button is pressed.
    public void ItemSelected (RemoteObject remoteObject, RemoteObjectIdentificationUIItem ui) {
        remoteObject.remote = currentRemote;
        currentRemote.Assigned();
    }

}