using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RemoteManager : MonoBehaviour {
    
    public static string localIP;

    static RemoteManager Instance;

    [HideInInspector] public List<RemoteObject> remotes = new List<RemoteObject>();

    // Poking sends a message regularly to keep a connection active.
    // This gets around some issues with inconsistent latency that can occur 
    // on setups configured with aggressive power-saving 
    // e.g. when using a mobile hotspot.
    [SerializeField] bool doPoking;
    // Seconds between "pokes"
    [SerializeField] float pokeInterval = 0.2f; 
    // Is there a poking invoke ongoing?
    static bool poking;

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
        if (doPoking) Poke();
    }

    private void Update() {
        // If there's a deviance between doPoking and poking, we need to fix it.
        if (doPoking != poking) {
            if (poking) {
                // poking true, therefore doPoking false
                // Disable Poke invokes
                CancelInvoke("Poke");
                poking = false;
            } else {
                // poking false, therefore doPoking true
                // Start a poke.
                Poke();
            }
        }

    }

    
    public static void RegisterRemote(RemoteObject remote) {
        Instance.remotes.Add(remote);
    }

    public static List<RemoteObject> GetRemotes() {
        return Instance.remotes;
    }

    private void Poke () {
        poking = true;
        RemoteNetHandler.SendToAll("/poke/");
        if (doPoking) {
            Invoke("Poke", pokeInterval);
        }
    }
}