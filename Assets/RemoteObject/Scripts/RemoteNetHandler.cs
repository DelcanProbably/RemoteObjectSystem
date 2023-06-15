/// <summary>
/// All the dodgy net code goes here.
/// </summary>
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class RemoteNetHandler {
    public static int Port = 32019;
    static List<RemoteDevice> sockets;

    public static void NewRemote (RemoteDevice rem) {
        if (sockets == null) {
            sockets = new List<RemoteDevice>();
        }
        sockets.Add(rem);
    }

    public static void SendToAll(string message) {
        if (sockets == null) {
            System.Console.WriteLine("SendToAll called when no sockets registered");
            return;
        }
        foreach (RemoteDevice rem in sockets) {
            // TODO: this is not particularly efficient smh data structures
            if (rem == null) {
                sockets.Remove(rem);
                continue;
            }
            
            SendNetMessage(rem, message);
        }
    }

    public static void SendNetMessage (RemoteDevice rem, string message) {
        rem.SendNetMessage(message);
    }

    public static void RemoveRemote (RemoteDevice rem) {
        sockets.Remove(rem);
    }

}