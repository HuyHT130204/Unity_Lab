using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public int coins;
    public List<PlaneData> ownedPlanes = new();
    public bool BGMusicOn;
    public string equippedPlaneName;
    public PlaneData equippedPlane;
}
