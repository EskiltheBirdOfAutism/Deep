using UnityEngine;
[CreateAssetMenu(fileName="biome",menuName ="Biome Atribut")]
public class biome : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

    public Lode[] lodes;

}

[System.Serializable]
public class Lode
{

    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
