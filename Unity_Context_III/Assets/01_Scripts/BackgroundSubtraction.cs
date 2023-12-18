
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSubtraction {

    private WebCamTexture camTex;

    private Texture2D targetTex;
    private Texture2D calibratedTexture;

    public BackgroundSubtraction(ref WebCamTexture _camTex, ref Texture2D _target) {
        camTex = _camTex;
        targetTex = _target;
    }

    public void Calibrate() {
        Color32[] pixels = camTex.GetPixels32();
        calibratedTexture = new Texture2D(camTex.width, camTex.height);
        calibratedTexture.SetPixels32(pixels);
        calibratedTexture.Apply();
        Debug.Log("Calibrated");
    }

    public void Update() {
        Color32[] pixels = camTex.GetPixels32();

        pixels = ComputeDifference(pixels);

        targetTex.SetPixels32(pixels);
        targetTex.Apply();
    }

    private Color32[] ComputeDifference(Color32[] _pixels) {

        Color32[] output = new Color32[_pixels.Length];

        Color32[] background = calibratedTexture.GetPixels32();

        int threshold = 50;

        for(int i = 0; i < _pixels.Length; i++) {

            int diffR = Mathf.Abs(_pixels[i].r - background[i].r);
            int diffG = Mathf.Abs(_pixels[i].g - background[i].g);
            int diffB = Mathf.Abs(_pixels[i].b - background[i].b);

            if(diffR > threshold || diffG > threshold || diffB > threshold) {
                output[i] = new Color32(255, 255, 255, 255);
            }
            else {
                output[1] = new Color32(0, 0, 0, 255);
            }
            
        }

        return output;
        
    }

}