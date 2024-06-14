using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TowerControlGenerator : MonoBehaviour
{

    
    [SerializeField]
    [Range(1, 100)]
    int m_MinHeight = 1; 

    [SerializeField] 
    [Range(1, 100)]
    int m_MaxHeight = 5;


    [SerializeField] Vector3 m_InstantiationPos = Vector3.zero;

    [SerializeField] GameObject m_CurrentNode;

    [SerializeField] List<GameObject> m_TowerParts;

    [SerializeField] Escalator m_Escalator;

    void Awake()
    {
        m_TowerParts = Resources
                        .LoadAll<GameObject>("Prefab/ATC")
                        .Where(go => go.name.EndsWith("Tower"))
                        .ToList();
        m_CurrentNode = null;

        GenerateTowerControl();
    }

    void GenerateTowerControl ()
    {
        int fixedBodyHeight = Random.Range(m_MinHeight, m_MaxHeight);

        m_CurrentNode = null;

        GameObject baseTower = GenerateTowerPart("Base");
        GenerateTowerParts("Body", fixedBodyHeight);
        GenerateTowerPart("Neck");
        GameObject headTower = GenerateTowerPart("Head");


        Escalator escalator = GetComponentInChildren<Escalator>();

        if (escalator != null)
        {
            escalator.m_BottomPos = baseTower.transform.Find("BottomPosElevator").gameObject;
            escalator.m_TopPos = headTower.transform.Find("TopPosElevator").gameObject;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    GameObject GenerateTowerPart (string namePart, bool worldSpace = true)
    {
        GameObject towerPart = m_TowerParts.FirstOrDefault(p => p.name.Contains(namePart));

        return InstantiateTowerPart(towerPart, worldSpace);
    }

    /// <summary>
    /// Génère la partie dont le nom est passé en paramètre n fois où n correspond au nombre d'itérations 
    /// </summary>
    /// <param name="namePart">Nom de la partie à instancier</param>
    /// <param name="iterations">Nombre de fois qu'il faut instancier la partie selon l'axe Y (hauteur)</param>
    void GenerateTowerParts (string namePart, int iterations, bool worldSpace = false)
    {
        GameObject towerPart = m_TowerParts.FirstOrDefault(p => p.name.Contains(namePart));

        for (int i = 0; i < iterations; i++)
            InstantiateTowerPart(towerPart, worldSpace);
    }

    GameObject InstantiateTowerPart (GameObject partToInstantiate, bool worldSpace)
    {
        GameObject instantiatedTowerPart = Instantiate(partToInstantiate, transform, false);

        if (m_CurrentNode != null) 
            instantiatedTowerPart.transform.position = m_CurrentNode.transform.position;

        var node = instantiatedTowerPart.GetComponentsInChildren<Transform>().FirstOrDefault(n => n.name.Equals("NodePart"));

        if (node != null)
            m_CurrentNode = node.gameObject;

        return instantiatedTowerPart;
    }


}
