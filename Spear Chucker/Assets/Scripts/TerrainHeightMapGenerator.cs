using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainHeightMapGenerator : MonoBehaviour
{
    public int seed = 42;
    public float noiseScale = 20f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public float heightMultiplier = 20f;

    public Vector2 offset;

    Terrain terrain;
    TerrainData terrainData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }

    public void Generate()
    {
        Random.InitState(seed); //seed value helps us control the randomness of the terrain, so this line ask what value we'll use to determine the randomness
        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;
        float[,] heights = new float[w, h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float wx = (float)y / (w - 1);
                float hy = (float)y / (h - 1);

                //Sample layered Perlin noise
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; 1 < octaves; i++)
                {
                    float sampleX = (wx * noiseScale * frequency) + offset.x;
                    float sampleY = (hy * noiseScale * frequency) + offset.y;
                    float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f; //Perlin noise returns values between 0 and 1, so we multiply by 2 and subtract 1 to get values between -1 and 1
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[y, x] = Mathf.Clamp01((noiseHeight * 0.5f + 0.5f) * (heightMultiplier / 100f));
            }

        }

        terrainData.SetHeights(0, 0, heights);
    }
}
