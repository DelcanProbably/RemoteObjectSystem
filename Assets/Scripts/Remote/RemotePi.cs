using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// A RemotePi is a direct network connection to a remote.
/// This is the direct boundary connection between Unity and the Pi.
/// </summary>
public class RemotePi {
    public Socket socket { get; private set; }
    public string ip { get; private set; }

    // Create a RemotePi with the given IP
    public RemotePi (string ipString) {
        // Initialise a UDP socket with which to connect this Remote.
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress ip = IPAddress.Parse(ipString);
        socket.Connect(ip, RemoteNetHandler.Port);
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
}