using UnityEngine;

[RequireComponent(typeof(TableProcces))]
public class DebugTable : MonoBehaviour
{
    private TableProcces _tableProcces;
    [SerializeField] private DebugDataStruct _debugData;

    public void InitializeDebuger()
    {
        _tableProcces.onUpdateDebugData += (table) => {
            DebugDataStruct debugData = new DebugDataStruct
            {
                levelNumber = table.LevelIterator + 1,
                trueAnswer = table.Cubes[table.CurrentTrueAnswerId].ChildImage.sprite,
                baseData = table.GameData.LevelDatas_m[table.LevelIterator].baseData,
            };
              UpdateData(debugData);
        };
    }

    private void UpdateData(DebugDataStruct data)
    {
        _debugData = data;
    }

    public DebugTable SetData(TableProcces tp)
    {
        _tableProcces = tp;
        return this;
    }
}
