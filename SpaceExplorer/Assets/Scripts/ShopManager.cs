using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    private UserData userData;

    [SerializeField]
    private TextMeshProUGUI coinsText;

    [SerializeField]
    private GameObject planeCardPrefab;

    [SerializeField]
    private Transform shopContainer;

    [SerializeField]
    private GameObject colorBtnPrefab;

    [SerializeField]
    private PlaneDatabase planeDatabase;

    [SerializeField]
    private GameObject notEnoughCoinsDialog;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadData();
        PopulateShop();
    }

    void LoadData()
    {
        if (PlayerPrefs.HasKey("SpaceShooter_UserData"))
        {
            string json = PlayerPrefs.GetString("SpaceShooter_UserData");
            userData = JsonUtility.FromJson<UserData>(json);
            coinsText.text = userData.coins.ToString();
        }
    }

    void PopulateShop()
    {
        foreach (Transform child in shopContainer)
            Destroy(child.gameObject);

        foreach (PlaneData plane in planeDatabase.allPlanes)
        {
            GameObject cardObj = Instantiate(planeCardPrefab, shopContainer);
            PlaneCardUI cardUI = cardObj.GetComponent<PlaneCardUI>();
            cardUI.colorBtnPrefab = colorBtnPrefab;
            cardUI.Setup(plane, userData, PopulateShop);
        }
    }

    public void ShowNotEnoughCoinsDialog()
    {
        notEnoughCoinsDialog.SetActive(true);
    }
}
