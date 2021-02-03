using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SendPeerData
{
    public List<(string GUID, string Name, Vector3 Position, Vector3 Rotation)> PlayerData;
}
