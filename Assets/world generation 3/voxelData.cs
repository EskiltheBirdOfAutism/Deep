using UnityEngine;

public static class voxelData
{
    public static readonly int Chunkwith = 16;
    public static readonly int Chunkheght = 128;
    public static readonly int worldSizeInChunks = 100;
    public static int WorldSizeInVoxels { get { return worldSizeInChunks * Chunkwith; } }

    public static readonly int wiewDistanceInChunks = 5;

    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {

        get { return 1f / (float)TextureAtlasSizeInBlocks; }

    }


    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f,0.0f,0.0f),
        new Vector3(1.0f,0.0f,0.0f),
        new Vector3(1.0f,1.0f,0.0f),
        new Vector3(0.0f,1.0f,0.0f),
        new Vector3(0.0f,0.0f,1.0f),
        new Vector3(1.0f,0.0f,1.0f),
        new Vector3(1.0f,1.0f,1.0f),
        new Vector3(0.0f,1.0f,1.0f),
    };
    public static readonly Vector3[] faceCheks = new Vector3[6] {
        new Vector3 (0.0f,0.0f,-1.0f),
        new Vector3 (0.0f,0.0f,1.0f),
        new Vector3 (0.0f,1.0f,0.0f),
        new Vector3 (0.0f,-1.0f,0.0f),
        new Vector3 (-1.0f,0.0f,0.0f),
        new Vector3 (1.0f,0.0f,0.0f)
        };

    public static readonly int[,] voxelTris = new int[6, 4]
    {
    { 0, 3, 1, 2  },//back face
    { 5, 6, 4, 7 },//front
    { 3, 7, 2, 6},//top
    { 1, 5, 0, 4},//bottom
    { 4, 7, 0, 3},//left
    { 1, 2, 5, 6}//right
    };
    //public static readonly Vector2[] voxelUvs = new Vector2[6]
    //{
      //  new Vector2 (0.0f,0.0f),
        //new Vector2 (0.0f,1.0f),
        //new Vector2 (1.0f,0.0f),
        //new Vector2 (1.0f,0.0f),
        //new Vector2 (0.0f,1.0f),
        //new Vector2 (1.0f,1.0f)
    //};
    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0.0f,0.0f),
        new Vector2(0.0f,1.0f),
        new Vector2(1.0f,0.0f),
        new Vector2(1.0f,1.0f)
    };

}

