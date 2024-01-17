using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayReplay : MonoBehaviour
{
    public static PlayReplay Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    int replayIndex;
    List<Vector3> gridActivationSequence;
    int currentGridNumber;
    public float autoPlaySpeed;
    float autoPlaySpeedConstant;
    public bool autoPlaying;
    public Button autoPlayButton;
    public Sprite autoPlayImage, stopAutoPlayImage;
    public LayerMask gridLayer;
    public List<GridPart> activatedGrids = new();

    private void Start()
    {
        autoPlaySpeedConstant = autoPlaySpeed;
        autoPlayButton.image.sprite = autoPlayImage;
    }


    public void SetVariables(int _replayIndex, List<Vector3> _gridActivationSequence)
    {
        replayIndex = _replayIndex;
        gridActivationSequence = _gridActivationSequence;
        autoPlaying = false;
        autoPlaySpeed = autoPlaySpeedConstant;
        currentGridNumber = -1;
        activatedGrids.Clear();
    }

    public void Forward()
    {
        currentGridNumber++;
        if (!GridCheck())
            return;
        RaycastForGrid();
        Debug.Log(gridActivationSequence[currentGridNumber]);
    }

    public void Backward()
    {
        if(activatedGrids != null)
        {
            Debug.Log("Its here");
            activatedGrids[currentGridNumber].ResetGrid();
            Debug.Log("Reset grid was done");
            activatedGrids.RemoveAt(currentGridNumber);
            Debug.Log("This was removed");
            currentGridNumber--;
        }
    }

    public void AutoPlay()
    {
        if (autoPlaying)
        {
            autoPlaying = false;
            autoPlayButton.image.sprite = autoPlayImage;
            autoPlaySpeed = autoPlaySpeedConstant;
            Debug.Log("Auto playing is off");
            return;
        }
        autoPlaying = true;
        autoPlayButton.image.sprite = stopAutoPlayImage;
        Debug.Log("Auto playing is on");
    }

    bool GridCheck()
    {
        if (gridActivationSequence == null)
            return false;

        if(currentGridNumber < 0)
        {
            currentGridNumber = 0;
            return false;
        }

        if (currentGridNumber > gridActivationSequence.Count)
        {
            currentGridNumber = gridActivationSequence.Count;
            return false;
        }

        return true;
    }

    void RaycastForGrid()
    {
        Vector3 startPosition = new Vector3(gridActivationSequence[currentGridNumber].x,
                                            gridActivationSequence[currentGridNumber].y + 10,
                                            gridActivationSequence[currentGridNumber].z);
        Ray ray = new(startPosition, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayer))
        {
            Debug.Log("Grid Hit");
            GridPart hitGrid = hit.transform.GetComponent<GridPart>();
            hitGrid.AttackedGrid();
            activatedGrids.Add(hitGrid);
        }
    }

    private void Update()
    {
        if (autoPlaying)
        {
            autoPlaySpeed -= Time.deltaTime;
            AutoPlaying();
        }
    }

    void AutoPlaying()
    {
        if (autoPlaySpeed < 0)
        {
            currentGridNumber++;
            if (!GridCheck())
                return;
            RaycastForGrid();
            autoPlaySpeed = autoPlaySpeedConstant;
        }
    }
}
