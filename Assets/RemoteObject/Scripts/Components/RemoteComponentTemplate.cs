using UnityEngine;

public class RemoteComponentTemplate : RemoteComponent {
    protected override void RemoteComponentAwake() {
        // Change this string to be this module's name.
        // This will be used as the first section of ALL commands from this component.
        // e.g. for audio moduleKeyword = "audio"
        //      SendCommand("play", [sound]) => /audio/play/[sound]
        moduleKeyword = "template";
    }

    public override void ActivateFallback() {
        // Code to be run when this RemoteComponent enters fallback mode.
        // For example, adding and configuring required components 
        // or, setting a bool to enable a standard gameplay implementation.
        // The base implementation prints a warning to the log.
        // base.ActivateFallback(); 
    }

    public override void DeactivateFallback() {
        // Code to be run when this RemoteComponent leaves fallback mode.
        // For example, removing added components for fallback mode, if applicable.
        // or, setting a bool to disable a standard gameplay implementation.
        // The base implementation prints a warning to the log.
        // base.DeactivateFallback();
    }
}