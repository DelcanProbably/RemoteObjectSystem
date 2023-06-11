using UnityEngine;

/// <summary>
/// Low-level GPIO Interface for Remotes
/// </summary>
public class RemoteGPIO : RemoteComponent
{

    protected override void RemoteComponentAwake() {
        moduleKeyword = "gpio";
    }

    public void SetOutputPin(int pin, string mode) {
        SendCommand("setpin", mode);
    }

}