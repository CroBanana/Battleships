using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadReplays : MonoBehaviour
{
    ReplayData replayData;
    public List<ReplayData> allReplays = new();
    public GameGenerator gameGenerator;

    //singelton
    public static LoadReplays Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        LoadReplay();
    }

    void LoadReplay()
    {
        // Get the path to the directory where replay files are stored
        string replayDirectory = Application.persistentDataPath;

        // Check if the directory exists
        if (Directory.Exists(replayDirectory))
        {
            // Get all files with a .json extension in the directory
            string[] replayFiles = Directory.GetFiles(replayDirectory, "*.json");

            // Iterate through each file and load its content
            foreach (string filePath in replayFiles)
            {
                Debug.Log("etets");
                try
                {
                    string json = File.ReadAllText(filePath);
                    ReplayData rd = JsonUtility.FromJson<ReplayData>(json);

                    // Add loaded frames to the existing list of replayFrames
                    allReplays.Add(rd);
                }
                catch
                {
                    Debug.Log("Not a replay file\n" + filePath);
                }
            }
        }
    }

    public void UseReplay(int _replayIndex)
    {
        gameGenerator.ReplayResetGrid();
        gameGenerator.ReplaySpawnShips(allReplays[_replayIndex].shipDataPlayer1);
        gameGenerator.ReplaySpawnShips(allReplays[_replayIndex].shipDataPlayer2);
        PlayReplay.Instance.SetVariables(_replayIndex, allReplays[_replayIndex].gridClickedPositions);
    }

}
