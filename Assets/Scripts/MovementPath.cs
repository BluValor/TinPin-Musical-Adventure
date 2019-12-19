using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    public Transform[] Locations;
    public int CircleIndex;

    public Transform AttakSpawnerTransform => this.Locations[0];

    public Transform CircleTransform => this.Locations[CircleIndex];

    public Transform PlayerTransform => this.Locations[this.Locations.Length - 1];
}
