using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayData
{
    public bool player1Won;
    public bool player2Won;
    public List<ShipData> shipDataPlayer1;
    public List<ShipData> shipDataPlayer2;
    public List<Vector3> gridClickedPositions;

    public ReplayData(bool _player1Won,
                        bool _player2Won,
                        List<Vector3> _gridClickedPositions,
                        List<ShipData> _shipDataPlayer1,
                        List<ShipData> _shipDataPlayer2)
    {
        player1Won = _player1Won;
        player2Won = _player2Won;
        gridClickedPositions = _gridClickedPositions;
        shipDataPlayer1 = _shipDataPlayer1;
        shipDataPlayer2 = _shipDataPlayer2;
    }
}


[System.Serializable]
public class ShipData
{
    public Vector3 shipPosition;
    public Quaternion shipRotation;

    public ShipData(Vector3 _shipPosition,
                    Quaternion _shipRotation)
    {
        shipPosition = _shipPosition;
        shipRotation = _shipRotation;
    }
}
