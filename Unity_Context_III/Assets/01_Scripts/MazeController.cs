
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {

    public static System.Action<Vector2> ReceiveVectorCall;
    public static System.Action ResetCall;
    public static System.Action StartCall;

    private Vector2 targetRotation;

    [SerializeField]
    private float maxRotationSpeed;
    [SerializeField, Range(0.0f, 45.0f)]
    private float maxAngle = 45.0f;

    public bool manualRotation = false;

    [SerializeField, Range(-45.0f, 45.0f)]
    private float x, z;

    [SerializeField]
    private Transform ballTransform;
    private Vector3 ballStartPosition;

    private bool canMove = false;

    private void Start() {
        ballStartPosition = ballTransform.position;
        ReceiveVectorCall += ReceiveVector;
        ResetCall += ResetMaze;
        StartCall += StartMaze;
    }

    private void FixedUpdate() {

        if(manualRotation) {
            x = Mathf.Clamp(x, -maxAngle, maxAngle);
            z = Mathf.Clamp(z, -maxAngle, maxAngle);
            targetRotation = new Vector2(x, z);
        }

        if(canMove) {
            RotateMaze(targetRotation);
        }

    }

    private void RotateMaze(Vector2 _vector) {
        Vector3 targetRot = new(_vector.x, 0.0f, _vector.y);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), maxRotationSpeed * Time.fixedDeltaTime);
    }

    private void ResetMaze() {
        transform.rotation = Quaternion.identity;
        ballTransform.position = ballStartPosition;
        ballTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        canMove = false;
    }

    private void StartMaze() {
        canMove = true;
    }

    private void ReceiveVector(Vector2 _vector) {
        if(!manualRotation) {
            targetRotation = new(
                _vector.x * maxAngle,
                _vector.y * maxAngle
            );
        }
    }
    
}