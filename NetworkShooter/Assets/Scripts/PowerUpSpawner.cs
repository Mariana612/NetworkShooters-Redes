using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;   // Largo, Incrementar...
    public Transform[] spawnPoints;
    public float interval = 15f;

    // Objeto activo de cada tipo (por índice de prefab)
    private GameObject[] activeByType;
    // Objeto activo en cada punto (por índice de punto)
    private GameObject[] activeByPoint;

    IEnumerator Start()
    {
        activeByType = new GameObject[powerUpPrefabs.Length];
        activeByPoint = new GameObject[spawnPoints.Length];

        while (true)
        {
            yield return new WaitForSeconds(interval);
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        // Tipos disponibles (sin objeto activo)
        List<int> freeTypes = new List<int>();
        for (int i = 0; i < powerUpPrefabs.Length; i++)
            if (activeByType[i] == null)
                freeTypes.Add(i);

        // Puntos disponibles (sin objeto activo)
        List<int> freePoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            if (activeByPoint[i] == null)
                freePoints.Add(i);

        // Si no hay tipo o punto libre, no aparece nada esta vez
        if (freeTypes.Count == 0 || freePoints.Count == 0)
            return;

        int typeIndex = freeTypes[Random.Range(0, freeTypes.Count)];
        int pointIndex = freePoints[Random.Range(0, freePoints.Count)];

        GameObject obj = Instantiate(powerUpPrefabs[typeIndex],
                                     spawnPoints[pointIndex].position,
                                     Quaternion.identity);

        activeByType[typeIndex] = obj;
        activeByPoint[pointIndex] = obj;
    }
}