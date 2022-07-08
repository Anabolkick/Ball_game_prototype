using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MapGenerator : MonoBehaviour
{
    [HideInInspector]
    public static List<GameObject> WaypointsList;

    public List<GameObject> BarrierPrefabs;
    public GameObject WaypointPrefab;
    public int WaypointsCount;
    public int BarrierCount;
    public GameObject Map;

    private int _emptyLength = 15; //дистанция местности в начале, где не будет объектов
    void Awake()
    {
        var length = Map.transform.localScale.z;
        var width    = Map.transform.localScale.x;
        WaypointsList = new List<GameObject>();

        for (int i = 0; i < BarrierCount; i++)
        {
            var prefab = BarrierPrefabs[Random.Range(0, BarrierPrefabs.Count)];
            prefab.transform.localScale = Vector3.one * Random.Range(0.7f, 1);
            Instantiate(prefab, new Vector3(Random.Range(0, width)-width/2, 0.01f, Random.Range(0, length- _emptyLength) - length/2 + _emptyLength), Quaternion.identity);
        }

        for (int i = 1; i <= WaypointsCount; i++)
        {
            WaypointsList.Add( Instantiate(WaypointPrefab, new Vector3(0, 2f, ((length - _emptyLength) / WaypointsCount * i)-length/2+_emptyLength), Quaternion.identity));
        }
    }

}
