using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Events;

/// <summary>
/// A RemoteObject is one that represents a physical remote Raspberry Pi.
/// Either use abstractly with empty gameObjects, or listerally with sensors
/// controlling its properties in Unity 3D space to create a virtual clone. 
/// </summary>
public class RemoteObject : MonoBehaviour
{

    // The device this object is linked with.
    public RemoteDevice device;
    // Fallback mode allows properly configured components to continue running locally if no RemoteDevice is linked. See documentation for more information.
    public bool fallbackMode {get; private set;}
    // List of attached Remote Components
    [HideInInspector] public List<RemoteComponent> rComponents = new List<RemoteComponent>();

    // Automatically activate fallback mode if no device is assigned.
    [SerializeField] bool autoFallbackMode = true;
    // If set in Awake, this a RemotePi will be created with the given IP
    [SerializeField] string debugIPAddress;
    // The name of this RemoteObject to be displayed to the user.
    [SerializeField] public string remoteName;
    // An icon for this RemoteObject to be displayed to the user.
    [SerializeField] public Sprite remoteIcon;

    // Little weird, but has some utility.
    [SerializeField] UnityEvent linkedDeviceUpdateEvent;


    private void Awake() {
        // If the debug IP is set then we'll establish that connection.
        if (debugIPAddress != "") {
            device = new RemoteDevice(debugIPAddress);
        }
        
        // If the remoteName hasn't been set, we'll set it to the gameObject name.
        if (remoteName == "") {
            remoteName = name;
        }
    }
    
    private void Start() {
        RemoteManager.RegisterRemote(this);
        UpdateFallbackMode();
    }

    // Send a command
    public void SendCommand(string module, string func, string[] args) {
        string command = "/";
        command += module + "/" + func;
        foreach (string s in args) {
            command += "/" + s;
        }
        SendRawCommand(command);
    }

    public void SendRawCommand (string command) {
        if (device == null) {
            Debug.Log("SendRawCommand called on RemoteObject with no linked remote. Ignoring.");
            return;
        }
        // TODO: why go through NetHandler instead of just directly calling the RemotePi??? idk?
        RemoteNetHandler.SendNetMessage(device, command);
    }

    // Updates fallback mode dependant on if a Remote is assigned, but only if autoFallbackMode is enabled
    public void UpdateFallbackMode() {
        if (autoFallbackMode) {
            fallbackMode = device == null; // i hate this
        }   

        foreach (RemoteComponent component in rComponents) {
            if (fallbackMode) component.ActivateFallback();
            else component.DeactivateFallback();
        }
    }

    // Resets this object's remote link.
    public void ResetRemote() {
        device = null;
    }

    public void UpdateLinkedDevice(RemoteDevice newDevice) {
        device = newDevice;
        linkedDeviceUpdateEvent.Invoke();
        foreach(RemoteComponent rc in rComponents) {
            rc.OnLinkUpdated();
        }
    }

}
