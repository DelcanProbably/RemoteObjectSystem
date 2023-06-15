using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public enum RemoteState {
    Unassigned,
    SkippedAssignment,
    Assigned
}

/// <summary>
/// A RemotePi is a direct network connection to a remote.
/// This is the direct boundary connection between Unity and the Pi.
/// </summary>
public class RemoteDevice {
    public RemoteState state { get; private set; }
    public Socket socket { get; private set; }
    IPAddress ipAddress;
    public string ip { 
        get { return ipAddress.ToString(); } 
    }

    // Create a RemotePi with the given IP
    public RemoteDevice (string ipString) {
        state = RemoteState.Unassigned;
        // Initialise a UDP socket with which to connect this Remote.
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ipAddress = IPAddress.Parse(ipString);
        socket.Connect(ipAddress, RemoteNetHandler.Port);
        RemoteNetHandler.NewRemote(this);
    }
    
    public void SendNetMessage(string message) {
        if (socket == null) {
            Debug.LogError("Error: socket " + ip + " does not exist.");
            return;
        }
        if (!socket.Connected) {
            Debug.LogError("Error: socket " + ip + " is not connected.");
            return;
        }
        byte[] encodedMessage;
        encodedMessage = Encoding.UTF8.GetBytes(message);
        socket.Send(encodedMessage);
    }

    public void SkippedAssignment() {
        state = RemoteState.SkippedAssignment;
        RemoteNetHandler.RemoveRemote(this);
    }

    public void Assigned (RemoteObject remoteObject = null) {
        state = RemoteState.Assigned;
    }

}