using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class Playing : MonoBehaviour
{
    public static Playing Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public GameGenerator gameGenerator;
    private int playerPlaying = 1;

    [SerializeField]
    LayerMask gridLayer;

    GameObject newObject;

    public Collider baricadePlayer1;
    public Collider baricadePlayer2;

    public List<MeshRenderer> player1ShipsRenderers;
    public List<MeshRenderer> player2ShipsRenderers;

    [Header("Ui objects")]
    public GameObject endTurnUI;
    public GameObject startGameUI;
    public Button startGameButton;
    public GameObject winnerUI;
    public TextMeshProUGUI winnerText;

    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera player1Camera, player2Camera;
    private void Start()
    {
        startGameButton.interactable = false;
        //its always player 1 turn at the start
        baricadePlayer1.GetComponent<MeshRenderer>().enabled = false;

        baricadePlayer2.GetComponent<MeshRenderer>().enabled = false;
        PlayerMissed();
        startGameUI.SetActive(true);
        endTurnUI.SetActive(false);
        winnerUI.SetActive(false);
        Invoke(nameof(WaitingForGameSetupToComplete), 0.1f);
    }

    void WaitingForGameSetupToComplete() {
        if (gameGenerator.gameSetupComplete)
        {
            Debug.Log("Its Done");
            FillShipMeshes(1, gameGenerator.player1Ships);
            FillShipMeshes(2, gameGenerator.player2Ships);
            startGameButton.interactable = true;
            return;
        }
        Invoke(nameof(WaitingForGameSetupToComplete), 0.1f);
    }

    void FillShipMeshes(int playerID, List<Transform> playerShips )
    {
        if(playerID == 1)
        {
            foreach (Transform ship in playerShips)
            {
                MeshRenderer[] shipMeshes = ship.GetComponentsInChildren<MeshRenderer>();
                Debug.Log(shipMeshes.Length);
                foreach (MeshRenderer mesh in shipMeshes)
                {
                    player1ShipsRenderers.Add(mesh);
                }
            }
            return;
        }

        foreach (Transform ship in playerShips)
        {
            MeshRenderer[] shipMeshes = ship.GetComponentsInChildren<MeshRenderer>();
            Debug.Log(shipMeshes.Length);
            foreach (MeshRenderer mesh in shipMeshes)
            {
                player2ShipsRenderers.Add(mesh);
            }
        }

    }

    void ShowShips(int showThis)
    {
        //ako treba prikazat prvi brod onda aktivira sve renderere od tih brodova
        //a deaktivira sve ond igraca 2
        if(showThis == 1)
        {
            for (int index = 0; index < player1ShipsRenderers.Count; index++)
            {
                if (player1ShipsRenderers[index] != null)
                    player1ShipsRenderers[index].enabled = true;
                if (player2ShipsRenderers[index] != null)
                    player2ShipsRenderers[index].enabled = false;
            }
            baricadePlayer1.enabled = true;
            baricadePlayer2.enabled = false;
            return;
        }

        for (int index = 0; index < player1ShipsRenderers.Count; index++)
        {
            if (player1ShipsRenderers[index] != null)
                player1ShipsRenderers[index].enabled = false;
            if (player2ShipsRenderers[index] != null)
                player2ShipsRenderers[index].enabled = true;
        }
        baricadePlayer1.enabled = false;
        baricadePlayer2.enabled = true;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayer))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            if (newObject != hit.transform.gameObject)
            {
                //mijenja materijal natrag
                try
                {
                    newObject.GetComponent<GridPart>().BackToNormal();
                }
                catch
                {
                    Debug.LogWarning("Old object does not contain GridPartScript");
                }

                //postavlja novi materijal kada je miš na gridu
                newObject = hit.transform.gameObject;
                try
                {
                    newObject.GetComponent<GridPart>().MouseOnGrid();
                }
                catch
                {
                    Debug.LogWarning("New object does not contain GridPartScript");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool hitBoat = newObject.GetComponent<GridPart>().AttackedGrid();
            SaveReplay.Instance.SetClickedGrid(newObject.transform.position);
            //ako je igrac fulo treba se promijenit igra na sljedeceg igraca
            if(hitBoat == false)
            {
                PlayerMissed();
            }

            Debug.Log(hitBoat);
        }
    }

    void PlayerMissed()
    {
        for (int index = 0; index < player1ShipsRenderers.Count; index++)
        {
            if(player1ShipsRenderers[index] != null)
                player1ShipsRenderers[index].enabled = false;
            if (player2ShipsRenderers[index] != null)
                player2ShipsRenderers[index].enabled = false;
        }
        baricadePlayer1.enabled = true;
        baricadePlayer2.enabled = true;
        player1Camera.enabled = false;
        player2Camera.enabled = false;
        endTurnUI.SetActive(true);
    }

    public void NextPlayerTurn()
    {
        if(playerPlaying == 1)
        {
            playerPlaying = 2;
            ShowShips(playerPlaying);
            endTurnUI.SetActive(false);
            player1Camera.enabled = false;
            player2Camera.enabled = true;
            return;
        }

        playerPlaying = 1;
        ShowShips(playerPlaying);
        endTurnUI.SetActive(false);
        player1Camera.enabled = true;
        player2Camera.enabled = false;
    }

    public void StartGame()
    {
        ShowShips(playerPlaying);
        startGameUI.SetActive(false);
        player1Camera.enabled = true;
        player2Camera.enabled = false;
    }

    public void WinCheck()
    {
        StartCoroutine(WinCheckCO());
    }

    IEnumerator WinCheckCO()
    {
        yield return new WaitForSeconds(0.1f);
        bool player1Won = true;
        foreach(Transform player1Ship in gameGenerator.player1Ships)
        {
            if(player1Ship != null)
            {
                player1Won = false;
                break;
            }
        }
        if (player1Won)
        {
            winnerText.text = "Congratilations, Player 1 has won the game!!";
            winnerUI.SetActive(true);
            
        }
        else
        {
            bool player2Won = true;
            foreach (Transform player2Ship in gameGenerator.player2Ships)
            {
                if (player2Ship != null)
                {
                    player2Won = false;
                    break;
                }
            }
            if (player2Won)
            {
                winnerText.text = "Congratilations, Player 2  has won the game!!";
                winnerUI.SetActive(true);

            }
        }
    }
}
