using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;




public class SaveReplay : MonoBehaviour
{
    [Header("Save variables")]
    public List<ShipData> shipDataPlayer1;
    public List<ShipData> shipDataPlayer2;
    public List<Vector3> clickedGrids;

    //singelton
    public static SaveReplay Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Save"))
            PlayerPrefs.SetInt("Save", 0);

            /*
        List<Vector3> test = new();
        test.Add(new(3, 3, 3));
        test.Add(new(0, 0, 0));
        test.Add(new(1, 2, 3));
        replayData = new ReplayData(true, false, test);
        Save();
        LoadReplay();
            */
    }

    public void Save()
    {
        ReplayData replayData = new ReplayData(false, false,
                                               clickedGrids,
                                               shipDataPlayer1,
                                               shipDataPlayer2);
        string json = JsonUtility.ToJson(replayData);
        PlayerPrefs.SetInt("Save", PlayerPrefs.GetInt("Save") + 1);
        string fileName = "Replay " + PlayerPrefs.GetInt("Save").ToString();
        File.WriteAllText(GetFilePath(fileName), json);
    }

    string GetFilePath(string fileName)
    {
        // Define the path where the files will be stored
        return Path.Combine(Application.persistentDataPath, fileName + ".json");
    }

    public void SetClickedGrid(Vector3 _thisClick)
    {
        clickedGrids.Add(_thisClick);
    }

    public void SetNewShipDataPlayer1(Vector3 _position, Quaternion _rotation)
    {
        ShipData newShipData = new ShipData(_position, _rotation);
        shipDataPlayer1.Add(newShipData);
    }
    public void SetNewShipDataPlayer2(Vector3 _position, Quaternion _rotation)
    {
        ShipData newShipData = new ShipData(_position, _rotation);
        shipDataPlayer2.Add(newShipData);
    }
}
