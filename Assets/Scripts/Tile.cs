using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum Type
    {
        Grass,
        Road,
        Water,
        Void,
        Dirt
    }

    [SerializeField] public GameObject Model;

    [SerializeField] public Vector3 LocalRotation;

    [SerializeField] public Type XPositiveConnection;
    [SerializeField] public Type XNegativeConnection;
    [SerializeField] public Type ZPositiveConnection;
    [SerializeField] public Type ZNegativeConnection;
}
    
