using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpProcces : MonoBehaviour
{
    [SerializeField] private bool _addDebugComponent = false;
    [SerializeField] private GameDataStruct _gameData;


    private void Awake()
    {
        //create table
       var session = CreateSession();
        session.SetData(_gameData, _addDebugComponent);

    }

    private SessionProcces CreateSession()
    {
        var go = new GameObject("Sessions", typeof(SessionProcces));
        var tp = go.GetComponent<SessionProcces>();
        go.transform.SetParent(this.transform);

        return tp;
    }
}
