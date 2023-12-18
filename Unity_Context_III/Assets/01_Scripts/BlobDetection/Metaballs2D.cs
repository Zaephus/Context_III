
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metaballs2D {

    protected float isoValue;

    protected int resX, resY;
    protected float stepX, stepY;
    protected float[] gridValue;
    protected int gridValueAmount;

    protected int[] voxel;
    protected int voxelAmount;

    protected EdgeVertex[] edgeVert;
    protected int edgeVertAmount;

    public int[] lineToDraw;
    public int lineToDrawAmount;

    public Metaballs2D() {}

    public void Init(int _resX, int _resY) {

        resX = _resX;
        resY = _resY;

        stepX = 1.0f / (resX-1);
        stepY = 1.0f / (resY-1);

        gridValueAmount = resX*resY;
        gridValue = new float[gridValueAmount];

        voxelAmount = gridValueAmount;
        voxel = new int[voxelAmount];

        edgeVertAmount = 2 * voxelAmount;
        edgeVert = new EdgeVertex[edgeVertAmount];

        lineToDraw = new int[2 * voxelAmount];
        lineToDrawAmount = 0;

        int x, y, n, index;
        n = 0;
        for(x = 0; x < resX; x++) {
            for(y = 0; y < resY; y++) {
                index = 2*n;

                voxel[x + resX*y] = index;

                edgeVert[index] = new EdgeVertex(x * stepX, y * stepY);
                edgeVert[index+1] = new EdgeVertex(x * stepX, y * stepY);
            }
        }

    }

    public virtual void ComputeIsoValue() {}

    public void ComputeMesh() {

        ComputeIsoValue();

        int x, y, squareIndex, n;
        int iEdge;
        int offX, offY, offAB;
        int toCompute;
        int offset;
        float t;
        float vx, vy;

        lineToDrawAmount = 0;
        vx = 0.0f;
        
        for(x = 0; x < resX-1; x++) {
            vy = 0.0f;

            for(y = 0; y < resY-1; y++) {

                offset = x + resX*y;
                squareIndex = GetSquareIndex(x, y);

                n = 0;

                while((iEdge = MetaballsTable.edgeCut[squareIndex, n++]) != -1) {
                    
                    offX = MetaballsTable.edgeOffsetInfo[iEdge, 0];
                    offY = MetaballsTable.edgeOffsetInfo[iEdge, 1];
                    offAB = MetaballsTable.edgeOffsetInfo[iEdge, 2];

                    lineToDraw[lineToDrawAmount++] = voxel[(x + offX) + resX*(y + offY)] + offAB;
                }

                toCompute = MetaballsTable.edgeToCompute[squareIndex];

                if(toCompute > 0) {
                    if((toCompute & 1) > 0) {
                        t = (isoValue - gridValue[offset]) / (gridValue[offset+1] - gridValue[offset]);
                        edgeVert[voxel[offset]].x = vx*(1.0f - t) + t*(vx + stepX);
                    }
                    if((toCompute & 2) > 0) {
                        t = (isoValue - gridValue[offset]) / (gridValue[offset + resX] - gridValue[offset]);
                        edgeVert[voxel[offset]+1].y = vy*(1.0f - t) + t*(vy + stepY);
                    }
                }

                vy += stepY;
            
            }

            vx += stepX;

        }

        lineToDrawAmount /= 2;

    }

    protected virtual int GetSquareIndex(int _x, int _y) {

        int squareIndex = 0;
        int offY = resX*_y;
        int offY1 = resX*(_y+1);

        if(gridValue[_x+offY] < isoValue) {
            squareIndex |= 1;
        }
        if(gridValue[_x+1+offY] < isoValue) {
            squareIndex |= 2;
        }
        if(gridValue[_x+1+offY1] < isoValue) {
            squareIndex |= 3;
        }
        if(gridValue[_x+offY1] < isoValue) {
            squareIndex |= 4;
        }

        return squareIndex;

    }

    public void SetIsoValue(float _iso) {
        isoValue = _iso;
    } 
    
}