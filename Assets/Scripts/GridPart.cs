using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPart : MonoBehaviour
{
    [SerializeField]
    Material normal, hoverOver, attackHit, attackMissed;
    MeshRenderer meshRenderer;
    BoxCollider collider;
    public ShipHP shipOnThis;
    public bool hasBeenAttacked = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = normal;
        collider = GetComponent<BoxCollider>();
    }

    public void MouseOnGrid()
    {
        meshRenderer.material = hoverOver;
    }

    public void BackToNormal()
    {
        if (hasBeenAttacked)
            return;
        meshRenderer.material = normal;
    }

    public bool AttackedGrid()
    {
        if (hasBeenAttacked)
            return true;

        hasBeenAttacked = true;
        DisableGrid();
        if (shipOnThis != null)
        {
            meshRenderer.material = attackHit;
            shipOnThis.TakeDmg();
            return true;
        }
        meshRenderer.material = attackMissed;
        return false;
    }

    public void DisableGrid()
    {
        if (hasBeenAttacked)
        {
            collider.enabled = false;
        }
    }

    public void ResetGrid()
    {
        Debug.Log("Grid Reset");
        
        meshRenderer.material = normal;
        collider.enabled = true;
        hasBeenAttacked = false;

        if(shipOnThis != null)
        {
            shipOnThis.GainHP();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
            shipOnThis = other.gameObject.GetComponent<ShipHP>();
    }

}
