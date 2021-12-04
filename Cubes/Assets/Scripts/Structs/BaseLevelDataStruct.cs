using System;
using UnityEngine;

[Serializable]
public struct BaseLevelDataStruct
{
    [Min(1)]
    public int strokesCount;
    [Min(1)]
    public int columnsCount;

    public int CountObjects()
    {
        return strokesCount * columnsCount;
    }
}
