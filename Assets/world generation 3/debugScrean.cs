using UnityEngine;
using UnityEngine.UI;

public class debugScrean : MonoBehaviour
{/*
    World world;
    Text text;

    float frameRate;
    float timer;

    int halfWorldSizeInVoxels;
    int halfWorldSizeInChunks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        world = GameObject.Find("world").GetComponent<World>();
        text.GetComponent<Text>();

        halfWorldSizeInVoxels = voxelData.WorldSizeInVoxels / 2;
        halfWorldSizeInChunks = voxelData.worldSizeInChunks / 2;

    }

    // Update is called once per frame
    void Update()
    {
        string degugText = "deep";
        degugText += "\n";
        degugText += frameRate + " fps";
        degugText += "\n\n";
        degugText += "XYZ: " + (Mathf.FloorToInt(world.player.transform.position.x) - halfWorldSizeInVoxels) + " / " + Mathf.FloorToInt(world.player.transform.position.y) + " / " + (Mathf.FloorToInt(world.player.transform.position.z) - halfWorldSizeInVoxels);
        degugText += "\n";
        degugText += "Chunk: " + (world.playerChunkCoord.x - halfWorldSizeInChunks) + " / " + (world.playerChunkCoord.z - halfWorldSizeInChunks);



        text.text = degugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }*/
}
