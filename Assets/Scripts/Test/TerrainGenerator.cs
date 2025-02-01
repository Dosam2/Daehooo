using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int terrainSize = 200;
    public float noiseScale = 0.1f;
    
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData data)
    {
        data.size = new Vector3(terrainSize, 50, terrainSize);
        data.SetHeights(0, 0, GenerateHeights());
        return data;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainSize, terrainSize];
        for(int x=0; x<terrainSize; x++)
        {
            for(int y=0; y<terrainSize; y++)
            {
                heights[x,y] = Mathf.PerlinNoise(x*noiseScale, y*noiseScale) * 0.1f;
            }
        }
        return heights;
    }
}
