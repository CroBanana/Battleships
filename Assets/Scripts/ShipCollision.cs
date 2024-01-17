using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    [Range(1,5)]
    public int shipSize;
    public List<Transform> collidingObjects;


    private void Start()
    {
        GetComponent<Collider>().enabled = true;
    }

    public bool EnoughSpace()
    {
        if (collidingObjects.Count == shipSize)
            return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        collidingObjects.Add(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        collidingObjects.Remove(other.transform);
    }

}
