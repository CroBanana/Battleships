using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayUI : MonoBehaviour
{
    public int replayIndex;

    public void UseThisReplay()
    {
        LoadReplays.Instance.UseReplay(replayIndex);
    }
}
