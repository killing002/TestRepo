using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionProcces : MonoBehaviour
{
    private TableProcces _tableProcces;
    private CanvasProcces _canvasProcces;
    private ParticleProcces _particleProcces;

    private bool _debugComponent = false;
    private GameDataStruct _gameData;

    private void Awake()
    {
        _canvasProcces = FindObjectOfType<CanvasProcces>();
        _particleProcces = FindObjectOfType<ParticleProcces>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _tableProcces = CreateTable(_gameData);

        if (_debugComponent)
            CreateDebugComponent(_tableProcces, _tableProcces.gameObject);

        _tableProcces.onBuildLevel_e += (trueCube) => { _canvasProcces.SetAimSprite(trueCube.ChildImage.sprite); };
        _tableProcces.onWinCubeClick_e += (trueCube) => { _particleProcces.PlayParticle(WhichParticle.stars, trueCube.transform.position); };
        _tableProcces.onEndAllLevels_e += () => { 
            _canvasProcces.FadeOut(_canvasProcces.BackGroundImage);
            _canvasProcces.FadeOut(_canvasProcces.RestartButton.image);
            _canvasProcces.RestartButton.interactable = true;
        };
        _canvasProcces.FadeOut(_canvasProcces.AimImage);
        _canvasProcces.FadeOut(_canvasProcces.FindText);
        _tableProcces.StartLevel();    
    }

    private TableProcces CreateTable(GameDataStruct data)
    {
        var go = new GameObject("Table", typeof(TableProcces));
        var table = go.GetComponent<TableProcces>();
        go.transform.SetParent(this.transform);
        table.SetData(data);
        return table;
    }

    public void SetData(GameDataStruct data, bool debugComponent)
    {
        _gameData = data;
        _debugComponent = debugComponent;
    }

    private void CreateDebugComponent(TableProcces tp, GameObject go)
    {
        go.AddComponent<DebugTable>();
        go.GetComponent<DebugTable>().SetData(tp).InitializeDebuger();
    }


}
