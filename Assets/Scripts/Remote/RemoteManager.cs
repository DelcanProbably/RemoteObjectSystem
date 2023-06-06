using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RemoteManager : MonoBehaviour {
    
    public static string localIP;

    static RemoteManager Instance;
    static Coroutine poker;

    public List<RemoteObject> remotes = new List<RemoteObject>();

    // Poking sends a message regularly to keep a connection active.
    // This gets around some issues with inconsistent latency that can occur 
    // on setups configured with aggressive power-saving 
    // e.g. when using a mobile hotspot.
    [SerializeField] bool doPoking;
    // Seconds between "pokes"
    [SerializeField] float pokeInterval = 0.2f; 

    [SerializeField] bool debugKeysEnabled;
    public static bool DebugKeysEnabled { 
        get {
            return Instance.debugKeysEnabled; 
        }
    }

    private void Awake() {
        if (Instance) Destroy(gameObject);
        Instance = this;
        
        // this bloody bastard gets the local ip address
        // TODO: there's probably a less scuffed method for this
        System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach(System.Net.IPAddress ip in host.AddressList) {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                Debug.Log(ip + " found as local IP Address");
                localIP = ip.ToString();
            }
        }
    }

    private void Start() {
        if (doPoking) StartPoking();
    }

    private void Update() {
        if (doPoking) {
            doPoking = false;
            StartPoking();
        }
    }

    
    public static void RegisterRemote(RemoteObject remote) {
        Instance.remotes.Add(remote);
    }

    public static List<RemoteObject> GetRemotes() {
        return Instance.remotes;
    }

    // See doPoking above
    // TODO: idk could probs be better as a simple class but it'll do
    public static void StartPoking() {
        if (poker != null) {
            Debug.Log("StartPoking called when already poking.");
            return;
        }
        poker = Instance.StartCoroutine(Poker());
    }

    public static void StopPoking() {
        if (poker != null)
            Instance.StopCoroutine(poker);
        else
            Debug.Log("StopPoking called when not poking.");
    }

    static IEnumerator Poker() {
        while (true) {
            yield return new WaitForSeconds(Instance.pokeInterval);
            RemoteNetHandler.SendToAll("/poke/");
        }
    }
}