using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System;
using System.Text;
using System.Net;

// unfinished
public class RemoteInput : RemoteComponent
{
    Socket socket;
    protected override void RemoteComponentAwake() {
        moduleKeyword = "input";
    }

    void Start() {
        SetupSocket();
    }

    void SetupSocket () {
        socket = remoteObject.device.socket;
        socket.Blocking = false;
        string ip = ((IPEndPoint)socket.LocalEndPoint).Address.ToString();
        SendCommand("init", ip);
    }

    private void Update() {
        string received = ReceiveData();
        if (received != "") {
            Debug.Log(received);
        }
    }

    private string ReceiveData () {
        byte[] incoming = new Byte[256];
        try { // FIXME IM BROKEN
            EndPoint oldMate = socket.RemoteEndPoint;
            socket.ReceiveFrom(incoming, ref oldMate);
        } catch (SocketException) {
            return "";
        }
        string decodedMessage = Encoding.UTF8.GetString(incoming);
        return decodedMessage;
    }

}
