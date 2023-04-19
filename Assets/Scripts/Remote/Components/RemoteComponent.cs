using UnityEngine;

/// <summary>
/// A remote component is a component which can be applied to Unity GameObjects and
/// corresponds directly to a module installed on the Remote Pi. All gameplay
/// interaction with RemotePi's should be through a class of RemoteComponent.
/// </summary>
[RequireComponent(typeof(RemoteObject))]
public abstract class RemoteComponent : MonoBehaviour {
    // The name of this module. MUST be set by child classes.
    protected string moduleName;
    // The RemoteObject this component is attached to.
    protected RemoteObject remote;
    // If true, will fallback to emulating the intended result through the local system.
    [SerializeField] protected bool fallbackMode;
    
    private void Awake() {
        remote = GetComponent<RemoteObject>();
        RemoteComponentAwake();
    }
    // Run in Awake after RemoteComponent parent setup.
    protected abstract void RemoteComponentAwake();

    public virtual void ActivateFallback() {
        Debug.LogWarning(name + " - A RemoteComponent on this object does not support fallback mode, but ActivateFallback has been called.");
    }

    // TODO: bit redundant innit
    protected void SendCommand(string func, string[] args) {

        if (fallbackMode) {
            Debug.LogWarning("RemoteComponent on " + gameObject.name + " attempted to run a command in fallback mode. This shouldn't be happening - fallback mode is not properly implemented somewhere.");
            return;
        }

        // this isn't perfect, but it will work fine.
        // Not sure what the perfect implementation of this kind of system is.
        remote.SendCommand(moduleName, func, args);
    }
    protected void SendCommand(string func, RemoteAsset remoteAsset) {
        SendCommand(func, remoteAsset.AsArgs());
    }
    protected void SendCommand(string func, RemoteArgs remoteArgs) {
        SendCommand(func, remoteArgs.AsArgs());
    }
}