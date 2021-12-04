using System;
using UnityEngine;

[Serializable]
public struct GameDataStruct
{
    [SerializeField] private LevelData[] _levelDatas_m;
    [SerializeField] private ObjectKitData[] _objectsKits_m;
    [Tooltip("Вероятность смены набора данных(%)")]
    [Range(0, 100)]
    [SerializeField] private int _chanceOfDataChange;

    [Tooltip("Расстояние на которое отступают друг от друга кубики")]
    [SerializeField] private float _offset;

    public LevelData[] LevelDatas_m => _levelDatas_m;
    public ObjectKitData[] ObjectsKits_m => _objectsKits_m;
    public int ChanceOfDataChange => _chanceOfDataChange;
    public float Offset => _offset;

}
