using System.Collections.Generic;
using UnityEngine;

public class AirportGenerator : MonoBehaviour
{
    public GameObject m_ControlTowerPrefab;
    public GameObject m_AirportBuildingPrefab;
    public GameObject m_HangarPrefab;
    public GameObject m_RunwayPrefab;

    public int m_GridWidth = 20;
    public int m_GridHeight = 20;
    public float m_CellSize = 10f;
    public int m_ZoneSize = 5; // Taille de chaque zone carrée
    public float m_SpaceBetweenHangars = 1f; // Espacement entre les hangars

    private int[,] m_Grid;

    void Start()
    {
        m_Grid = new int[m_GridWidth, m_GridHeight];
        GenerateAirportGrid();
        InstantiateAirport();
    }

    void GenerateAirportGrid()
    {
        // Initialiser la grille à 0 (vide)
        for (int x = 0; x < m_GridWidth; x++)
        {
            for (int y = 0; y < m_GridHeight; y++)
            {
                m_Grid[x, y] = 0;
            }
        }

        // Diviser le tableau en zones carrées et placer les bâtiments
        PlaceBuildingInZone(1, 0, 0); // Tour de contrôle en haut à gauche
        PlaceBuildingInZone(2, m_GridWidth - m_ZoneSize, 0); // Bâtiment de l'aéroport en haut à droite
        PlaceHangarsInZone(m_ZoneSize, m_GridHeight - m_ZoneSize); // Hangars en bas à gauche
        PlaceRunwayInZone(m_GridWidth - m_ZoneSize, m_GridHeight - m_ZoneSize, 2); // Pistes en bas à droite
    }

    void PlaceBuildingInZone(int buildingType, int startX, int startY)
    {
        bool placed = false;
        while (!placed)
        {
            int x = Random.Range(startX, startX + m_ZoneSize);
            int y = Random.Range(startY, startY + m_ZoneSize);

            if (m_Grid[x, y] == 0)
            {
                m_Grid[x, y] = buildingType;
                placed = true;
            }
        }
    }

    void PlaceHangarsInZone(int startX, int startY)
    {
        Vector2Int hangarSize = GetPrefabSizeInGridUnits(m_HangarPrefab);
        bool horizontal = Random.value > 0.5f;

        // Calculate the maximum number of hangars that can fit in the zone
        int maxHangars = horizontal ? (m_ZoneSize / (hangarSize.x + (int)(m_SpaceBetweenHangars / m_CellSize))) : (m_ZoneSize / (hangarSize.y + (int)(m_SpaceBetweenHangars / m_CellSize)));
        int hangarCount = Random.Range(1, maxHangars + 1);

        bool placed = false;
        while (!placed)
        {
            int x = Random.Range(startX, startX + m_ZoneSize - (horizontal ? hangarCount * hangarSize.x : hangarSize.x));
            int y = Random.Range(startY, startY + m_ZoneSize - (horizontal ? hangarSize.y : hangarCount * hangarSize.y));

            if (IsHangarPositionValid(x, y, hangarCount, horizontal))
            {
                for (int i = 0; i < hangarCount; i++)
                {
                    int posX = horizontal ? x + i * (hangarSize.x + (int)(m_SpaceBetweenHangars / m_CellSize)) : x;
                    int posY = horizontal ? y : y + i * (hangarSize.y + (int)(m_SpaceBetweenHangars / m_CellSize));
                    PlaceHangarAtPosition(posX, posY, hangarSize);
                }
                placed = true;
            }
        }
    }

    void PlaceHangarAtPosition(int x, int y, Vector2Int hangarSize)
    {
        for (int j = 0; j < hangarSize.x; j++)
        {
            for (int k = 0; k < hangarSize.y; k++)
            {
                m_Grid[x + j, y + k] = 3;
            }
        }
    }

    bool IsHangarPositionValid(int startX, int startY, int count, bool horizontal)
    {
        Vector2Int hangarSize = GetPrefabSizeInGridUnits(m_HangarPrefab);
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < hangarSize.x; j++)
            {
                for (int k = 0; k < hangarSize.y; k++)
                {
                    int x = horizontal ? startX + i * (hangarSize.x + (int)(m_SpaceBetweenHangars / m_CellSize)) + j : startX + j;
                    int y = horizontal ? startY + k : startY + i * (hangarSize.y + (int)(m_SpaceBetweenHangars / m_CellSize)) + k;
                    if (x >= m_GridWidth || y >= m_GridHeight || m_Grid[x, y] != 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    void PlaceRunwayInZone(int startX, int startY, int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int x = Random.Range(startX, startX + m_ZoneSize);
                int y = Random.Range(startY, startY + m_ZoneSize - 4); // Ensure room for runway length

                if (IsRunwayPositionValid(x, y))
                {
                    for (int j = 0; j < 5; j++)
                    {
                        m_Grid[x, y + j] = 4;
                    }
                    placed = true;
                }
            }
        }
    }

    bool IsRunwayPositionValid(int x, int y)
    {
        for (int i = 0; i < 5; i++)
        {
            if (y + i >= m_GridHeight || m_Grid[x, y + i] != 0)
            {
                return false;
            }
        }
        return true;
    }
    void InstantiateAirport()
    {
        for (int x = 0; x < m_GridWidth; x++)
        {
            for (int y = 0; y < m_GridHeight; y++)
            {
                Vector3 position = new Vector3(x * m_CellSize, 0, y * m_CellSize);
                GameObject instantiatedPart = new();

                switch (m_Grid[x, y])
                {
                    case 1:
                        //Instantiate(m_ControlTowerPrefab, position, Quaternion.identity);
                        instantiatedPart = Instantiate(m_ControlTowerPrefab, transform, false);

                        break;
                    case 2:
                        //Instantiate(m_AirportBuildingPrefab, position, Quaternion.identity);
                        instantiatedPart = Instantiate(m_AirportBuildingPrefab, transform, false);
                        break;
                    case 3:
                        //Instantiate(m_HangarPrefab, position, Quaternion.identity);
                        instantiatedPart = Instantiate(m_HangarPrefab, transform, false);

                        Renderer renderer = instantiatedPart.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            Vector3 size = renderer.bounds.size;

                            instantiatedPart.transform.position = new Vector3(position.x + (size.x * 10), position.y, position.z );
                        }

                        continue;
                    case 4:
                        //Instantiate(m_RunwayPrefab, position, Quaternion.identity);
                        instantiatedPart = Instantiate(m_RunwayPrefab, transform, false);

                        break;
                }

                instantiatedPart.transform.position = position;

            }
        }
    }
    Vector2Int GetPrefabSizeInGridUnits(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = prefab.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            int width = Mathf.CeilToInt(size.x / m_CellSize);
            int height = Mathf.CeilToInt(size.z / m_CellSize);
            return new Vector2Int(width, height);
        }

        return Vector2Int.one; // Default size if no renderer found
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            m_GridWidth = Random.Range(10, 20);
            m_GridHeight = Random.Range(10, 20);

            m_Grid = new int[m_GridWidth, m_GridHeight];
            GenerateAirportGrid();
            InstantiateAirport();
        }
    }

}
