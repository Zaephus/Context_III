
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpOSC;

public class MessageReceiver : MonoBehaviour {

    UDPListener listener;

    HandleOscPacket callback = delegate(OscPacket _packet) {
        OscMessage message = (OscMessage)_packet;
        Debug.Log(message.Address);

        switch(message.Address) {
            case "/Zaephus/Vector":
                Vector2 receivedVec = new((float)message.Arguments[0], (float)message.Arguments[1]);
                MazeController.ReceiveVectorCall?.Invoke(receivedVec);
                break;
        }
    };

    private void Start() {
        listener = new UDPListener(6201, callback);

        // OscMessage message = new OscMessage("/test/1", 23, 42.0f);
        // UDPSender sender = new UDPSender("192.168.1.45", 6200);
        // sender.Send(message);
    }

    private void Update() {

    }

    private void OnDisable() {
       listener.Close();
    }

}