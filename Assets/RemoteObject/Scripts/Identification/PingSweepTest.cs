using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

// TODO: this is not used and is the base for code in RemoteObjectIdentificationHandler, but should be a standard class "PingSweep" => move Start in constructor, Coroutine as async
public class PingSweepTest : MonoBehaviour
{
    static string ipBase = "192.168.1.";

    // Start is called before the first frame update
    void Start()
    {
        string[] ipChunks = RemoteManager.localIP.Split(".");
        ipBase = ipChunks[0] + "." + ipChunks[1] + "." + ipChunks[2] + ".";
        StartCoroutine(IPSweep());
    }

    // note future me: this code doesn't work, there have been changes to this coroutine only in remoteobjectidentificationhandler
    static IEnumerator IPSweep() {
        List<Ping> pings = new List<Ping>();
        for (int i = 1; i < 255; i++) {
            string ip = ipBase + i.ToString();
            Ping ping = new Ping(ip);
            pings.Add(ping);
        }
        
        yield return new WaitForSeconds(1);
        foreach (Ping p in pings) {
            if (p.isDone) {
                Debug.Log("Found IP at " + p.ip.ToString());
                pings.Remove(p);
            }
        }

    }
}
