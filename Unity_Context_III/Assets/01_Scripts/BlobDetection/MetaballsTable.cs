
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class MetaballsTable {

    public static int[,] edgeCut = {
        {-1, -1, -1, -1, -1 }, //0
        { 0,  3, -1, -1, -1 }, //3
        { 0,  1, -1, -1, -1 }, //1
        { 3,  1, -1, -1, -1 }, //2
        { 1,  2, -1, -1, -1 }, //0
        { 1,  2,  0,  3, -1 }, //3
        { 0,  2, -1, -1, -1 }, //1
        { 3,  2, -1, -1, -1 }, //2
        { 3,  2, -1, -1, -1 }, //2
        { 0,  2, -1, -1, -1 }, //1
        { 1,  2,  0,  3, -1 }, //3
        { 1,  2, -1, -1, -1 }, //0
        { 3,  1, -1, -1, -1 }, //2
        { 0,  1, -1, -1, -1 }, //1
        { 0,  3, -1, -1, -1 }, //3
        {-1, -1, -1, -1, -1 }  //0
    };

    public static int[,] edgeOffsetInfo = {
        { 0, 0, 0 },
        { 1, 0, 1 },
        { 0, 1, 0 },
        { 0, 0, 1 }
    };

    public static int[] edgeToCompute = { 0, 3, 1, 2, 0, 3, 1, 2, 2, 1, 3, 0, 2, 1, 3, 0 };

    // neighborVoxel Array
    // ------------------------------
    // bit 0 : X+1
    // bit 1 : X-1
    // bit 2 : Y+1
    // bit 3 : Y-1
    public static byte[] neighborVoxel = { 0, 10, 9, 3, 5, 15, 12, 6, 6, 12, 12, 5, 3, 9, 10, 0 };  
    public static void ComputeNeighborTable() {

        int iEdge;
        int n;

        for (int i=0 ; i<16 ; i++) {
            
            neighborVoxel[i] = 0;

            n = 0;
            while ((iEdge = edgeCut[i, n++]) != -1) {
                switch (iEdge) {
                case 0:
                    neighborVoxel[i] |= 1<<3;     
                    break;
                case 1:
                    neighborVoxel[i] |= 1<<0;     
                    break;
                case 2:
                    neighborVoxel[i] |= 1<<2;     
                    break;
                case 3:
                    neighborVoxel[i] |= 1<<1;     
                    break;
                }

            }    
            
        }

    
    }
    
}