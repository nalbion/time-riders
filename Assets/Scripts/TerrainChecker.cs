using UnityEngine;

public class TerrainChecker : MonoBehaviour
{
    public TerrainType terrainType;
    
    [Header("Visual Settings")]
    public Material terrainMaterial;
    public Color terrainColor = Color.white;
    
    void Start()
    {
        // Apply visual styling based on terrain type
        Renderer renderer = GetComponent<Renderer>();
        if (renderer && terrainMaterial)
        {
            renderer.material = terrainMaterial;
            renderer.material.color = terrainColor;
        }
    }
}