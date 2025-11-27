using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public biome Biome;

    public Transform player;
    public Vector3 spawn;

    public Material material;
    public blockType[] blocktypes;

    chunk[,] chunks = new chunk[voxelData.worldSizeInChunks, voxelData.worldSizeInChunks];

    List<ChunkCoord> activeChunk = new List<ChunkCoord>();

    
    public ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    private bool isCreatingChunks;

    public GameObject debugScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Random.InitState(seed);
        spawn = new Vector3((voxelData.worldSizeInChunks * voxelData.Chunkwith) / 2f, voxelData.Chunkheght + 2f, (voxelData.worldSizeInChunks * voxelData.Chunkwith) / 2f);
        GenerateWorld();
        //playerLastChunkCoord = GetChunkcoordFromVector3(player.position);
    }

    // Update is called once per frame
    void Update()
    {
        //playerChunkCoord = GetChunkcoordFromVector3(player.position);

        //if (!playerChunkCoord.Equals(playerLastChunkCoord))
          //  CheckViewDistance();
        if(chunksToCreate.Count>0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");
        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);
    }
    void GenerateWorld()
    {

        int _i = 0;
        for (int x = (voxelData.worldSizeInChunks / 2) - voxelData.wiewDistanceInChunks; x < (voxelData.worldSizeInChunks / 2) + voxelData.wiewDistanceInChunks; x++)
        {
            for (int z = (voxelData.worldSizeInChunks / 2) - voxelData.wiewDistanceInChunks; z < (voxelData.worldSizeInChunks / 2) + voxelData.wiewDistanceInChunks; z++)
            {
                chunks[x, z] = new chunk(new ChunkCoord(x,z),this,true,_i,GetComponent<NetworkTransformChild>());
                activeChunk.Add(new ChunkCoord(x, z));
                _i++;
            }
        }
       // player.position = spawn;


    }
    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;
        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].z].Init();
            
            chunksToCreate.RemoveAt(0);
            yield return null;
        }
        isCreatingChunks = false;
    }
    ChunkCoord GetChunkcoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / voxelData.Chunkwith);
        int z = Mathf.FloorToInt(pos.z / voxelData.Chunkwith);
        return new ChunkCoord(x, z);

    }
    public chunk GetChunkFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / voxelData.Chunkwith);
        int z = Mathf.FloorToInt(pos.z / voxelData.Chunkwith);
        return chunks[x, z];
    }

    private void CheckViewDistance()
    {
        //ChunkCoord coord = GetChunkcoordFromVector3(player.position);
       // playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunk);
        /*
        int _i = 0;
        // Loop through all chunks currently within view distance of the player.
        for (int x = coord.x - voxelData.wiewDistanceInChunks; x < coord.x + voxelData.wiewDistanceInChunks; x++)
        {
            for (int z = coord.z - voxelData.wiewDistanceInChunks; z < coord.z + voxelData.wiewDistanceInChunks; z++)
            {

                // If the current chunk is in the world...
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {

                    // Check if it active, if not, activate it.
                    if (chunks[x, z] == null)
                    {
                        //CreateNewChunk(x, z);
                        chunks[x, z] = new chunk(new ChunkCoord(x, z), this, false, _i, GetComponent<NetworkTransformChild>());
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }else if (!chunks[x, z].isActive)
                    {
                        chunks[x, z].isActive = true;
                        
                    }
                    activeChunk.Add(new ChunkCoord(x, z));
                    _i++;
                }

                // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {

                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);

                }


            }
        }
        foreach (ChunkCoord c in previouslyActiveChunks)
            chunks[c.x, c.z].isActive = false;
        */
    }
    public bool checkForVoxel(Vector3 pos)
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > voxelData.Chunkheght)
            return false;
        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            return blocktypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;

        return blocktypes[GetVoxel(pos)].isSolid;
    }
    public byte GetVoxel(Vector3 pos)
    {
        int Ypos = Mathf.FloorToInt(pos.y);

        if (!IsVoxelInWorld(pos)) 
        return 0;
        if (Ypos == 0)
            return 1;
        int terrainHeight = Mathf.FloorToInt(Biome.terrainHeight * noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, Biome.terrainScale)) + Biome.solidGroundHeight;
        byte voxelValue = 0;

        if (Ypos == terrainHeight)
            voxelValue = 3;
        else if (Ypos < terrainHeight && Ypos > terrainHeight - 4)
            voxelValue = 5;
        else if (Ypos > terrainHeight)
            return 0;
        else
            voxelValue = 2;

        /* SECOND PASS */

        if (voxelValue == 2)
        {

            foreach (Lode lode in Biome.lodes)
            {

                if (Ypos > lode.minHeight && Ypos < lode.maxHeight)
                    if (noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;

            }

        }
        return voxelValue;
    }
    /*void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new chunk(new ChunkCoord(x, z), this);
        activeChunk.Add(new ChunkCoord(x, z));
    }*/
    bool IsChunkInWorld(ChunkCoord coord)
    {

        if (coord.x > 0 && coord.x < voxelData.worldSizeInChunks - 1 && coord.z > 0 && coord.z < voxelData.worldSizeInChunks - 1)
            return true;
        else
            return
                false;

    }

    bool IsVoxelInWorld(Vector3 pos)
    {

        if (pos.x >= 0 && pos.x < voxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < voxelData.Chunkheght && pos.z >= 0 && pos.z < voxelData.WorldSizeInVoxels)
            return true;
        else
            return false;

    }
    [System.Serializable]
    public class blockType
    {
        public string blockName;
        public bool isSolid;

        [Header("Texture Values")]
        public int backFaceTexture;
        public int frontFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;

        // Back, Front, Top, Bottom, Left, Right

        public int GetTextureID(int faceIndex)
        {

            switch (faceIndex)
            {

                case 0:
                    return backFaceTexture;
                case 1:
                    return frontFaceTexture;
                case 2:
                    return topFaceTexture;
                case 3:
                    return bottomFaceTexture;
                case 4:
                    return leftFaceTexture;
                case 5:
                    return rightFaceTexture;
                default:
                    Debug.Log("Error in GetTextureID; invalid face index");
                    return 0;


            }

        }

    } 
}
