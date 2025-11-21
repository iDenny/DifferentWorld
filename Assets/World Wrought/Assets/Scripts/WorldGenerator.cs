using UnityEngine;

/// <summary>
/// Generates a procedural terrain using Perlin noise.  This class can be attached
/// to a Terrain GameObject to generate varied landscapes for each playthrough.
/// Sample multiple noise layers for biomes (e.g., temperature, moisture) if desired【306984311763347†L27-L33】.
/// </summary>
[RequireComponent(typeof(Terrain))]
public class WorldGenerator : MonoBehaviour
{
    public int MapWidth = 256;
    public int MapLength = 256;
    public float NoiseScale = 20f;
    public float HeightMultiplier = 10f;
    public Vector2 NoiseOffset;

    private Terrain _terrain;

    private void Awake()
    {
        _terrain = GetComponent<Terrain>();
    }

    private void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        TerrainData data = _terrain.terrainData;
        data.heightmapResolution = MapWidth + 1;
        float[,] heights = new float[MapWidth, MapLength];
        float seedX = Random.Range(0f, 10000f) + NoiseOffset.x;
        float seedY = Random.Range(0f, 10000f) + NoiseOffset.y;
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapLength; z++)
            {
                float xCoord = seedX + (float)x / MapWidth * NoiseScale;
                float zCoord = seedY + (float)z / MapLength * NoiseScale;
                float sample = Mathf.PerlinNoise(xCoord, zCoord);
                heights[x, z] = sample * HeightMultiplier / data.size.y;
            }
        }
        data.size = new Vector3(MapWidth, HeightMultiplier, MapLength);
        data.SetHeights(0, 0, heights);
    }
}