
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobDetection : EdgeDetection {

    public static int maxBlobAmount = 1000;

    public int blobAmount;
    public Blob[] blobs;
    public bool[] gridVisited;

    public float blobWidthMin, blobHeightMin;

    public BlobDetection(int _w, int _h) : base(_w, _h) {

        gridVisited = new bool[gridValueAmount];

        blobs = new Blob[maxBlobAmount];
        blobAmount = 0;

        for(int i = 0; i < maxBlobAmount; i++) {
            blobs[i] = new Blob(this);
        }

        blobWidthMin = 0.0f;
        blobHeightMin = 0.0f;

        Debug.Log("BlobDetect is initialized");

    }

    public void SetMaxBlobAmount(int _amount) {
        maxBlobAmount = _amount;
    }

    public Blob GetBlob(int _index) {
        Blob b = null;
        if(_index < blobAmount) {
            return blobs[_index];
        }
        return b;
    }

    public int GetBlobAmount() {
        return blobAmount;
    }

    public void ComputeBlobs(Color32[] _pixels) {

        SetImage(_pixels);

        for(int i = 0; i < gridValueAmount; i++) {
            gridVisited[i] = false;
        }

        ComputeIsoValue();

        int x, y, squareIndex;
        int offset;
        float vx, vy;

        lineToDrawAmount = 0;
        vx = 0.0f;
        blobAmount = 0;

        for(x = 0; x < resX-1; x++) {
            vy = 0.0f;
            for(y = 0; y < resY-1; y++) {

                offset = x + resX*y;

                if(gridVisited[offset]) {
                    continue;
                }

                squareIndex = GetSquareIndex(x, y);

                if(squareIndex > 0 && squareIndex < 15) {
                    if(blobAmount < maxBlobAmount) {
                        FindBlob(blobAmount, x, y);
                        blobAmount++;
                    }
                }

                vy += stepY;

            }

            vx += stepX;

        }

        lineToDrawAmount /= 2;

    }

    public void FindBlob(int _iBlob, int _x, int _y) {

        if(_iBlob < 0 || _iBlob > blobs.Length) {
            return;
        }

        blobs[_iBlob].id = _iBlob;
        blobs[_iBlob].xMin = 1000.0f;
        blobs[_iBlob].xMax = -1000.0f;
        blobs[_iBlob].yMin = 1000.0f;
        blobs[_iBlob].yMax = -1000.0f;
        blobs[_iBlob].lineAmount = 0;

        ComputeEdgeVertex(_iBlob, _x, _y);

        if(blobs[_iBlob].xMin >= 1000.0f || blobs[_iBlob].xMax <= -1000.0f || blobs[_iBlob].yMin >= 1000.0f || blobs[_iBlob].yMax <= -1000.0f) {
            blobAmount--;
        }
        else {
            blobs[_iBlob].Update();
        }

        if(blobs[_iBlob].w < blobWidthMin || blobs[_iBlob].h < blobHeightMin) {
            blobAmount--;
        }

    }

    private void ComputeEdgeVertex(int _iBlob, int _x, int _y) {

        int offset = _x + resX*_y;

        if(gridVisited[offset]) {
            return;
        }

        gridVisited[offset] = true;

        int iEdge, offX, offY, offAB;
        int squareIndex = GetSquareIndex(_x, _y);
        float vx = _x*stepX;
        float vy = _y*stepY;

        int n = 0;

        while((iEdge = MetaballsTable.edgeCut[squareIndex, n++]) != -1) {
            offX = MetaballsTable.edgeOffsetInfo[iEdge, 0];
            offY = MetaballsTable.edgeOffsetInfo[iEdge, 1];
            offAB = MetaballsTable.edgeOffsetInfo[iEdge, 2];

            if(blobs[_iBlob].lineAmount < Blob.maxLineAmount) {
                lineToDraw[lineToDrawAmount++] = blobs[_iBlob].lines[blobs[_iBlob].lineAmount++] = voxel[_x+offX + resX*(_y+offY)] + offAB;
            }
            else {
                return;
            }
        }

        int toCompute = MetaballsTable.edgeToCompute[squareIndex];
        float t = 0.0f;
        float value = 0.0f;

        if(toCompute > 0) {
            if((toCompute & 1) > 0) {
                t = (isoValue - gridValue[offset]) / (gridValue[offset+1] - gridValue[offset]);
                value = vx * (1.0f-t) + t * (vx + stepX);
                edgeVert[voxel[offset]].x = value;

                if(value < blobs[_iBlob].xMin) {
                    blobs[_iBlob].xMin = value;
                }
                if(value > blobs[_iBlob].xMax) {
                    blobs[_iBlob].xMax = value;
                }
            }
            if((toCompute & 2) > 0) {
                t = (isoValue - gridValue[offset]) / (gridValue[offset+resX] - gridValue[offset]);
                value = vy * (1.0f-t) + t * (vy + stepY);
                edgeVert[voxel[offset]+1].y = value;

                if(value < blobs[_iBlob].yMin) {
                    blobs[_iBlob].yMin = value;
                }
                if(value > blobs[_iBlob].yMax) {
                    blobs[_iBlob].yMax = value;
                }
            }
        }

        byte neighborVoxel = MetaballsTable.neighborVoxel[squareIndex];
        
		if(_x < resX-2 && (neighborVoxel & (1<<0)) > 0) {
            ComputeEdgeVertex(_iBlob, _x + 1, _y);
        }
		if(_x > 0 && (neighborVoxel & (1<<1)) > 0) {
            ComputeEdgeVertex(_iBlob, _x - 1, _y);
        }
		if(_y < resY-2 && (neighborVoxel & (1<<2)) > 0) {
            ComputeEdgeVertex(_iBlob, _x, _y + 1);
        }
		if(_y > 0 && (neighborVoxel & (1<<3)) > 0) {
            ComputeEdgeVertex(_iBlob, _x, _y - 1);
        }
        
    }
    
}