
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpOSC;

public class MessageReceiver : MonoBehaviour {

    UDPListener listener;

    HandleOscPacket callback = delegate(OscPacket _packet) {
        OscMessage message = (OscMessage)_packet;
        Debug.Log(message.Address);
        for(int i = 0; i < message.Arguments.Count; i++) {
            Debug.Log(message.Arguments[i]);
        }
    };

    private void Start() {
        listener = new UDPListener(6201, callback);
    }

    private void Update() {

    }

    private void OnDisable() {
       listener.Close();
    }

}