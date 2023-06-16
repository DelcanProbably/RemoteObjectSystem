using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemoteIdentificationIPUI : MonoBehaviour
{
    static Dictionary<string, RemoteIdentificationIPUI> mes;
    [SerializeField] TMP_Text ipText;
    [SerializeField] Image statusIcon;
    [SerializeField] Sprite unconfirmedIcon;
    [SerializeField] Sprite confirmedIcon;
    bool isConfirmed = false;

    public void Initialise (string ip) {
        ipText.text = ip;
        mes.Add(ip, this);
    }

    void UpdateStatusIcon() {
        statusIcon.sprite = isConfirmed ? confirmedIcon : unconfirmedIcon;
    }

    void Confirm() {
        isConfirmed = true;
        UpdateStatusIcon();
    }

    public static void ConfirmByIP(string ip) {
        mes[ip].Confirm();
    }

    public static void NewSweep() {
        // Create a new dictionary or clear an existing one.
        if (mes == null) {
            mes = new();
        } else {
            mes.Clear();
        }
    }

}
