using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using TMPro;
using System.Net.Sockets;
using System.Text;
using System;

// TODO: this script is getting chunky
public class RemoteIdentificationHandler : MonoBehaviour {

    enum State {Idle, Scanning, Identification};

    State state = State.Idle;
    
    List<string> pongsReceived = new List<string>();
    List<RemoteDevice> currentFlowDevices;

    [SerializeField] Canvas remoteIdentificationCanvas;
    [SerializeField] RectTransform uiIpListPanel;
    [SerializeField] GameObject uiIpPrefab;
    [SerializeField] RectTransform uiObjectPanel;
    [SerializeField] GameObject uiObjectPrefab;
    [SerializeField] TMP_Text uiHeadingText;

    [SerializeField] int ipSweepTimeout = 5;
    [SerializeField] float identifyRepeatRate = 1.0f;

    int currentFlowDeviceID = 0;
    RemoteDevice currentRemote;

    [SerializeField] bool searchOnStart;
    [SerializeField] bool pauseTimescaleDuringUI;

    bool skipped = false;
    public bool dontSkipIps;

    private void Start() {
        // Ensure canvas begins disabled
        remoteIdentificationCanvas.enabled = false;
        if (searchOnStart) Begin();

    }

    void Update () {
        // DEBUG KEYS
        if (RemoteManager.DebugKeysEnabled) {
            if (Input.GetKeyDown(KeyCode.F9)) Begin();
            if (Input.GetKeyDown(KeyCode.Escape)) Skip();
        }
    }

    public void Begin() {
        remoteIdentificationCanvas.enabled = true;
        if (pauseTimescaleDuringUI) {
            Time.timeScale = 0;
        }
        ClearAllUILists();
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
        uiHeadingText.text = "Searching local network for IPs...";

        // Ensure UI correctly setup
        uiObjectPanel.gameObject.SetActive(false);
        uiIpListPanel.gameObject.SetActive(true);

        // Ping every IP from xxx.xxx.xxx.1 to xxx.xxx.xxx.255
        List<Ping> pings = new List<Ping>();
        for (int i = 1; i < 255; i++) {
            string ip = ipBase + i.ToString();
            // Skip this PC's IP
            if (ip == RemoteManager.localIP) continue;
            // Send a ping
            Ping ping = new Ping(ip);
            pings.Add(ping);
        }

        // # Slash pinging
        // We've sent a standard network ping, but once we've identified an IP as real, we want to confirm whether it is a remote.
        // To do this, we'll assume it is, and use a slash command that asks it to return a /pong/ command

        // Construct ping slash message - this is the same for all cases from this host.
        string pingMessage = "/ping/" + RemoteManager.localIP + "/" + (RemoteNetHandler.Port + 1).ToString() + "/";

        // Start checking for pongs
        StartCoroutine(PongReceiver());

        // List to store IPs that pong
        List<RemoteDevice> foundDevices = new();
        RemoteIdentificationIPUI.NewSweep();

        // Initialise skipped bool to false.
        skipped = false;


        // Because we can't know how many remotes there are on the network, we always run for the whole timeout time.
        for (int secs = 0; secs < ipSweepTimeout; secs++) {
            yield return new WaitForSecondsRealtime(1);

            // We will delete seen pings, this Stack will handle that
            Stack<int> seen = new();

            for (int i = 0; i < pings.Count; i++) {
                Ping p = pings[i];
                if (p.isDone) {
                    Debug.Log("Found IP at " + p.ip);
                    CreateIPUI(p.ip.ToString());

                    RemoteDevice device = new RemoteDevice(p.ip);
                    foundDevices.Add(device);
                    seen.Push(i);

                    // Send this device a /ping/
                    device.SendNetMessage(pingMessage);
                }
            }

            while (seen.Count > 0) {
                int i = seen.Pop();
                pings.RemoveAt(i);
            }

            // If skip has been called, skip.
            // Note - up to 1 sec latency, but whatevs.
            if (skipped) {
                break;
            }
            
        }

        // Configure current flow devices
        currentFlowDevices = new List<RemoteDevice>();
        // TODO: Restructure this, double for loop (foreach, contains).
        if (dontSkipIps) {
            currentFlowDevices = foundDevices;
        } else {
            foreach (RemoteDevice device in foundDevices) {
                if (pongsReceived.Contains(device.ip)) {
                    currentFlowDevices.Add(device);
                }
            }
        }

        CompleteIPSweep(currentFlowDevices);

    }

    // Receive /pong/'s
    IEnumerator PongReceiver () {
        UdpClient recvClient = new UdpClient(RemoteNetHandler.Port + 1);
        // Blank IPEndPoint to receive data about the sending IP.
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        while (state == State.Scanning) {
            yield return null;
            try {
                if (recvClient.Available > 0) {
                    byte [] data = recvClient.Receive(ref sender);
                    string s = Encoding.ASCII.GetString(data);
                    Debug.Log("Received " + s.ToString() + " from " + sender.Address.ToString());
                    
                    // If the message is pong, horray, it's a remote!
                    if (s.Contains("/pong")) {
                        RemoteIdentificationIPUI.ConfirmByIP(sender.Address.ToString());
                        // This device is confirmed, add it to the pong received list.
                        pongsReceived.Add(sender.Address.ToString());
                    }
                }
            } catch (Exception e) {
                Debug.LogWarning(e.ToString());
            }
        }
        Debug.Log("Stopped waiting for /pong/'s");
        recvClient.Close();
    }

