/// <summary>
/// All the dodgy net code goes here.
/// </summary>
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class RemoteNetHandler {
    public static int Port = 32019;
    static List<RemotePi> sockets;

    public static void NewRemote (RemotePi rem) {
        if (sockets == null) {
            sockets = new List<RemotePi>();
        }
        sockets.Add(rem);
    }

    public static void SendToAll(string message) {
        if (sockets == null) {
            System.Console.WriteLine("SendToAll called when no sockets registered");
            return;
        }
        foreach (RemotePi rem in sockets) {
            // TODO: this is not particularly efficient smh data structures
            if (rem == null) {
                sockets.Remove(rem);
                continue;
            }
            
            SendNetMessage(rem, message);
        }
    }

    public static void SendNetMessage (RemotePi rem, string message) {
        rem.SendNetMessage(message);
    }

}