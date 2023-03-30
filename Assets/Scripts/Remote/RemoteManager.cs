using UnityEngine;
using System.Collections;

public class RemoteManager : MonoBehaviour {
    
    static RemoteManager Instance;

    static Coroutine poker;

    // Poking sends a message regularly to keep a connection active.
    // This gets around some issues with inconsistent latency that can occur 
    // on setups configured with aggressive power-saving 
    // e.g. when using a mobile hotspot.
    [SerializeField] bool doPoking;
    // Seconds between "pokes"
    [SerializeField] float pokeInterval = 0.2f; 

    private void Awake() {
        if (Instance) Destroy(gameObject);
        Instance = this;
    }

    private void Start() {
        if (doPoking) StartPoking();
    }

    // See doPoking above
    #region Poking
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
    #endregion
}