    void CompleteIPSweep(List<RemoteDevice> devices) {
        string ipList = "";
        foreach (RemoteDevice r in devices.ToArray()) ipList += r.ip + "  ";
        Debug.Log("Starting flow with IP List: " + ipList);
        // TODO: confirm these are all actual remotes using a /ping command
        StartLinkingFlow(RemoteManager.GetRemotes());
    }

    // ###########################
    // ID flow (IPs are now KNOWN)
    // ###########################

    void StartLinkingFlow(List<RemoteObject> remoteObjects) {
        
        // Check state before starting flow.
        if (state == State.Identification) {
            return;
        } else {
            state = State.Identification;
        }

        // Swap UI panels
        uiIpListPanel.gameObject.SetActive(false);
        uiObjectPanel.gameObject.SetActive(true);


        // UI - generate and show
        List<RemoteIdentificationUIItem> items = new List<RemoteIdentificationUIItem>();
        foreach (RemoteObject remoteObject in remoteObjects) {
            // Clear current remotepi.
            remoteObject.ResetRemote();

            // Create UI item for this object
            RemoteIdentificationUIItem item = CreateObjectUI(remoteObject);
            items.Add(item);
        }

        // Make sure we're at the start of the list and then call the coroutine to start working through each remote.
        currentFlowDeviceID = -1; // FIXME: wtf this should probably be a function
        StartCoroutine(IdentificationCoroutine());

    }

    IEnumerator IdentificationCoroutine() {
        
        RemoteDevice currentRemote = NextRemote();

        // If the currentRemote is null after looking for next remote, then no devices were found.
        if (currentRemote == null) {
            // Hide the object panel and update message, then wait 3 seconds before closing.
            uiObjectPanel.gameObject.SetActive(false);
            uiHeadingText.text = "Could not find any Remote Devices on the network!";
            yield return new WaitForSecondsRealtime(3);
        }

        // This loop will run until there are no more remote devices left on the network.
        while (currentRemote != null) {
            Debug.Log("Identifying remote at IP " + currentRemote.ip + "[" + currentFlowDeviceID + "]");
            IdentifyItem(currentRemote.ip);
            float timer = 0; // timer for repeated identification pings.

            // Ensure skipped is false.
            skipped = false;

            while (currentRemote.state == RemoteState.Unassigned) {
                timer += Time.unscaledDeltaTime;
                // Every few seconds repeat the identification. e.g. sound will play every 2 seconds rather than just once
                if (timer >= identifyRepeatRate) {
                    IdentifyItem(currentRemote.ip);
                    timer -= identifyRepeatRate;
                }
                if (skipped) {
                    // skip this remote
                    SkipRemoteIdentification();
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
    RemoteDevice NextRemote () {
        currentFlowDeviceID++;

        // Have gone through all remtoes, return null.
        if (currentFlowDeviceID >= currentFlowDevices.Count) {
            currentFlowDeviceID = -1;
            return null; 
        }

        // Get the next remote and return it.
        currentRemote = currentFlowDevices[currentFlowDeviceID];
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
        // ISSUE: this could have issues if pause is toggled during the flow or if time should return to anything other than 1.0
        if (pauseTimescaleDuringUI) {
            Time.timeScale = 1;
        }
        state = State.Idle;
        
        // Clear devices list so garbage collection can do its thing.
        currentFlowDevices.Clear();
    }

    // ID flow helpers

    // Creates an IP entry in the IP list, returns its UI logic script.
    RemoteIdentificationIPUI CreateIPUI(string ip) {
        GameObject g = Instantiate(uiIpPrefab, uiIpListPanel);
        RemoteIdentificationIPUI ipui = g.GetComponent<RemoteIdentificationIPUI>();
        ipui.Initialise(ip);
        return ipui;
    }

    // Creates a button on the identification panel for a RemoteObject, returns its UI logic script.
    RemoteIdentificationUIItem CreateObjectUI (RemoteObject remote) {
        GameObject itemObject = Instantiate(uiObjectPrefab, uiObjectPanel);
        RemoteIdentificationUIItem item = itemObject.GetComponent<RemoteIdentificationUIItem>();
        item.Initialise(this, remote);
        return item;
    }

    // Helper to clear all RemoteObject panels
    void ClearAllUILists () {
        foreach(Transform t in uiIpListPanel) {
            Destroy(t.gameObject);
        }
        foreach(Transform t in uiObjectPanel) {
            Destroy(t.gameObject);
        }
    }

    // ID flow Controls

    // Sends ping to identify to given ip
    // 'Identify' - plays test sound, activates test lighting etc. depending on active modules
    void IdentifyItem(string ip) {
        RemoteNetHandler.SendNetMessage(currentRemote, "/id/");
        uiHeadingText.text = ip;
    }
    

    // Called when a RemoteObjectIdentificationUIItem's button is pressed.
    public void ItemSelected (RemoteObject remoteObject, RemoteIdentificationUIItem ui) {
        remoteObject.UpdateLinkedDevice(currentRemote);
        currentRemote.Assigned();
    }

    public void Skip() {
        skipped = true;
    }

    // Called when a remote link is skipped.
    void SkipRemoteIdentification() {
        currentRemote.SkippedAssignment();
    }


    public bool IsBusy() {
        return state != State.Idle;
    }


}