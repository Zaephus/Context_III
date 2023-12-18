
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob {

    public BlobDetection parent;

    public int id;
    public float x, y;
    public float w, h;
    public float xMin, xMax, yMin, yMax;

    public int[] lines;
    public int lineAmount;

    public static int maxLineAmount = 4000;

    public Blob(BlobDetection _parent) {
        parent = _parent;
        lines = new int[maxLineAmount];
        lineAmount = 0;
    }

    public EdgeVertex GetEdgeVertexA(int _iEdge) {
        if(_iEdge * 2 < parent.lineToDrawAmount * 2) {
            return parent.GetEdgeVertex(lines[_iEdge * 2]);
        }
        else {
            return null;
        }
    }

    public EdgeVertex GetEdgeVertexB(int _iEdge) {
        if((_iEdge * 2 + 1) < parent.lineToDrawAmount * 2) {
            return parent.GetEdgeVertex(lines[_iEdge*2 + 1]);
        }
        else {
            return null;
        }
    }

    public int GetEdgeAmount() {
        return lineAmount;
    }

    public void Update() {
        w = xMax - xMin;
        h = yMax - yMin;
        x = 0.5f * (xMax + xMin);
        y = 0.5f * (yMax + yMin);

        lineAmount /= 2;
    }

    public override string ToString() {
        return $"{x}, {y}";
    }

}