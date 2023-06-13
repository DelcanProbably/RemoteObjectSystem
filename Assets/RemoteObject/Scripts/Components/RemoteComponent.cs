using UnityEngine;

/// <summary>
/// BASE CLASS FOR ALL REMOTE COMPONENTS
/// A remote component is a component which can be applied to Unity GameObjects and
/// corresponds directly to a module installed on the Remote Pi. All gameplay
/// interaction with RemotePi's should be through a class of RemoteComponent.
/// </summary>
[RequireComponent(typeof(RemoteObject))]
public abstract class RemoteComponent : MonoBehaviour {
    // The base command for this module. MUST be set by child classes.
    protected string moduleKeyword;
    // The RemoteObject this component is attached to.
    protected RemoteObject remoteObject;
    // If true, will fallback to emulating the intended result through the local system.
    protected bool fallbackMode {get {
        return remoteObject.fallbackMode;
    }}
    
    private void Awake() {
        remoteObject = GetComponent<RemoteObject>();
        // Add this components to remote's component list
        // This could probably be turned into a function to avoid the public list
        remoteObject.rComponents.Add(this);
        RemoteComponentAwake();
    }
    // Run in Awake after RemoteComponent parent setup.
    protected abstract void RemoteComponentAwake();

    public virtual void OnLinkUpdated() {}

    public virtual void ActivateFallback() {
        Debug.LogWarning(name + " - fallback mode has been activated on a RemoteComponent but there is no implementation.");
    }

    public virtual void DeactivateFallback() {
        Debug.LogWarning(name + " - fallback mode has been deactivated on a RemoteComponent but there is no implementation.");
    }

    protected void SendCommand(string func, string[] args) {

        if (fallbackMode) {
            return;
        }

        // this isn't perfect, but it will work fine.
        // Not sure what the perfect implementation of this kind of system is.
        remoteObject.SendCommand(moduleKeyword, func, args);
    }

    protected void SendCommand(string func, string arg) {
        SendCommand(func, new string[] {arg});
    }
}