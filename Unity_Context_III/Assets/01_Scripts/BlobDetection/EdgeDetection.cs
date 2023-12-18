
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : Metaballs2D {

    public static byte C_R = 0x01;
    public static byte C_G = 0x02;
    public static byte C_B = 0x03;

    public int imgWidth, imgHeight;
    public Color32[] pixels;
    public bool posDiscrimination;

    public float coefficient = 3.0f * 255.0f;

    public EdgeDetection(int _w, int _h) {
        imgWidth = _w;
        imgHeight = _h;

        Init(imgWidth, imgHeight);

        posDiscrimination = false;
    }

    public void SetPosDiscrimination(bool _value) {
        posDiscrimination = _value;
    }

    public void SetThreshold(float _value) {
        _value = Mathf.Clamp01(_value);
        SetIsoValue(_value * coefficient);
    }

    public void SetImage(Color32[] _pixels) {
        pixels = _pixels;
    }

    public void ComputeEdges(Color32[] _pixels) {
        SetImage(_pixels);
        ComputeMesh();
    }

    public override void ComputeIsoValue() {
        
        Color32 pixel; 
        int r, g, b;
        int x, y;
        int offset;

        r = 0;
        g = 0;
        b = 0;

        for(y = 0; y < imgHeight; y++) {
            for(x = 0; x < imgWidth; x++) {
                offset = x + imgWidth*y;

                pixel = pixels[offset];
                r = pixel.r;
                g = pixel.g;
                b = pixel.b;

                gridValue[offset] = (float)(r + g + b);
            }
        }

    }

    protected override int GetSquareIndex(int _x, int _y) {

        int squareIndex = 0;
        int offY = resX * _y;
        int offY1 = resX * (_y+1);

        if(!posDiscrimination) {
            if(gridValue[_x + offY] < isoValue) {
                squareIndex |= 1;
            }
            if(gridValue[_x + 1 + offY] < isoValue) {
                squareIndex |= 2;
            }
            if(gridValue[_x + 1 + offY1] < isoValue) {
                squareIndex |= 4;
            }
            if(gridValue[_x + offY1] < isoValue) {
                squareIndex |= 8;
            }
        }
        else {
            if(gridValue[_x + offY] > isoValue) {
                squareIndex |= 1;
            }
            if(gridValue[_x + 1 + offY] > isoValue) {
                squareIndex |= 2;
            }
            if(gridValue[_x + 1 + offY1] > isoValue) {
                squareIndex |= 4;
            }
            if(gridValue[_x + offY1] > isoValue) {
                squareIndex |= 8;
            }
        }

        return squareIndex;

    }

    public EdgeVertex GetEdgeVertex(int _index) {
        return edgeVert[_index];
    }

}