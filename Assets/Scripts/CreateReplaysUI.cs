using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateReplaysUI : MonoBehaviour
{
    public Transform parent;
    public GameObject replayUIPrefab;
    private void Start()
    {
        Invoke(nameof(CreateReplays), 2f);
    }

    void CreateReplays()
    {
        foreach(ReplayData replayData in LoadReplays.Instance.allReplays)
        {
            ReplayUI rUI = Instantiate(replayUIPrefab,parent).GetComponent<ReplayUI>();
            rUI.replayIndex = LoadReplays.Instance.allReplays.IndexOf(replayData);
            Debug.LogWarning(LoadReplays.Instance.allReplays.IndexOf(replayData));
        }
    }
}
