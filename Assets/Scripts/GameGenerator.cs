using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public bool gameSetupComplete;
    public bool isReplay;
    [SerializeField]
    GameObject gridPrefab;
    [SerializeField]
    int gridSize = 10;
    [SerializeField]
    List<GameObject> shipsToSpawn;

    [SerializeField]
    List<ShipCollision> spawnedShips;


    public List<Transform> player1Grid;
    public List<Transform> player2Grid;

    public List<Transform> disabledPlayer1Grid;
    public List<Transform> disabledPlayer2Grid;

    public List<Transform> player1Ships;
    public List<Transform> player2Ships;

    private void Start()
    {
        //kreira grid za obadva igrača
        GridCreation();

        if(!isReplay)
            StartCoroutine(CreateShips(player1Grid));
        //StartCoroutine(CreateShips(player2Grid));
    }
    void GridCreation()
    {
        for (int x = 1; x < gridSize + 1; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                //grid za prvog igrača
                player1Grid.Add( CreateGridPart(x, z));

                //grid za drugog igrača
                player2Grid.Add(CreateGridPart(-x, z));
            }
        }
    }
    
    Transform CreateGridPart(int x, int z)
    {
        Vector3 createPosition = new(x, 0, z);
        return Instantiate(gridPrefab, 
                            createPosition, 
                            Quaternion.identity, 
                            null).transform;
    }

    IEnumerator CreateShips(List<Transform> playerGrid)
    {
        //ovo je samo za jednom igrača, treba još za jednom, identično

        for (int currentPrefabID = 0; currentPrefabID < shipsToSpawn.Count;)
        {
            //random polje na gridu
            Transform randomGridPoint = playerGrid[Random.Range(0, playerGrid.Count)];
            //kreira 4 broda na toj poziciji s različitim rotacijama
            CreateNewShip(randomGridPoint.position,shipsToSpawn[currentPrefabID].gameObject);
            yield return new WaitForEndOfFrame();
            //treba proc kroz sve brodove novokreirane brodove i maknut/obrisat one koji nemaju dovoljno polja
            //ako brodova nema onda treba ponovti prijašnji step, nema brodova znaci da na toj lokaciji nema praznog mijesta za taj brod

            List<ShipCollision> worthyShips = new();
            foreach (ShipCollision ship in spawnedShips)
            {
                if (ship.EnoughSpace())
                    worthyShips.Add(ship);
            }

            if (worthyShips.Count == 0)
            {
                RemoveSpawnedShips();
                continue;
            }

            //ako ima brodova odabire jedan od njih random
            int randomShip = Random.Range(0, worthyShips.Count);
            ShipCollision selectedShip = worthyShips[randomShip];

            //sve ostale brise
            spawnedShips.Remove(selectedShip);
            RemoveSpawnedShips();

            //i deaktivira grid da se drugi brodovi mogu spawnat na drugim lokacijama
            EditGrid(selectedShip.GetComponentInChildren<CloseGrid>());

            //postavlja brod kao brod igrača
            if(player1Grid == playerGrid)
            {
                player1Ships.Add(selectedShip.transform);
                SaveReplay.Instance.SetNewShipDataPlayer1(selectedShip.transform.position,
                                                            selectedShip.transform.rotation);
            }
            else
            {
                player2Ships.Add(selectedShip.transform);
                SaveReplay.Instance.SetNewShipDataPlayer2(selectedShip.transform.position,
                                                            selectedShip.transform.rotation);
            }
            currentPrefabID++;
        }  
        if(player1Grid == playerGrid)
        {
            //ponovno aktivira grid od prvog igraca
            foreach(Transform gridPart in disabledPlayer1Grid)
            {
                gridPart.gameObject.SetActive(true);
                player1Grid.Add(gridPart);
            }
            StartCoroutine(CreateShips(player2Grid));
        }
        else
        {
            //ponovno aktivira grid od drugog igraca
            foreach (Transform gridPart in disabledPlayer2Grid)
            {
                gridPart.gameObject.SetActive(true);
                player2Grid.Add(gridPart);
            }
            gameSetupComplete = true;
        }

    }

    void CreateNewShip(Vector3 spawnPoint,GameObject spawnThis)
    {
        //prvi brod
        GameObject newShip = Instantiate(spawnThis,
                    spawnPoint,
                    Quaternion.identity,
                    null);
        spawnedShips.Add(newShip.GetComponent<ShipCollision>());

        //drugi brod
        newShip = Instantiate(spawnThis,
                    spawnPoint,
                    Quaternion.identity,
                    null);
        newShip.transform.Rotate(Vector3.up, 90);
        spawnedShips.Add(newShip.GetComponent<ShipCollision>());

        //treci brod
        newShip = Instantiate(spawnThis,
                    spawnPoint,
                    Quaternion.identity,
                    null);
        newShip.transform.Rotate(Vector3.up, 180);
        spawnedShips.Add(newShip.GetComponent<ShipCollision>());

        //cetvrti brod
        newShip = Instantiate(spawnThis,
                    spawnPoint,
                    Quaternion.identity,
                    null);
        newShip.transform.Rotate(Vector3.up, 270);
        spawnedShips.Add(newShip.GetComponent<ShipCollision>());
    }

    void RemoveSpawnedShips()
    {
        foreach (ShipCollision ship in spawnedShips)
        {
            Destroy(ship.gameObject);
        }
        spawnedShips.Clear();
    }

    void EditGrid(CloseGrid closeGrid)
    {
        //Gasi potrebne gridove od 1 i drugog igraca kada se spawnaju brodovi
        if (player1Grid.Contains(closeGrid.closeGridObjects[1].transform))
        {
            foreach (GameObject grid in closeGrid.closeGridObjects)
            {
                disabledPlayer1Grid.Add(grid.transform);
                player1Grid.Remove(grid.transform);
                grid.SetActive(false);
            }
            return;
        }

        foreach (GameObject grid in closeGrid.closeGridObjects)
        {
            disabledPlayer2Grid.Add(grid.transform);
            player2Grid.Remove(grid.transform);
            grid.SetActive(false);
        }
    }

    //ovo je sekcija za replaye sto se tice generatora
    public void ReplaySpawnShips(List<ShipData> _shipData) 
    {
        //spawna sve brodove na određenim mjestima i pod određenim rotacijama
        foreach(ShipData _ship in _shipData)
        {
            int shipIndex = _shipData.IndexOf(_ship);
            Transform ship =Instantiate(shipsToSpawn[shipIndex], 
                                        _shipData[shipIndex].shipPosition, 
                                        _shipData[shipIndex].shipRotation,
                                        null).transform;
            ship.GetComponent<ShipHP>().isReplay = true;
            player1Ships.Add(ship);
        }
    }

    public void ReplayResetGrid()
    {
        //ovo bi trebalo obrisat postojeće brodove 
        foreach(Transform ship in player1Ships)
        {
            if (ship == null)
                continue;
            Destroy(ship.gameObject);
        }
        player1Ships.Clear();

        //ovo bi trebalo resetirat grid
        foreach(Transform grid in player1Grid)
        {
            grid.GetComponent<GridPart>().ResetGrid();
        }
        foreach (Transform grid in player2Grid)
        {
            grid.GetComponent<GridPart>().ResetGrid();
        }
    }

}
