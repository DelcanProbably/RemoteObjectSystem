using UnityEngine;

/// <summary>
/// Low-level GPIO Interface for Remotes
/// </summary>
public class RemoteGPIO : RemoteComponent
{

    protected override void RemoteComponentAwake() {
        moduleName = "gpio";
    }

    public void SetOutputPin(int pin, string mode) {
        SendCommand("setpin", new string[] { mode }); // TODO: this whole args thing is getting out of hand.
    }

}