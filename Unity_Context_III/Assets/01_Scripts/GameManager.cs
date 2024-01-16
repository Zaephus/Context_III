
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private void Start() {
        for(int i = 1; i < Display.displays.Length; i++) {
            Display.displays[i].Activate();
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

}