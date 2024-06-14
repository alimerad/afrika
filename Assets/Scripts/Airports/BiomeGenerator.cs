using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public enum Biome { Plains, Mountains, Desert }
    public GameObject m_AirportPrefab;

    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float heightMultiplier = 20f;

    public int numTerrains = 3; // Number of terrains to generate (one for each biome)
    public Vector2 terrainSpacing = new Vector2(300, 300); // Spacing between terrains

    private Dictionary<Biome, Texture2D> biomeTextures;

    void Start()
    {
        LoadTextures();
        GenerateTerrains();
    }

    void LoadTextures()
    {
        biomeTextures = new Dictionary<Biome, Texture2D>();

        // Load all textures from the Resources/Textures folder
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Textures");

        foreach (Texture2D texture in textures)
        {
            if (texture.name.Contains("Grass"))
            {
                biomeTextures[Biome.Plains] = texture;
            }
            else if (texture.name.Contains("Rock"))
            {
                biomeTextures[Biome.Mountains] = texture;
            }
            else if (texture.name.Contains("Sand"))
            {
                biomeTextures[Biome.Desert] = texture;
            }
        }
    }

    void GenerateTerrains()
    {
        for (int i = 0; i < numTerrains; i++)
        {
            Biome biome = (Biome)i;
            Vector3 position = new Vector3(
                Random.Range(0, terrainSpacing.x * numTerrains),
                0,
                Random.Range(0, terrainSpacing.y * numTerrains)
            );

            CreateTerrain(biome, position);
        }
    }

    void CreateTerrain(Biome biome, Vector3 position)
    {
        GameObject terrainObject = new GameObject(biome.ToString() + "Terrain");
        terrainObject.transform.position = position;

        Terrain terrain = terrainObject.AddComponent<Terrain>();
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = width + 1,
            size = new Vector3(width, heightMultiplier, height)
        };

        terrain.terrainData = terrainData;
        terrainObject.AddComponent<TerrainCollider>().terrainData = terrainData;

        float[,] heights = GenerateHeights(biome);
        terrainData.SetHeights(0, 0, heights);

        ApplyTextures(terrain, terrainData, biome);

        PlaceAirports(terrainData, position, biome);
    }

    float[,] GenerateHeights(Biome biome)
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                sample = ApplyBiomeHeight(sample, biome);

                heights[x, y] = sample;
            }
        }
        return heights;
    }

    float ApplyBiomeHeight(float sample, Biome biome)
    {
        switch (biome)
        {
            case Biome.Plains:
                return sample * 0.3f; // Flatter terrain for plains
            case Biome.Mountains:
                return Mathf.Pow(sample, 3); // Higher and more uneven terrain for mountains
            case Biome.Desert:
                return sample * 0.1f; // Very flat terrain for desert
            default:
                return sample;
        }
    }

    void ApplyTextures(Terrain terrain, TerrainData terrainData, Biome biome)
    {
        if (biomeTextures.ContainsKey(biome))
        {
            // Create TerrainLayer
            TerrainLayer terrainLayer = new TerrainLayer
            {
                diffuseTexture = biomeTextures[biome],
                tileSize = new Vector2(30, 30) // Adjust tile size if necessary
            };

            // Assign TerrainLayer to the terrain
            terrainData.terrainLayers = new TerrainLayer[] { terrainLayer };

            // Apply the alphamaps to use the texture
            float[,,] alphaData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 1];
            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrainData.alphamapWidth; x++)
                {
                    alphaData[x, y, 0] = 1;
                }
            }

            terrainData.SetAlphamaps(0, 0, alphaData);
        }
    }

    void PlaceAirports(TerrainData terrainData, Vector3 terrainPosition, Biome biome)
    {
        int numAirports = Random.Range(1, 3); // Number of airports per biome
        for (int i = 0; i < numAirports; i++)
        {
            float x = Random.Range(0, terrainData.size.x);
            float y = Random.Range(0, terrainData.size.z);
            float height = terrainData.GetHeight((int)x, (int)y);

            Vector3 airportPosition = new Vector3(x, height, y) + terrainPosition;
            Instantiate(m_AirportPrefab, airportPosition, Quaternion.identity);
            Debug.Log("Airport placed in " + biome.ToString() + " biome at " + airportPosition);
        }
    }
}
