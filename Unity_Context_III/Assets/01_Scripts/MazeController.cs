
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {

    [SerializeField]
    private Vector3 dir;

    [SerializeField, Range(-90, 90)]
    private float x, y, z;

    private void Update() {
        dir = new Vector3(x, y, z);
        transform.eulerAngles = dir;
    }
    
}