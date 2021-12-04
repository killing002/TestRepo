using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PoolObject
{
    public GameObject prefab;
    public int count;
}

public enum WhichPrefab
{
    quad,
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolObject[] _objects_m;
    private List<List<GameObject>> _vault = new List<List<GameObject>>();

    private void Awake()
    {
        if (_objects_m.Length == 0)
            Debug.LogError("Pool is empty!");

        FillPool();
    }

    private void FillPool()
    {
        var l = new List<GameObject>();
        foreach (var obj in _objects_m)
        {
            for (int i = 0; i < obj.count; i++)
            {
                var go = Instantiate(obj.prefab, transform);
                go.SetActive(false);
                l.Add(go);
            }
            _vault.Add(l);
        }
    }

    public GameObject GetPrefab(WhichPrefab prefab)
    {
        int id = (int)prefab;
        for (int i = 0; i < _vault[id].Count; i++)
        {
            if (!_vault[id][i].activeSelf)
            {
                _vault[id][i].SetActive(true);
                return _vault[id][i];
            }
        }

        Debug.LogError("No more free prefabs");
        return null;
    }
}
