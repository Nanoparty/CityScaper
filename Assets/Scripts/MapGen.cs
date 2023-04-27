using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    [SerializeField] int Rows;
    [SerializeField] int Cols;
    [SerializeField] int BlockSize;

    [SerializeField] List<GameObject> InputTiles;
    [SerializeField] List<GameObject> SecondPassTiles;
    [SerializeField] GameObject NullTile;

    [SerializeField] List<GameObject> RoadTiles;
    [SerializeField] List<GameObject> WaterTiles;
    [SerializeField] List<GameObject> DirtTiles;
    [SerializeField] List<GameObject> ResidentialTiles;
    [SerializeField] List<GameObject> CommercialTiles;
    [SerializeField] List<GameObject> GrassTiles;

    [SerializeField] List<GameObject> RoadAndWaterTiles;

    [SerializeField] bool Roads;
    [SerializeField] bool Water;
    [SerializeField] bool Dirt;
    [SerializeField] bool Residential;
    [SerializeField] bool Commercial;
    [SerializeField] bool Grass;

    [SerializeField] int RoadWeight = 1;
    [SerializeField] int ResidentialWeight = 1;
    [SerializeField] int WaterWeight = 1;
    [SerializeField] int CommericalWeight = 1;
    [SerializeField] int DirtWeight = 1;
    [SerializeField] int GrassWeight = 1;

    [SerializeField] bool UseRandomSeed = true;
    [SerializeField] int Seed;

    // UI Fields

    public TMP_InputField RowField;
    public TMP_InputField ColField;

    public Slider RoadSlider;
    public TMP_Text RoadWeightText;

    public Slider DirtSlider;
    public TMP_Text DirtWeightText;

    public Slider WaterSlider;
    public TMP_Text WaterWeightText;

    public Slider ResidentialSlider;
    public TMP_Text ResidentialWeightText;

    public Slider CommercialSlider;
    public TMP_Text CommercialWeightText;

    public Slider GrassSlider;
    public TMP_Text GrassWeightText;

    private List<GameObject> _allTiles;

    [SerializeField] public List<GameObject>[,] _mapList;
    public GameObject[,] _map;
    private List<GameObject> _objects;
    private int _totalResolved;

    private void Awake()
    {
        if (UseRandomSeed)
        {
            Seed = (int)System.DateTime.Now.Ticks;
            Random.InitState(Seed);
        }

        _allTiles = new List<GameObject>();
        _objects = new List<GameObject>();

        InitializeArrays();

        RowField.onValueChanged.AddListener(delegate { SetRows(); });
        ColField.onValueChanged.AddListener(delegate { SetCols(); });

        RowField.text = Rows.ToString();
        ColField.text = Cols.ToString();

        RoadSlider.onValueChanged.AddListener(delegate { SetWeight(RoadSlider, RoadWeightText, ref RoadWeight); }); 
        WaterSlider.onValueChanged.AddListener(delegate { SetWeight(WaterSlider, WaterWeightText, ref WaterWeight); });
        DirtSlider.onValueChanged.AddListener(delegate { SetWeight(DirtSlider, DirtWeightText, ref DirtWeight); });
        ResidentialSlider.onValueChanged.AddListener(delegate { SetWeight(ResidentialSlider, ResidentialWeightText, ref ResidentialWeight); });
        CommercialSlider.onValueChanged.AddListener(delegate { SetWeight(CommercialSlider, CommercialWeightText, ref CommericalWeight); });
        GrassSlider.onValueChanged.AddListener(delegate { SetWeight(GrassSlider, GrassWeightText, ref GrassWeight); });
    }

    private void InitializeArrays()
    {
        _mapList = new List<GameObject>[Rows, Cols];
        _map = new GameObject[Rows, Cols];

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                _mapList[r, c] = new List<GameObject>();
            }
        }
    }

    private void Start()
    {
        

        GenerateMap();
        DrawMap();
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

        for (int i = 0; i < _mapList.GetLength(0); i++)
        {
            for (int j = 0; j < _mapList.GetLength(1); j++)
            {
                _mapList[i, j].Clear();
                _map[i, j] = null;
            }
        }

        _totalResolved= 0;

        if (_map.GetLength(0) != Rows || _map.GetLength(1) != Cols)
        {
            InitializeArrays();
        }
    }

    private void GenerateMap()
    {
        Reset();

        //Populate States

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (Grass)
                {
                    for (int i = 0; i < GrassWeight; i++)
                    {
                        _mapList[r, c].AddRange(GrassTiles);
                    }
                }
                if (Roads)
                {
                    for (int i = 0; i < RoadWeight; i++)
                    {
                        _mapList[r, c].AddRange(RoadTiles);
                    }
                }
                if (Water)
                {
                    for (int i = 0; i < WaterWeight; i++)
                    {
                        _mapList[r, c].AddRange(WaterTiles);
                    }
                }
                if (Dirt)
                {
                    for (int i = 0; i < DirtWeight; i++)
                    {
                        _mapList[r, c].AddRange(DirtTiles);
                    }
                }
                if (Residential)
                {
                    for(int i = 0; i < ResidentialWeight; i++)
                    {
                        _mapList[r, c].AddRange(ResidentialTiles);
                    }
                }
                if (Commercial)
                {
                    for (int i = 0; i < CommericalWeight; i++)
                    {
                        _mapList[r, c].AddRange(CommercialTiles);
                    }
                }

                if (Water && Roads)
                {
                    for (int i = 0; i < (RoadWeight + WaterWeight) / 2; i++)
                    {
                        _mapList[r, c].AddRange(RoadAndWaterTiles);
                    }
                }
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

        //Spot fill
        for(int r = 0; r < Rows; r++)
        {
            for(int c = 0; c < Cols; c++)
            {
                if (_map[r,c] == null)
                {
                    List<GameObject> availableTiles = new List<GameObject>();
                    List<GameObject> validTiles = new List<GameObject>();

                    availableTiles.AddRange(SecondPassTiles);

                    foreach (GameObject tile in availableTiles)
                    {
                        Tile.Type topType = Tile.Type.Void;
                        Tile.Type bottomType = Tile.Type.Void;
                        Tile.Type leftType = Tile.Type.Void;
                        Tile.Type rightType = Tile.Type.Void;

                        //Check Below
                        if (r > 0 && _map[r - 1, c] != null)
                        {
                            bottomType = _map[r - 1,c].GetComponent<Tile>().ZPositiveConnection;
                        }

                        //Check Above
                        if (r < Rows - 1 && _map[r + 1, c] != null)
                        {
                            topType = _map[r + 1, c].GetComponent<Tile>().ZNegativeConnection;
                        }

                        //Check Left
                        if (c > 0 && _map[r, c - 1] != null)
                        {
                            leftType = _map[r, c - 1].GetComponent<Tile>().XPositiveConnection;
                        }

                        //Check Right
                        if (c < Cols - 1 && _map[r, c + 1] != null)
                        {
                            rightType = _map[r, c + 1].GetComponent<Tile>().XNegativeConnection;
                        }

                        if ((bottomType == Tile.Type.Void || bottomType == tile.GetComponent<Tile>().ZNegativeConnection)
                            && (topType == Tile.Type.Void || topType == tile.GetComponent<Tile>().ZPositiveConnection)
                            && (leftType == Tile.Type.Void || leftType == tile.GetComponent<Tile>().XNegativeConnection)
                            && (rightType == Tile.Type.Void || rightType == tile.GetComponent<Tile>().XPositiveConnection))
                        {
                            validTiles.Add(tile);
                        }
                        
                    }

                    if (validTiles.Count > 0)
                    {
                        GameObject selectedTile = validTiles[Random.Range(0, validTiles.Count)];
                        _map[r, c] = selectedTile;
                    }
                    else
                    {
                        _map[r, c] = NullTile;
                    }

                    
                }
            }
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

        if (totalStates == int.MaxValue) { return; }

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


    // UI Functions

    public void SetRows()
    {
        if (int.Parse(RowField.text) > 200) RowField.text = "200";
        Rows = int.Parse(RowField.text);
        Debug.Log("Rows set to " + Rows);
    }
    public void SetCols()
    {
        if (int.Parse(ColField.text) > 200) ColField.text = "200";
        Cols = int.Parse(ColField.text);
        Debug.Log("Cols set to " + Cols);
    }

    public void SetWeight(Slider inputSlider, TMP_Text weightText, ref int weight)
    {
        weightText.text = inputSlider.value.ToString();
        weight = (int)inputSlider.value;
    }
}
