
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpOSC;

public class MessageReceiver : MonoBehaviour {

    private UDPListener listener;

    private static bool isPaused;

    HandleOscPacket callback = delegate(OscPacket _packet) {
        OscMessage message = (OscMessage)_packet;

        switch(message.Address) {
            case "/Zaephus/Vector":
                HandleVector(message);
                break;
            default:
                Debug.Log(message.Address);
                break;
        }
    };

    private static void HandleVector(OscMessage _message) {
        if(!isPaused) {
            Vector2 receivedVec = new((float)_message.Arguments[0], (float)_message.Arguments[1]);
            MazeController.ReceiveVectorCall?.Invoke(receivedVec);
        }
    }

    private void Start() {
        listener = new UDPListener(6201, callback);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) {
            OscMessage message = new("/Zaephus/Calibrate", 23, 42.0f);
            UDPSender sender = new("10.3.26.89", 6200);
            sender.Send(message);
            MazeController.ResetCall?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            MazeController.StartCall?.Invoke();
        }
    }

    private void OnDisable() {
       listener.Close();
    }

}