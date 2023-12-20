
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WebcamViewer : MonoBehaviour {

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private Vector2Int textureSize;

    [SerializeField]
    private string webCamName;

    [SerializeField, Range(0.0f, 1.0f)]
    private float blobDetectThreshold = 0.5f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float vectorModifier = 0.5f;

    [SerializeField]
    private float minBlobSize = 0.01f;

    private WebCamTexture camTex;
    private Texture2D targetTex;

    private BackgroundSubtraction backSub;
    private BlobDetection blobDetect;

    private Vector3 dir;

    private void Start() {

        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textureSize.x);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textureSize.y);

        WebCamDevice[] devices = WebCamTexture.devices;

        foreach(WebCamDevice device in devices) {
            Debug.Log(device.name);
        }

        if(!devices.Contains(Array.Find(devices, x => x.name == webCamName))) {
            webCamName = devices[0].name;
        }

        camTex = new WebCamTexture(webCamName) {
            requestedFPS = 8,
            requestedWidth = textureSize.x,
            requestedHeight = textureSize.y
        };
        camTex.Play();

        targetTex = new Texture2D(camTex.width, camTex.height);

        backSub = new BackgroundSubtraction(ref camTex, ref targetTex);
        backSub.Calibrate();

        blobDetect = new BlobDetection(camTex.width, camTex.height);
        blobDetect.SetPosDiscrimination(true);
        blobDetect.SetThreshold(blobDetectThreshold);
        blobDetect.blobWidthMin = minBlobSize;
        blobDetect.blobHeightMin = minBlobSize;

        image.texture = targetTex;

    }

    private void Update() {
        if(camTex.didUpdateThisFrame) {
            backSub.Update();
            blobDetect.ComputeBlobs(targetTex.GetPixels32());
        }

        CalculateVector();
        MazeController.ReceiveVectorCall?.Invoke(dir);

        if(Input.GetKeyDown(KeyCode.Q)) {
            backSub.Calibrate();
            MazeController.ResetCall?.Invoke();
        }
    }

    private void CalculateVector() {

        dir = Vector3.zero;
        
        int blobAmount = blobDetect.blobAmount;
        if(blobAmount <= 0) {
            return;
        }

        for(int i = 0; i < blobAmount; i++) {
            Blob blob = blobDetect.GetBlob(i);
            float size = blob.w * image.rectTransform.rect.width * blob.h * image.rectTransform.rect.height * vectorModifier;
            dir += new Vector3(blob.x, blob.y, 0.0f) * size;
            dir = new Vector3(Map(dir.x, 0.0f, size, -1.0f, 1.0f), Map(dir.y, 0.0f, size, -1.0f, 1.0f), 0.0f);
        }

        dir /= blobAmount;
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;

        Vector2 imageSize = image.rectTransform.rect.size * image.rectTransform.localScale;
        Vector3 imagePos = new(image.transform.position.x - imageSize.x / 2, image.canvas.transform.position.y - imageSize.y / 2, 0.0f);
 
        if(blobDetect != null) {
            for(int i = 0; i < blobDetect.blobAmount; i++) {

                Vector3 blobPos = imagePos + new Vector3(blobDetect.GetBlob(i).x * imageSize.x, blobDetect.GetBlob(i).y * imageSize.y, 0.0f);
                Vector3 blobSize = new(imageSize.x * blobDetect.GetBlob(i).w, imageSize.y * blobDetect.GetBlob(i).h, 1.0f);
                Gizmos.DrawWireCube(blobPos, blobSize);
            }
        }

        if(dir.magnitude >= Mathf.Epsilon) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(image.transform.position, image.transform.position + dir);
        }
    }

    private float Map(float value, float low1, float high1, float low2, float high2) {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

}