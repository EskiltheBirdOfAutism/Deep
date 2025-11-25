using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class chunk 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ChunkCoord coord;

    GameObject chunkObjekt;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    int vertaxIndex = 0;
    List<Vector3> verteces = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[voxelData.Chunkwith, voxelData.Chunkheght, voxelData.Chunkwith];
    World world;

    private bool _isActive;
    public bool isVoxelMapPopulated = false;
    public chunk (ChunkCoord _coord,World _world, bool generateOnLoad)
    {
        coord = _coord;
        world = _world;
        if (generateOnLoad)
            Init();

        
    }
    public void Init()
    {
        chunkObjekt = new GameObject();
        meshFilter = chunkObjekt.AddComponent<MeshFilter>();
        meshRenderer = chunkObjekt.AddComponent<MeshRenderer>();
        meshCollider = chunkObjekt.AddComponent<MeshCollider>();

        meshRenderer.material = world.material;
        chunkObjekt.transform.SetParent(world.transform);
        chunkObjekt.transform.position =new Vector3(coord.x * voxelData.Chunkwith, 0f, coord.z * voxelData.Chunkwith);
        chunkObjekt.name = "Chunk " + coord.x + ", " + coord.z;
        populateVoxelMap();
        UpdateChunk();
        /*CreateMechData();
        CreateMesh();*/
    }
    
    void Start()
    {
        

        

       /* for(int p = 0; p < 6; p++)
        {
           for(int i = 0; i < 6; i++)
            {
                int triangleIndex = voxelData.voxelTris[p, i];
                vertices.Add(voxelData.voxelVerts[triangleIndex]);
                triangels.Add(vertexIndex);

                uvs.Add(voxelData.voxelVerts[i]);

                vertexIndex++;
            }
        }
        */
    }
    void populateVoxelMap()
    {
        for (int y=0; y<voxelData.Chunkheght; y++)
        {
            for (int x = 0; x < voxelData.Chunkwith; x++)
            {
                for (int z = 0; z < voxelData.Chunkwith; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
        isVoxelMapPopulated = true;

    }
    void UpdateChunk()
    {
        ClearMeshData();
        for (int y = 0; y < voxelData.Chunkheght; y++)
        {
            for (int x = 0; x < voxelData.Chunkwith; x++)
            {
                for (int z = 0; z < voxelData.Chunkwith; z++)
                {
                    if (world.blocktypes[voxelMap[x,y,z]].isSolid)
                        UpdateMeshData(new Vector3(x, y, z));
                }
            }
        }
        CreateMesh();
    }
    void ClearMeshData() 
    {
        vertaxIndex = 0;
        verteces.Clear();
        triangles.Clear();
        uvs.Clear();
    }
    /*void CreateMechData()
    {
        for (int y = 0; y < voxelData.Chunkheght; y++)
        {
            for (int x = 0; x < voxelData.Chunkwith; x++)
            {
                for (int z = 0; z < voxelData.Chunkwith; z++)
                {
                    if (world.blocktypes[voxelMap[x, y, z]].isSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }*/
    public bool isActive
    {

        get { return _isActive; }
        set 
        {
            _isActive = value;
            if (chunkObjekt != null)
                chunkObjekt.SetActive(value);
        }

    }

    public Vector3 position
    {

        get { return chunkObjekt.transform.position; }

    }
    bool IsVoxelInChunk (int x, int y, int z)
    {
        if (x < 0 || x > voxelData.Chunkwith - 1 || y < 0 || y > voxelData.Chunkheght - 1 || z < 0 || z > voxelData.Chunkwith - 1) return false;
        else
            return true;
        

    }
    public void EditVoxel(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObjekt.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObjekt.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        UpdateChunk();

    }
    void UpdateSurroundingVoxels (int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + voxelData.faceCheks[p];
            if (!IsVoxelInChunk((int)currentVoxel.x,(int)currentVoxel.y, (int)currentVoxel.z))
            {
                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
            }
        }
    }
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.checkForVoxel(pos + position);
        
        return world.blocktypes[voxelMap[x, y, z]].isSolid;
                
                
                

    }
    public byte GetVoxelFromGlobalVector3(Vector3 pos)
    {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObjekt.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObjekt.transform.position.z);

        return voxelMap[xCheck, yCheck, zCheck];

    }

    void UpdateMeshData(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + voxelData.faceCheks[p]))
            {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
                verteces.Add(pos + voxelData.voxelVerts[voxelData.voxelTris[p, 0]]);
                verteces.Add(pos + voxelData.voxelVerts[voxelData.voxelTris[p, 1]]);
                verteces.Add(pos + voxelData.voxelVerts[voxelData.voxelTris[p, 2]]);
                verteces.Add(pos + voxelData.voxelVerts[voxelData.voxelTris[p, 3]]);
                AddTexture(world.blocktypes[blockID].GetTextureID(p));
                /*uvs.Add(voxelData.voxelUvs[0]);
                uvs.Add(voxelData.voxelUvs[1]);
                uvs.Add(voxelData.voxelUvs[2]);
                uvs.Add(voxelData.voxelUvs[3]);*/
                triangles.Add(vertaxIndex);
                triangles.Add(vertaxIndex + 1);
                triangles.Add(vertaxIndex + 2);
                triangles.Add(vertaxIndex + 2);
                triangles.Add(vertaxIndex + 1);
                triangles.Add(vertaxIndex + 3);

                /*triangles.Add(vertaxIndex + 0);
                triangles.Add(vertaxIndex + 1);
                triangles.Add(vertaxIndex + 2);

                triangles.Add(vertaxIndex + 0);
                triangles.Add(vertaxIndex + 2);
                triangles.Add(vertaxIndex + 3);*/
                vertaxIndex += 4;
                
            }
            
        }
        
    }
   void CreateMesh()
    {
        // DEBUG: Check array sizes
        
        Mesh mesh = new Mesh();
        mesh.vertices = verteces.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
            
        }
    }
    void AddTexture (int textureID)
    {
        float y = textureID / voxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * voxelData.TextureAtlasSizeInBlocks);

        x *= voxelData.NormalizedBlockTextureSize;
        y *= voxelData.NormalizedBlockTextureSize;

        y = 1f - y - voxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + voxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + voxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + voxelData.NormalizedBlockTextureSize, y + voxelData.NormalizedBlockTextureSize));

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ChunkCoord
{
    public int x;
    public int z;
    public ChunkCoord()
    {
        x = 0;
        z = 0;
    }
    public ChunkCoord(int _x, int _Z)
    {
        x = _x;
        z = _Z;
    }
    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / voxelData.Chunkwith;
        z = zCheck / voxelData.Chunkwith;
    }
    public bool equals(ChunkCoord other)
    {

        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;

    }
    
}
