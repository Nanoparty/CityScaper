using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileContainer
{
    public List<GameObject> RoadTiles;
    public List<GameObject> WaterTiles;
    public List<GameObject> DirtTiles;
    public List<GameObject> GrassTiles;
    public List<GameObject> ResidentialTiles;
    public List<GameObject> CommercialTiles;

    public List<GameObject> RoadAndWaterTiles;

    public int RoadWeight;
    public int WaterWeight;
    public int DirtWeight;
    public int GrassWeight;
    public int ResidentialWeight;
    public int CommercialWeight;

    public List<(float, List<GameObject>)> WeightedTiles;

    public void CreateLists()
    {
        RoadTiles = new List<GameObject>();
        WaterTiles = new List<GameObject>();
        DirtTiles= new List<GameObject>();
        GrassTiles = new List<GameObject>();
        ResidentialTiles = new List<GameObject>();
        CommercialTiles = new List<GameObject>();

        RoadAndWaterTiles = new List<GameObject>();

        WeightedTiles = new List<(float, List<GameObject>)>();
    }

    public void Reset()
    {
        RoadTiles.Clear();
        WaterTiles.Clear();
        DirtTiles.Clear();
        GrassTiles.Clear();
        ResidentialTiles.Clear();
        CommercialTiles.Clear();

        RoadAndWaterTiles.Clear();
    }

    public void GenerateWeights()
    {
        WeightedTiles.Add((RoadWeight, RoadTiles));
        WeightedTiles.Add((WaterWeight, WaterTiles));
        WeightedTiles.Add((DirtWeight, DirtTiles));
        WeightedTiles.Add((ResidentialWeight, ResidentialTiles));
        WeightedTiles.Add((CommercialWeight, CommercialTiles));

        WeightedTiles.Add(((RoadWeight + WaterWeight)/2, RoadAndWaterTiles));
    }

    public GameObject RandomTile()
    {
        float totalWeight = 0f;

        List<((float, float), int)> weightCuttoffs = new List<((float, float), int)>();

        int index = 0;
        foreach((float, List<GameObject>) tileSet in WeightedTiles){
            float startingWeight = totalWeight;
            float additionalWeight = (tileSet.Item2.Count * tileSet.Item1);

            if (additionalWeight == 0) continue;

            totalWeight += additionalWeight;
            weightCuttoffs.Add(((startingWeight, totalWeight) ,index));
            index++;
        }

        float randomNumber = Random.Range(0f, totalWeight);

        for(int i = 0; i < weightCuttoffs.Count; i++)
        {
            if (randomNumber >= weightCuttoffs[i].Item1.Item1 && randomNumber <= weightCuttoffs[i].Item1.Item2)
            {
                // this list
                int selectedIndex = weightCuttoffs[i].Item2;
                List<GameObject> selectedTiles = WeightedTiles[selectedIndex].Item2;
                //if (selectedTiles.Count == 0) return null;
                return selectedTiles[Random.Range(0, selectedTiles.Count)];
            }
        }

        Debug.Log("NO VALID TILES!");
        return null;
    }

    public int Entropy()
    {
        int count = 0;
        count += RoadTiles.Count * RoadWeight;
        count += WaterTiles.Count * WaterWeight;
        count += DirtTiles.Count * DirtWeight;
        count += GrassTiles.Count * GrassWeight;
        count += ResidentialTiles.Count * ResidentialWeight;
        count += CommercialTiles.Count * CommercialWeight;
        count += RoadAndWaterTiles.Count * ((RoadWeight + WaterWeight) / 2);

        return count;
    }

    public void RemoveInvalidTiles(string dir, Tile.Type match)
    {
        

        for (int i = 0; i < WeightedTiles.Count; i++)
        {
            List<GameObject> list = WeightedTiles[i].Item2;
            List<GameObject> validTiles = new List<GameObject>();
            foreach (GameObject tile in list)
            {
                if (dir == "bottom")
                {
                    if (tile.GetComponent<Tile>().ZPositiveConnection == match){
                        validTiles.Add(tile);
                    }
                }
                if (dir == "top")
                {
                    if (tile.GetComponent<Tile>().ZNegativeConnection == match)
                    {
                        validTiles.Add(tile);
                    }
                }
                if (dir == "left")
                {
                    if (tile.GetComponent<Tile>().XPositiveConnection == match)
                    {
                        validTiles.Add(tile);
                    }
                }
                if (dir == "right")
                {
                    if (tile.GetComponent<Tile>().XNegativeConnection == match)
                    {
                        validTiles.Add(tile);
                    }
                }
            }
            WeightedTiles[i] = (WeightedTiles[i].Item1, validTiles);
        }
    }
}
