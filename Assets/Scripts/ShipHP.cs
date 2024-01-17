using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHP : MonoBehaviour
{
    int hp;
    public bool isReplay;
    MeshRenderer boatRenderer;
    List<GridPart> gridsAround = new() ;
    ShipCollision shipCollision;
    void Start()
    {
        shipCollision = GetComponent<ShipCollision>();
        hp = shipCollision.shipSize;
        boatRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void TakeDmg()
    {
        hp--;
        if (hp <= 0)
        {
            //edit sve gridove oko broda da se ne mogu koristit
            foreach(GameObject closeGrid in GetComponentInChildren<CloseGrid>().closeGridObjects)
            {
                GridPart grid = closeGrid.GetComponent<GridPart>();
                grid.AttackedGrid();
                if(gridsAround == null || !gridsAround.Contains(grid))
                    gridsAround.Add(grid);
            }

            if (isReplay)
            {
                boatRenderer.enabled = false;
                return;
            }
            Playing.Instance.WinCheck();
            //destroy Ship
            Destroy(gameObject);
        }
    }
    
    //samo za replay
    public void GainHP()
    {
        hp++;
        
        foreach(GridPart grid in gridsAround)
        {
            if (shipCollision.collidingObjects.Contains(grid.transform))
                continue;
            grid.ResetGrid();
        }
        boatRenderer.enabled = true;
    }
}
