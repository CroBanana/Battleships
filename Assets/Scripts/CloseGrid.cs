using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGrid : MonoBehaviour
{
    public List<GameObject> closeGridObjects;
    private void OnTriggerEnter(Collider other)
    {
        closeGridObjects.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        closeGridObjects.Remove(other.gameObject);
    }
}
