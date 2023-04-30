using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> cars;
    [SerializeField] float SpawnChance = 10;

    [SerializeField] Transform Position1;
    [SerializeField] Transform Position2;

    void Start()
    {
        int RandomNumber = Random.Range(0, 100);

        if (RandomNumber <= SpawnChance)
        {
            GameObject car1 = Instantiate(cars[Random.Range(0, cars.Count)], Position1.position, Position1.rotation);
            car1.transform.localScale = Position1.localScale;
            GameObject Decorations = GameObject.FindGameObjectWithTag("Decorations");
            car1.transform.SetParent(Decorations.transform, true);
        }
    }

}
