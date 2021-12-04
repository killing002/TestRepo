using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



public class TableProcces : MonoBehaviour
{
    private enum LevelStage
    {
        start,
        normal,
        end,
    }

    private GameDataStruct _gameData;
    private CanvasProcces _canvasProcces;
    private PoolManager _poolManager;

    private List<int> _usedSprites_l = new List<int>();
    private List<Quad> _cubes_l = new List<Quad>();
    private Dictionary<int, List<int>> _usedTrueAnswerd_d = new Dictionary<int, List<int>>();
    private int _levelIterator = 0;
    private int _currentKitId = 0;
    private int _currentTrueAnswerId = 0;

    public Action onStartEachLevel_e = delegate { };
    public Action onEndEachLevel_e = delegate { };
    public Action onEndAllLevels_e = delegate { };
    public Action<Quad> onBuildLevel_e;
    public Action<Quad> onWinCubeClick_e = delegate { };

    public Action<TableProcces> onUpdateDebugData = delegate { };

    public int LevelIterator => _levelIterator;
    public int CurrentKitId  => _currentKitId;
    public int CurrentTrueAnswerId => _currentTrueAnswerId;
    public List<Quad> Cubes => _cubes_l;
    public GameDataStruct GameData => _gameData;



    private void Awake()
    {
        _canvasProcces = FindObjectOfType<CanvasProcces>();
        _poolManager = FindObjectOfType<PoolManager>();

        if (_canvasProcces == null)
            Debug.LogError("Canvas Data is empty! Set CanvasProcces!");
        if (_poolManager == null)
            Debug.LogError("Pool is empty! Set PoolManager!");

    }

    public void StartLevel()
    {
        Debug.Log("Start Level");

        //end levels
        if(_levelIterator == _gameData.LevelDatas_m.Length)
        {
            onEndAllLevels_e?.Invoke();
            return;
        }

        if (CalculateChance(_gameData.ChanceOfDataChange) ||
             _gameData.ObjectsKits_m[_currentKitId]._objectsKit.Length < _gameData.LevelDatas_m[_levelIterator].baseData.CountObjects())
        {
            _currentKitId = GetRightObjectsKit(_currentKitId);
        }


        BuildLevel(_gameData.LevelDatas_m[_levelIterator], _gameData.ObjectsKits_m[_currentKitId]._objectsKit, (LevelStage)_levelIterator);

        onStartEachLevel_e?.Invoke();
        onUpdateDebugData?.Invoke(this);

        _levelIterator++;
    }

    private void EndLevel()
    {
        foreach (var cube in _cubes_l)
        {
            cube.SetTrueQuad(false);
            cube.ClearEvents();
            cube.gameObject.SetActive(false);
        }

        _cubes_l.Clear();
        _usedSprites_l.Clear();

        onEndEachLevel_e?.Invoke();
    }

    /// <summary>
    /// Выбирает киты в зависимости того, какой в данный момент выбран кит и проверяет на соответствие количества объектов в ките и в требованиях на уровне
    /// </summary>
    /// <param name="currentIdKit"></param>
    /// <returns></returns>
    private int GetRightObjectsKit(int currentIdKit)
    {
        int randIdKit = Random.Range(0, _gameData.ObjectsKits_m.Length);
        //не выбирать текущий кит или
        //если количество объектов в ките меньше чем количество объектов в требованиях левла то ливнуть
        if (_gameData.ObjectsKits_m[randIdKit]._objectsKit.Length < _gameData.LevelDatas_m[_levelIterator].baseData.CountObjects())
        {
           return GetRightObjectsKit(currentIdKit);
        }
        return randIdKit;
    }

