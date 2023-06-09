using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

// TODO: this script is getting chunky
public class RemoteObjectIdentificationHandler : MonoBehaviour {

    enum State {Idle, Scanning, Identification};

    State state = State.Idle;
    
    List<string> ipAddresses;
    [SerializeField] List<RemoteObjectIdentificationUIItem> uiItems = new List<RemoteObjectIdentificationUIItem>();

    [SerializeField] Canvas remoteIdentificationCanvas;
    [SerializeField] RectTransform identificationUIPanel;
    [SerializeField] GameObject identificationUIItemPrefab;
    [SerializeField] TMP_Text IPText;
    [SerializeField] GameObject searchingUIObject;

    [SerializeField] int ipSweepTimeout = 5;
    [SerializeField] float identifyRepeatRate = 1.0f;

    int currentIp = 0;
    RemotePi currentRemote;

    [SerializeField] bool searchOnStart;

    private void Start() {
        // Ensure canvas begins disabled
        remoteIdentificationCanvas.enabled = false;
        if (searchOnStart) Begin();

    }

    void Update () {
        // DEBUG KEYS
        if (RemoteManager.DebugKeysEnabled) {
            if (Input.GetKeyDown(KeyCode.F9)) Begin();
        }
    }

    public void Begin() {
        remoteIdentificationCanvas.enabled = true;
        StartIPSweep();
    }

    // Gathering IPs
    void StartIPSweep() {
        // Check state before starting flow.
        if (state == State.Scanning) {
            return;
        } else {
            state = State.Scanning;
        }
        
        // Get each section of this IP and squash it into a base IP e.g. "192.168.1."
        // TODO: my brain cannot do sensible string manipulation right now but surely this can just like find the last dot and cut off the end instead
        string[] ipChunks = RemoteManager.localIP.Split(".");
        string ipBase = ipChunks[0] + "." + ipChunks[1] + "." + ipChunks[2] + ".";
        StartCoroutine(IPSweep(ipBase));
    }

    IEnumerator IPSweep(string ipBase) {
        // Show UI e.g. Searching...
        searchingUIObject.SetActive(true);

        // Ping every IP from xxx.xxx.xxx.1 to xxx.xxx.xxx.255
        List<Ping> pings = new List<Ping>();
        for (int i = 1; i < 255; i++) {
            string ip = ipBase + i.ToString();
            Ping ping = new Ping(ip);
            pings.Add(ping);
        }

        // List to store IPs that pong
        List<string> foundIps = new();

        // Because we can't know how many remotes there are on the network, we always run for the whole timeout time.
        for (int secs = 0; secs < ipSweepTimeout; secs++) {
            yield return new WaitForSeconds(1);

            // We will delete seen pings, this Stack will handle that
            Stack<int> seen = new();

            for (int i = 0; i < pings.Count; i++) {
                Ping p = pings[i];
                if (p.isDone) {
                    Debug.Log("Found IP at " + p.ip.ToString());
                    foundIps.Add(p.ip);
                    seen.Push(i);
                }
            }

            while (seen.Count > 0) {
                int i = seen.Pop();
                pings.RemoveAt(i);
            }
            
        }

        CompleteIPSweep(foundIps);

    }

    void CompleteIPSweep(List<string> ips) {
        searchingUIObject.SetActive(false); // TODO: change text to "checking for remotes" or other.
        string ipList = "";
        foreach (string s in ips.ToArray()) ipList += s + "  ";
        Debug.Log("Starting flow with IP List: " + ipList);
        // TODO: confirm these are all actual remotes using a /ping command
        StartLinkingFlow(RemoteManager.GetRemotes(), ips);
    }

    // ###########################
    // ID flow (IPs are now KNOWN)
    // ###########################

    void StartLinkingFlow(List<RemoteObject> remoteObjects, List<string> ips) {
        
        // Check state before starting flow.
        if (state == State.Identification) {
            return;
        } else {
            state = State.Identification;
        }

        // Set global IP Address list to updated list
        // This is done here to make sure that without doubt the IP list has been updated before continuing the flow
        ipAddresses = ips;

        // UI - generate and show
        ClearUI();
        List<RemoteObjectIdentificationUIItem> items = new List<RemoteObjectIdentificationUIItem>();
        foreach (RemoteObject remoteObject in remoteObjects) {
            // Clear current remotepi.
            remoteObject.ResetRemote();

            // Create UI item for this object
            RemoteObjectIdentificationUIItem item = CreateUIItem(remoteObject);
            items.Add(item);
        }

        // Make sure we're at the start of the list and then call the coroutine to start working through each remote.
        currentIp = -1; // FIXME: wtf this should probably be a function
        StartCoroutine(IdentificationCoroutine());

    }

    IEnumerator IdentificationCoroutine() {
        
        RemotePi currentRemote = NextRemote();
        while (currentRemote != null) {
            Debug.Log("Identifying remote at IP " + currentRemote.ip + "[" + currentIp + "]");
            IdentifyItem(currentRemote.ip);
            float timer = 0; // timer for repeated identification pings.
            while (currentRemote.state == RemoteState.Unassigned) {
                timer += Time.deltaTime;
                // Every few seconds repeat the identification. e.g. sound will play every 2 seconds rather than just once
                if (timer >= identifyRepeatRate) {
                    IdentifyItem(currentRemote.ip);
                    timer -= identifyRepeatRate;
                }
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // skip this remote
                    SkipRemote();
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

    // Called when a remote object/pi pair is confirmed or skipped in identification flow
    // Retrieves the next Remote device that needs pairing
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

    // Run once IdentificationCoroutine complete
    void FinishIdentificationFlow() {
        Debug.Log("Identification Complete");

        // Loop through each RemoteObject and update its fallbackmode.
        foreach (RemoteObject r in RemoteManager.GetRemotes()) {
            r.UpdateFallbackMode();
        }

        remoteIdentificationCanvas.enabled = false;
        state = State.Idle;
    }

    // ID flow helpers

    RemoteObjectIdentificationUIItem CreateUIItem (RemoteObject remote) {
        GameObject itemObject = Instantiate(identificationUIItemPrefab, identificationUIPanel);
        RemoteObjectIdentificationUIItem item = itemObject.GetComponent<RemoteObjectIdentificationUIItem>();
        item.Initialise(this, remote);
        return item;
    }

    // Helper to clear all RemoteObject panels
    void ClearUI () {
        foreach(Transform t in identificationUIPanel) {
            Destroy(t.gameObject);
        }
    }

    // ID flow Controls

    // Sends ping to identify to given ip
    // 'Identify' - plays test sound, activates test lighting etc. depending on active modules
    void IdentifyItem(string ip) {
        RemoteNetHandler.SendNetMessage(currentRemote, "/id/");
        IPText.text = ip;
    }
    

    // Called when a RemoteObjectIdentificationUIItem's button is pressed.
    public void ItemSelected (RemoteObject remoteObject, RemoteObjectIdentificationUIItem ui) {
        remoteObject.remote = currentRemote;
        currentRemote.Assigned();
    }

    // Called by skip button and Escape key.
    public void SkipRemote() {
        currentRemote.SkippedAssignment();
    }



}