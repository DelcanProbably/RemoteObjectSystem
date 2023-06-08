using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;

/// <summary>
/// A RemoteObject is one that represents a physical remote Raspberry Pi.
/// Either use abstractly with empty gameObjects, or listerally with sensors
/// controlling its properties in Unity 3D space to create a virtual clone. 
/// </summary>
public class RemoteObject : MonoBehaviour
{

    public RemotePi remote;
    // If set, this a RemotePi will be created with the given IP
    // TODO: Eventually this should be handled at runtime as unique
    // remotes will need to be identified
    [SerializeField] string debugIPAddress;
    [SerializeField] public string remoteName;
    [SerializeField] public Sprite remoteIcon;

    // Automatically activate fallback mode if no device is assigned.
    [SerializeField] bool autoFallbackMode = true;
    public bool fallbackMode {get; private set;}

    public List<RemoteComponent> rComponents = new List<RemoteComponent>();

    private void Awake() {
        // If the debug IP is set then we'll establish that connection.
        if (debugIPAddress != "") {
            remote = new RemotePi(debugIPAddress);
        }
        RemoteManager.RegisterRemote(this);
        
        // If the remoteName hasn't been set, we'll set it to the gameObject name.
        if (remoteName == "") {
            remoteName = name;
        }
    }

    #if UNITY_EDITOR
    private void Update() {
        // Debug log info dump when f10 booped
        if (Input.GetKeyDown(KeyCode.F10)) Debug.Log(remoteName + ", ip: " + remote.ip);
    }
    #endif


    void SendRawCommand (string command) {
        if (remote == null) {
            Debug.Log("SendRawCommand called on RemoteObject with no linked remote. Ignoring.");
            return;
        }
        // TODO: why go through NetHandler instead of just directly calling the RemotePi??? idk?
        RemoteNetHandler.SendNetMessage(remote, command);
    }

    // Send da command
    public void SendCommand(string module, string func, string[] args) {
        string command = "/";
        command += module + "/" + func;
        foreach (string s in args) {
            command += "/" + s;
        }
        SendRawCommand(command);
    }

    // TODO: this gets a bit redundant...
    // Send a command from the given module with the given args
    public void SendCommand (string module, string func, RemoteArgs args) {
        SendCommand(module, func, args.AsArgs());
    }

    public void SendCommand (string module, string func, RemoteAsset assetRef) {
        SendCommand(module, func, assetRef.AsArgs());
    }

    // Updates fallback mode dependant on if a Remote is assigned, but only if autoFallbackMode is enabled
    public void UpdateFallbackMode() {
        if (autoFallbackMode) {
            fallbackMode = remote == null; // i hate this
        }   

        foreach (RemoteComponent component in rComponents) {
            if (fallbackMode) component.ActivateFallback();
            else component.DeactivateFallback();
        }
    }

    // Resets this object's remote link.
    public void ResetRemote() {
        remote = null;
    }

}