    private void BuildLevel(LevelData levelData, Sprite[] spriteData, LevelStage levelStage = LevelStage.normal)
    {
        int countQuads = levelData.baseData.strokesCount * levelData.baseData.columnsCount;

        // spawn cubes objects and save
        var cubes = SpawnCubes(levelData.baseData.strokesCount, levelData.baseData.columnsCount);

        ChacheCubes(cubes);

        //start bounce animation
        if (levelStage == LevelStage.start)
        {
            foreach (Quad cube in _cubes_l)
            {
                cube.BounceScaleAnimation();
            }
        }
        //positioning cubes in world
        PosCubesLikeGrid(cubes, levelData.baseData.strokesCount, levelData.baseData.columnsCount);


        //fill value cubes
        for (int i = 0; i < _cubes_l.Count; i++)
        {
            SetUniqueValueToCube(_cubes_l[i].GetComponent<Quad>(), spriteData);
        }

        _usedTrueAnswerd_d.TryGetValue(_currentKitId, out List<int> vault);
        if(vault == null)
        {
            vault = new List<int>();
            _usedTrueAnswerd_d.Add(_currentKitId, vault);
        }
        //random - unique id true answer
        RandomInsidePool(_usedSprites_l,vault,out int indexOfTrueAnswer , true);
        _currentTrueAnswerId = indexOfTrueAnswer;
        
        Quad trueCube = _cubes_l[indexOfTrueAnswer].GetComponent<Quad>();

        //set true cube bool
        trueCube.SetTrueQuad(true);
        //subscribe on event
        trueCube.onEndBounceScaleAnimation_e.AddListener(()=> {
            trueCube.SetTouchableQuad(true);
            EndLevel();
            StartLevel();
        });

        trueCube.onMouseDown_e.AddListener(() =>
        {
            onWinCubeClick_e?.Invoke(trueCube);
        });

        onBuildLevel_e?.Invoke(trueCube);
    }

    private void SetUniqueValueToCube(Quad cube, Sprite[] data)
    {
        //color
        Color color = GetRandomColor();
        //get unique-random sprite id
        int idSprite = GetUniqueRandomId(data.Length, _usedSprites_l);
        //set sprite and color
        cube.SetData(color, data[idSprite]);
    }

    private Color GetRandomColor() => Random.ColorHSV(0, 1, 0.1f, 0.5f, 1f, 1f, 1, 1);

    private void PosCubesLikeGrid( List<GameObject> quads, int strokes, int columns)
    {
        int identifier = 0;

        for (int i = 0; i < strokes; i++)
            for (int j = 0; j < columns; j++)
            {
                Vector3 firstPos = Vector3.zero;
                float offsetX = _gameData.Offset;
                float offsetY = _gameData.Offset;

                quads[identifier].transform.position = new Vector3(firstPos.x + j * offsetX, firstPos.y - offsetY * i, 0);

                identifier++;
            }
    }

    private List<GameObject> SpawnCubes(int strokes, int columns)
    {
        var list = new List<GameObject>();
        for (int i = 0; i < strokes * columns; i++)
        {
            list.Add(_poolManager.GetPrefab(WhichPrefab.quad));
        }

        return list;
    }

    public void SetData(GameDataStruct data)
    {
        _gameData = data;
    }

    private int GetUniqueRandomId(int maxEclusiveValue, List<int> vault = null, bool saveInVault = true)
    {
        int randId = Random.Range(0, maxEclusiveValue);
      

        if (vault == null)
            return randId;

        if ((vault.Count >= maxEclusiveValue ||
           maxEclusiveValue <= 0))
        {
            Debug.LogError("Free id is end. Restart game.");
            return -1;
        }

        if (vault.Contains(randId))
            return GetUniqueRandomId(maxEclusiveValue, vault);
        if(saveInVault)
            vault.Add(randId);
        return randId;
    }

    private int RandomInsidePool(List<int> pool, List<int> vault, out int indexPool, bool unique = false)
    {
        int r = Random.Range(0, pool.Max() + 1);
        if (unique &&
            (
            !pool.Contains(r) ||
            vault.Contains(r))
            )
        {
            return RandomInsidePool(pool,vault,out indexPool, unique);

        }

        indexPool = pool.IndexOf(r);
        vault.Add(r);
        return r;
    }
    /// <summary>
    /// Простая формула по высчитыванию вероятности
    /// </summary>
    /// <param name="chance">Шанс прока случая(%)</param>
    /// <returns>True - если вероятнсть свершилась. False - если нет</returns>
    private bool CalculateChance(int chance)
    {
        if (chance == 0)
            return false;

        int count = 0;
        //new function
        for (int i = 1, summ = 0; i < 100; i++)
        {
            summ += chance;
            if (summ >= 100)
            {
                count = i;
                break;
            }
        }

        int r = Random.Range(0, count);

        //проверяем на 0 так как при любой вероятности 0 будет равновероятен всем числам и будет присутствовать во всех выборках
        if (r == 0)
            return true;
        return false;
    }

    private void ChacheCubes(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            _cubes_l.Add(objects[i].GetComponent<Quad>());
        }
    }

    private void OnDestroy()
    {
        onBuildLevel_e = null;
        onEndAllLevels_e = null;
        onEndEachLevel_e = null;
        onStartEachLevel_e = null;
        onWinCubeClick_e = null;

        _usedSprites_l.Clear();
        _usedTrueAnswerd_d.Clear();
        _cubes_l.Clear();
    }
}
