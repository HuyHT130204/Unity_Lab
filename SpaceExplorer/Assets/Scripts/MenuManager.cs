using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private UserData userData;

    [SerializeField]
    private PlaneDatabase planeDatabase;

    [SerializeField]
    private TextMeshProUGUI coinsText;

    void Start()
    {
        // PlayerPrefs.DeleteKey("SpaceShooter_UserData"); // For testing, remove in production
        LoadData();
    }

    void LoadData()
    {
        if (PlayerPrefs.HasKey("SpaceShooter_UserData"))
        {
            try
            {
                string json = PlayerPrefs.GetString("SpaceShooter_UserData");
                userData = JsonUtility.FromJson<UserData>(json);
                Debug.Log("User data: " + json);
                Debug.Log(userData);
                coinsText.text = userData.coins.ToString();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error in loading user data: " + ex.Message);
                coinsText.text = "0";
                userData = CreateDefaultUserData();
                SaveUserData();
            }
        }
        else
        {
            coinsText.text = "0";
            userData = CreateDefaultUserData();
            SaveUserData();
        }

        foreach (PlaneData plane in planeDatabase.allPlanes)
        {
            plane.isUnlocked = userData.ownedPlanes.Exists(ownedPlane =>
                ownedPlane.planeName == plane.planeName
            );
        }
    }

    void SaveUserData()
    {
        Debug.Log("Saving user data: " + userData);
        Debug.Log("User data: " + JsonUtility.ToJson(userData));
        string json = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString("SpaceShooter_UserData", json);
        PlayerPrefs.Save();
    }

    UserData CreateDefaultUserData()
    {
        return new UserData
        {
            coins = 0,
            BGMusicOn = true,
            equippedPlaneName = "Triangle",
            ownedPlanes = new List<PlaneData> { planeDatabase.allPlanes[0] },
            equippedPlane = planeDatabase.allPlanes[0],
        };
    }
}
