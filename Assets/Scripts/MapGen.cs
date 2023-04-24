using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{
    [SerializeField] int Rows;
    [SerializeField] int Cols;
    [SerializeField] int BlockSize;

    [SerializeField] List<GameObject> InputTiles;

    [SerializeField] bool UseRandomSeed = true;
    [SerializeField] int Seed;

    private List<GameObject> _allTiles;

    [SerializeField] public List<GameObject>[,] _mapList;
    public GameObject[,] _map;
    private List<GameObject> _objects;
    private int _totalResolved;

    private void Start()
    {
        if (UseRandomSeed)
        {
            Seed = (int)System.DateTime.Now.Ticks;
            Random.InitState(Seed);
        }

        _allTiles = new List<GameObject>();

        _mapList = new List<GameObject>[Rows, Cols];
        _map = new GameObject[Rows,Cols];
        _objects = new List<GameObject>();

        for(int r = 0; r < Rows; r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                _mapList[r, c] = new List<GameObject>();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            //Generate New Map
            GenerateMap();

            //_map[0, 0] = InputTiles[0];
            //_map[0, 1] = InputTiles[1];

            //_map[2, 1] = InputTiles[0];
            //_map[2, 2] = InputTiles[1];


            DrawMap();
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            ResolveNextTile();
            DrawMap();
        }
    }

    private void Reset()
    {
        foreach (GameObject o in _objects)
        {
            Destroy(o);
        }
        _objects.Clear();

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                _mapList[i, j].Clear();
                _map[i, j] = null;
            }
        }

        _totalResolved= 0;
    }

    private void GenerateMap()
    {
        Reset();

        //Populate States

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                _mapList[r, c].AddRange(InputTiles);
            }
        }

        //Choose Starting Point
        int startingRow = Random.Range(0, Rows);
        int startingCol = Random.Range(0, Cols);

        GameObject currentTile = _mapList[startingRow, startingCol][Random.Range(0, _mapList[startingRow, startingCol].Count)];
        _map[startingRow, startingCol] = currentTile;
        _mapList[startingRow, startingCol].Clear();

        PropagateWave(startingRow, startingCol, currentTile);

        for (int i = 0; i < Rows * Cols - 1; i++)
        {
            ResolveNextTile();
        }

    }

    private void ResolveNextTile()
    {
        int nextRow = 0;
        int nextCol = 0;
        int totalStates = int.MaxValue;

        for(int r = 0; r < Rows; r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                int states = _mapList[r, c].Count;

                if (states <= 0) continue;

                if (states < totalStates)
                {
                    totalStates = states;
                    nextRow = r;
                    nextCol = c;
                }
            }
        }

        GameObject currentTile = _mapList[nextRow, nextCol][Random.Range(0, _mapList[nextRow, nextCol].Count)];
        _map[nextRow, nextCol] = currentTile;
        _mapList[nextRow, nextCol].Clear();

        PropagateWave(nextRow, nextCol, currentTile);
    }

    private void PropagateWave(int r, int c, GameObject currentTile)
    {
        // Check Below
        if (r > 0)
        {
            List<GameObject> validTiles = new List<GameObject>();
            List<GameObject> availableTiles = _mapList[r - 1, c];
            foreach (GameObject tile in availableTiles)
            {
                if (tile.GetComponent<Tile>().ZPositiveConnection == currentTile.GetComponent<Tile>().ZNegativeConnection)
                {
                    validTiles.Add(tile);
                }
            }
            _mapList[r - 1, c] = validTiles;
        }

        // Check Above
        if (r < Rows - 1)
        {
            List<GameObject> validTiles = new List<GameObject>();
            List<GameObject> availableTiles = _mapList[r + 1, c];
            foreach (GameObject tile in availableTiles)
            {
                if (tile.GetComponent<Tile>().ZNegativeConnection == currentTile.GetComponent<Tile>().ZPositiveConnection)
                {
                    validTiles.Add(tile);
                }
            }
            _mapList[r + 1, c] = validTiles;
        }

        // Check Left
        if (c > 0)
        {
            List<GameObject> validTiles = new List<GameObject>();
            List<GameObject> availableTiles = _mapList[r, c - 1];
            foreach (GameObject tile in availableTiles)
            {
                if (tile.GetComponent<Tile>().XPositiveConnection == currentTile.GetComponent<Tile>().XNegativeConnection)
                {
                    Debug.Log("Tile " + tile.GetComponent<Tile>().XPositiveConnection + " == " + currentTile.GetComponent<Tile>().XNegativeConnection);
                    validTiles.Add(tile);
                }
            }
            _mapList[r, c - 1] = validTiles;
        }

        // Check Right
        if (c < Cols - 1)
        {
            List<GameObject> validTiles = new List<GameObject>();
            List<GameObject> availableTiles = _mapList[r, c + 1];
            foreach (GameObject tile in availableTiles)
            {
                if (tile.GetComponent<Tile>().XNegativeConnection == currentTile.GetComponent<Tile>().XPositiveConnection)
                {
                    validTiles.Add(tile);
                }
            }
            _mapList[r, c + 1] = validTiles;
        }
    }
    

    private void DrawMap()
    {
        foreach(GameObject o in _objects)
        {
            Destroy(o);
        }
        _objects.Clear();

        int xpos = 0;
        int zpos = 0;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (_map[r, c] == null) continue;
                xpos += BlockSize;
                Debug.Log("Instantiate Tile");
                GameObject o =Instantiate(_map[r,c], new Vector3(c * BlockSize, 0, r * BlockSize), Quaternion.identity);
                _objects.Add(o);
            }
        }
    }
}
